using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace Co_owner_Vehicle.Pages.Vehicles
{
    [Authorize]
    public class DetailsModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IBookingService _bookingService;
        private readonly IExpenseService _expenseService;
        private readonly IServiceRecordService _serviceRecordService;
        private readonly IGroupService _groupService;

        public DetailsModel(
            IVehicleService vehicleService,
            IBookingService bookingService,
            IExpenseService expenseService,
            IServiceRecordService serviceRecordService,
            IGroupService groupService)
        {
            _vehicleService = vehicleService;
            _bookingService = bookingService;
            _expenseService = expenseService;
            _serviceRecordService = serviceRecordService;
            _groupService = groupService;
        }

        [BindProperty(SupportsGet = true)]
        public int? Id { get; set; }

        public Vehicle? Vehicle { get; set; }

        // Quick Stats
        public class QuickStatsVm
        {
            public int TotalKm { get; set; }
            public decimal MonthExpense { get; set; }
            public int MonthBookings { get; set; }
        }
        public QuickStatsVm Stats { get; set; } = new();

        public class RecentBookingVm
        {
            public string UserName { get; set; } = string.Empty;
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public int DistanceKm { get; set; }
        }
        public List<RecentBookingVm> RecentBookings { get; set; } = new();

        public class ExpenseVm
        {
            public string Title { get; set; } = string.Empty;
            public DateTime Date { get; set; }
            public decimal Amount { get; set; }
            public ExpenseStatus Status { get; set; }
        }
        public List<ExpenseVm> RecentExpenses { get; set; } = new();

        public class OwnerVm
        {
            public string UserName { get; set; } = string.Empty;
            public string Email { get; set; } = string.Empty;
            public decimal Percentage { get; set; }
            public decimal InvestmentAmount { get; set; }
            public MemberRole Role { get; set; }
        }
        public List<OwnerVm> Owners { get; set; } = new();

        public class UpcomingBookingVm
        {
            public DateTime Start { get; set; }
            public DateTime End { get; set; }
            public string UserName { get; set; } = string.Empty;
            public BookingStatus Status { get; set; }
        }
        public List<UpcomingBookingVm> UpcomingBookings { get; set; } = new();

        public class MaintenanceVm
        {
            public string Title { get; set; } = string.Empty;
            public DateTime? LastDate { get; set; }
            public ServiceStatus Status { get; set; }
        }
        public List<MaintenanceVm> Maintenances { get; set; } = new();

        public async Task OnGetAsync()
        {
            if (Id.HasValue)
            {
                Vehicle = await _vehicleService.GetVehicleByIdAsync(Id.Value);
                if (Vehicle == null) return;

                // Quick stats
                var today = DateTime.UtcNow.Date;
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var bookings = await _bookingService.GetBookingsByVehicleIdAsync(Vehicle.VehicleId);
                var monthBookings = bookings.Where(b => b.StartTime >= monthStart).ToList();
                Stats = new QuickStatsVm
                {
                    TotalKm = Vehicle.CurrentMileage ?? 0,
                    MonthBookings = monthBookings.Count,
                    MonthExpense = (await _expenseService.GetExpensesByVehicleIdAsync(Vehicle.VehicleId))
                        .Where(e => e.ExpenseDate >= monthStart)
                        .Sum(e => e.Amount)
                };

                // Recent usage (completed bookings)
                var recent = bookings
                    .Where(b => b.Status == BookingStatus.Completed)
                    .OrderByDescending(b => b.EndTime)
                    .Take(3)
                    .ToList();
                RecentBookings = recent.Select(b => new RecentBookingVm
                {
                    UserName = b.User?.FullName ?? $"User #{b.UserId}",
                    StartTime = b.StartTime,
                    EndTime = b.EndTime,
                    DistanceKm = b.EstimatedMileage ?? 0
                }).ToList();

                // Expenses
                RecentExpenses = (await _expenseService.GetExpensesByVehicleIdAsync(Vehicle.VehicleId))
                    .OrderByDescending(e => e.ExpenseDate)
                    .Take(3)
                    .Select(e => new ExpenseVm
                    {
                        Title = e.ExpenseTitle,
                        Date = e.ExpenseDate,
                        Amount = e.Amount,
                        Status = e.Status
                    }).ToList();

                // Ownership
                var groups = await _groupService.GetGroupsByVehicleIdAsync(Vehicle.VehicleId);
                if (groups.Any())
                {
                    var group = groups.First();
                    var members = await _groupService.GetGroupMembersAsync(group.CoOwnerGroupId);
                    var shares = await _groupService.GetOwnershipSharesAsync(group.CoOwnerGroupId);
                    Owners = members.Select(m => new OwnerVm
                    {
                        UserName = m.User.FullName,
                        Email = m.User.Email,
                        Role = m.Role,
                        Percentage = shares.FirstOrDefault(s => s.UserId == m.UserId)?.Percentage ?? 0,
                        InvestmentAmount = shares.FirstOrDefault(s => s.UserId == m.UserId)?.InvestmentAmount ?? 0
                    })
                    .OrderByDescending(o => o.Role)
                    .ThenByDescending(o => o.Percentage)
                    .ToList();
                }

                // Upcoming bookings
                UpcomingBookings = bookings
                    .Where(b => b.StartTime >= today && (b.Status == BookingStatus.Confirmed || b.Status == BookingStatus.Pending))
                    .OrderBy(b => b.StartTime)
                    .Take(3)
                    .Select(b => new UpcomingBookingVm
                    {
                        Start = b.StartTime,
                        End = b.EndTime,
                        UserName = b.User?.FullName ?? $"User #{b.UserId}",
                        Status = b.Status
                    }).ToList();

                // Maintenances
                var services = await _serviceRecordService.GetByVehicleAsync(Vehicle.VehicleId);
                Maintenances = services
                    .OrderByDescending(s => s.CompletedAt ?? s.ScheduledDate)
                    .Take(3)
                    .Select(s => new MaintenanceVm
                    {
                        Title = s.VehicleService?.ServiceName ?? s.VehicleServiceId.ToString(),
                        LastDate = s.CompletedAt ?? s.ScheduledDate,
                        Status = s.Status
                    }).ToList();
            }
        }
    }
}

