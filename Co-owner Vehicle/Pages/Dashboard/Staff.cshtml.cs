using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Data;

namespace Co_owner_Vehicle.Pages.Dashboard
{
    [Authorize(Roles = "Staff")]
    public class StaffModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IVehicleService _vehicleService;
        private readonly CoOwnerVehicleDbContext _context;

        public StaffModel(
            IBookingService bookingService,
            IVehicleService vehicleService,
            CoOwnerVehicleDbContext context)
        {
            _bookingService = bookingService;
            _vehicleService = vehicleService;
            _context = context;
        }

        // Statistics
        public StaffStats Stats { get; set; } = new();

        // Pending Bookings
        public List<BookingSchedule> PendingBookings { get; set; } = new();

        // Today's Schedule
        public List<BookingSchedule> TodayBookings { get; set; } = new();
        public List<CheckInOutRecord> TodayCheckIns { get; set; } = new();
        public List<CheckInOutRecord> TodayCheckOuts { get; set; } = new();
        public List<ServiceRecord> TodayServices { get; set; } = new();

        // Vehicle Status
        public VehicleStatusStats VehicleStats { get; set; } = new();

        // Maintenance Alerts
        public List<MaintenanceAlert> MaintenanceAlerts { get; set; } = new();

        public class StaffStats
        {
            public int PendingBookings { get; set; }
            public int TodayBookings { get; set; }
            public int CompletedTodayBookings { get; set; }
            public int TodayCheckIns { get; set; }
            public int TodayCheckOuts { get; set; }
            public int VehiclesNeedingMaintenance { get; set; }
        }

        public class VehicleStatusStats
        {
            public int TotalVehicles { get; set; }
            public int ActiveVehicles { get; set; }
            public int InUseVehicles { get; set; }
            public int MaintenanceVehicles { get; set; }
            public int InactiveVehicles { get; set; }
        }

        public class MaintenanceAlert
        {
            public Vehicle Vehicle { get; set; } = null!;
            public string Message { get; set; } = string.Empty;
            public string AlertLevel { get; set; } = string.Empty; // "danger", "warning", "info"
            public int DaysUntil { get; set; }
        }

        public async Task OnGetAsync()
        {
            var today = DateTime.UtcNow.Date;
            var todayEnd = today.AddDays(1).AddTicks(-1);

            await LoadPendingBookingsAsync();
            await LoadTodayScheduleAsync(today, todayEnd);
            await LoadVehicleStatusAsync();
            await LoadMaintenanceAlertsAsync();
            await CalculateStatsAsync(today, todayEnd);
        }

        private async Task LoadPendingBookingsAsync()
        {
            PendingBookings = await _bookingService.GetPendingBookingsAsync();
            PendingBookings = PendingBookings
                .OrderBy(b => b.CreatedAt)
                .Take(10)
                .ToList();
        }

        private async Task LoadTodayScheduleAsync(DateTime todayStart, DateTime todayEnd)
        {
            // Today's bookings
            var allBookings = await _bookingService.GetAllBookingsAsync();
            TodayBookings = allBookings
                .Where(b => b.StartTime.Date == todayStart.Date || 
                           (b.StartTime <= todayEnd && b.EndTime >= todayStart))
                .OrderBy(b => b.StartTime)
                .ToList();

            // Today's check-ins
            TodayCheckIns = await _context.CheckInOutRecords
                .Include(c => c.Vehicle)
                .Include(c => c.User)
                .Include(c => c.BookingSchedule)
                .Where(c => c.CheckTime.Date == todayStart.Date && 
                           c.Type == CheckInOutType.CheckIn)
                .OrderByDescending(c => c.CheckTime)
                .ToListAsync();

            // Today's check-outs
            TodayCheckOuts = await _context.CheckInOutRecords
                .Include(c => c.Vehicle)
                .Include(c => c.User)
                .Include(c => c.BookingSchedule)
                .Where(c => c.CheckTime.Date == todayStart.Date && 
                           c.Type == CheckInOutType.CheckOut)
                .OrderByDescending(c => c.CheckTime)
                .ToListAsync();

            // Today's scheduled services
            TodayServices = await _context.ServiceRecords
                .Include(s => s.Vehicle)
                .Include(s => s.VehicleService)
                .Where(s => s.ScheduledDate.Date == todayStart.Date)
                .OrderBy(s => s.ScheduledDate)
                .ToListAsync();
        }

        private async Task LoadVehicleStatusAsync()
        {
            var allVehicles = await _vehicleService.GetAllVehiclesAsync();
            VehicleStats.TotalVehicles = allVehicles.Count;
            VehicleStats.ActiveVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Active);
            VehicleStats.InUseVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Repair); // Using Repair as InUse equivalent
            VehicleStats.MaintenanceVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Maintenance);
            VehicleStats.InactiveVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Inactive);
        }

        private async Task LoadMaintenanceAlertsAsync()
        {
            var alerts = new List<MaintenanceAlert>();
            var vehicles = await _vehicleService.GetAllVehiclesAsync();
            
            // Check vehicles approaching maintenance milestones
            foreach (var vehicle in vehicles.Where(v => v.Status == VehicleStatus.Active))
            {
                // Check for upcoming maintenance milestones (every 10,000km)
                var nextMaintenanceKm = ((vehicle.CurrentMileage / 10000) + 1) * 10000;
                var kmRemaining = nextMaintenanceKm - vehicle.CurrentMileage;

                if (kmRemaining <= 1000)
                {
                    alerts.Add(new MaintenanceAlert
                    {
                        Vehicle = vehicle,
                        Message = $"Sắp đến hạn bảo dưỡng {nextMaintenanceKm:N0}km",
                        AlertLevel = kmRemaining <= 300 ? "danger" : "warning",
                        DaysUntil = 0 // Simplified
                    });
                }

                // Check if maintenance is overdue
                if (vehicle.CurrentMileage % 10000 < 1000 && vehicle.CurrentMileage > 10000)
                {
                    alerts.Add(new MaintenanceAlert
                    {
                        Vehicle = vehicle,
                        Message = $"Đã qua hạn bảo dưỡng {((vehicle.CurrentMileage / 10000) * 10000):N0}km",
                        AlertLevel = "danger",
                        DaysUntil = 0
                    });
                }
            }

            MaintenanceAlerts = alerts.OrderBy(a => a.AlertLevel == "danger" ? 0 : 1).Take(10).ToList();
        }

        private Task CalculateStatsAsync(DateTime todayStart, DateTime todayEnd)
        {
            Stats.PendingBookings = PendingBookings.Count;
            Stats.TodayBookings = TodayBookings.Count;
            Stats.CompletedTodayBookings = TodayBookings.Count(b => 
                b.Status == BookingStatus.Completed || 
                (b.EndTime <= DateTime.UtcNow && b.Status == BookingStatus.Confirmed));
            Stats.TodayCheckIns = TodayCheckIns.Count;
            Stats.TodayCheckOuts = TodayCheckOuts.Count;
            Stats.VehiclesNeedingMaintenance = MaintenanceAlerts.Count(a => a.AlertLevel == "danger" || a.AlertLevel == "warning");
            return Task.CompletedTask;
        }

        public List<ScheduleItem> GetTodayScheduleItems()
        {
            var items = new List<ScheduleItem>();
            var now = DateTime.UtcNow;

            // Add check-outs
            foreach (var checkOut in TodayCheckOuts)
            {
                items.Add(new ScheduleItem
                {
                    Time = checkOut.CheckTime,
                    Type = "checkout",
                    Vehicle = checkOut.Vehicle,
                    User = checkOut.User,
                    BookingSchedule = checkOut.BookingSchedule,
                    Description = $"Check-out: {checkOut.Vehicle?.Brand} {checkOut.Vehicle?.Model}",
                    Status = checkOut.Status == CheckInOutStatus.Completed ? "completed" : 
                             checkOut.CheckTime <= now ? "ongoing" : "upcoming"
                });
            }

            // Add check-ins
            foreach (var checkIn in TodayCheckIns)
            {
                items.Add(new ScheduleItem
                {
                    Time = checkIn.CheckTime,
                    Type = "checkin",
                    Vehicle = checkIn.Vehicle,
                    User = checkIn.User,
                    Description = $"Check-in: {checkIn.Vehicle?.Brand} {checkIn.Vehicle?.Model}",
                    Status = checkIn.Status == CheckInOutStatus.Completed ? "completed" : "upcoming"
                });
            }

            // Add services
            foreach (var service in TodayServices)
            {
                items.Add(new ScheduleItem
                {
                    Time = service.ScheduledDate,
                    Type = "service",
                    Vehicle = service.Vehicle,
                    Description = $"Bảo dưỡng: {service.VehicleService?.ServiceName}",
                    Status = service.Status == ServiceStatus.Completed ? "completed" : 
                             service.Status == ServiceStatus.InProgress ? "ongoing" : "scheduled"
                });
            }

            return items.OrderBy(i => i.Time).ToList();
        }

        public class ScheduleItem
        {
            public DateTime Time { get; set; }
            public string Type { get; set; } = string.Empty; // "checkout", "checkin", "service"
            public Vehicle? Vehicle { get; set; }
            public User? User { get; set; }
            public BookingSchedule? BookingSchedule { get; set; }
            public string Description { get; set; } = string.Empty;
            public string Status { get; set; } = string.Empty; // "completed", "ongoing", "upcoming", "scheduled"
        }
    }
}

