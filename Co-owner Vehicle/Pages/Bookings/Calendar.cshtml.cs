using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Bookings
{
    [Authorize]
    public class CalendarModel : PageModel
    {
        private readonly IBookingService _bookingService;
        private readonly IVehicleService _vehicleService;
        private readonly IGroupService _groupService;

        public CalendarModel(
            IBookingService bookingService,
            IVehicleService vehicleService,
            IGroupService groupService)
        {
            _bookingService = bookingService;
            _vehicleService = vehicleService;
            _groupService = groupService;
        }

        public List<BookingSchedule> Bookings { get; set; } = new();
        public List<Vehicle> Vehicles { get; set; } = new();
        public List<CalendarEvent> CalendarEvents { get; set; } = new();
        public List<BookingSchedule> TodayBookings { get; set; } = new();
        public List<BookingSchedule> UpcomingBookings { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public int? VehicleId { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public BookingStatus? StatusFilter { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public bool MyBookingsOnly { get; set; }
        
        [BindProperty(SupportsGet = true)]
        public int Year { get; set; } = DateTime.UtcNow.Year;
        
        [BindProperty(SupportsGet = true)]
        public int Month { get; set; } = DateTime.UtcNow.Month;

        public class CalendarEvent
        {
            public DateTime Date { get; set; }
            public BookingSchedule Booking { get; set; } = null!;
            public string Color { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
        }

        // Create booking form
        [BindProperty]
        public int SelectedVehicleId { get; set; }
        [BindProperty]
        public DateTime? StartDate { get; set; }
        [BindProperty]
        public TimeSpan? StartTimeOfDay { get; set; }
        [BindProperty]
        public DateTime? EndDate { get; set; }
        [BindProperty]
        public TimeSpan? EndTimeOfDay { get; set; }
        [BindProperty]
        public string? Purpose { get; set; }
        [BindProperty]
        public string? PickupLocation { get; set; }
        [BindProperty]
        public string? ReturnLocation { get; set; }
        [BindProperty]
        public string? Notes { get; set; }

        public async Task OnGetAsync()
        {
            var userId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            // Load vehicles for filter
            if (isAdmin || isStaff)
            {
                Vehicles = await _vehicleService.GetAllVehiclesAsync();
            }
            else
            {
                // Co-owner: Only see vehicles they own
                var userGroups = await _groupService.GetGroupsByUserIdAsync(userId);
                var vehicleIds = userGroups
                    .Where(g => g.Vehicle != null)
                    .Select(g => g.Vehicle!.VehicleId)
                    .Distinct()
                    .ToList();
                
                Vehicles = (await _vehicleService.GetAllVehiclesAsync())
                    .Where(v => vehicleIds.Contains(v.VehicleId))
                    .ToList();
            }

            // Calculate date range for current month
            var monthStart = new DateTime(Year, Month, 1);
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // Load bookings for the month
            var allBookings = await _bookingService.GetBookingsByDateRangeAsync(monthStart, monthEnd);

            // Apply filters
            if (VehicleId.HasValue)
            {
                allBookings = allBookings.Where(b => b.VehicleId == VehicleId.Value).ToList();
            }

            if (StatusFilter.HasValue)
            {
                allBookings = allBookings.Where(b => b.Status == StatusFilter.Value).ToList();
            }

            if (MyBookingsOnly)
            {
                allBookings = allBookings.Where(b => b.UserId == userId).ToList();
            }

            Bookings = allBookings.OrderBy(b => b.StartTime).ToList();

            // Create calendar events
            CalendarEvents = Bookings.Select(b => new CalendarEvent
            {
                Date = b.StartTime.Date,
                Booking = b,
                Color = GetBookingColor(b.Status),
                Title = $"{b.Vehicle?.Brand} {b.Vehicle?.Model} - {b.User?.FullName}"
            }).ToList();

            // Load today's bookings
            var today = DateTime.UtcNow.Date;
            TodayBookings = Bookings
                .Where(b => b.StartTime.Date == today)
                .OrderBy(b => b.StartTime)
                .ToList();

            // Load upcoming bookings (next 7 days)
            var nextWeek = today.AddDays(7);
            UpcomingBookings = Bookings
                .Where(b => b.StartTime.Date > today && b.StartTime.Date <= nextWeek)
                .OrderBy(b => b.StartTime)
                .Take(5)
                .ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            var userId = this.GetCurrentUserId();
            if (SelectedVehicleId <= 0 || !StartDate.HasValue || !StartTimeOfDay.HasValue || !EndDate.HasValue || !EndTimeOfDay.HasValue || string.IsNullOrWhiteSpace(Purpose))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng nhập đầy đủ thông tin đặt lịch");
                await OnGetAsync();
                return Page();
            }

            var start = StartDate.Value.Date + StartTimeOfDay.Value;
            var end = EndDate.Value.Date + EndTimeOfDay.Value;
            if (end <= start)
            {
                ModelState.AddModelError(string.Empty, "Thời gian kết thúc phải sau thời gian bắt đầu");
                await OnGetAsync();
                return Page();
            }

            var available = await _bookingService.IsTimeSlotAvailableAsync(SelectedVehicleId, start, end, null);
            if (!available)
            {
                ModelState.AddModelError(string.Empty, "Khung thời gian đã có người đặt");
                await OnGetAsync();
                return Page();
            }

            var booking = new BookingSchedule
            {
                VehicleId = SelectedVehicleId,
                UserId = userId,
                StartTime = start,
                EndTime = end,
                Purpose = Purpose!,
                PickupLocation = PickupLocation,
                ReturnLocation = ReturnLocation,
                Notes = Notes,
                Status = BookingStatus.Pending
            };

            await _bookingService.CreateBookingAsync(booking);
            TempData["SuccessMessage"] = "Đã tạo yêu cầu đặt lịch";
            return RedirectToPage(new { VehicleId, Year, Month });
        }

        private string GetBookingColor(BookingStatus status)
        {
            return status switch
            {
                BookingStatus.Pending => "bg-warning",
                BookingStatus.Confirmed => "bg-primary",
                BookingStatus.Completed => "bg-success",
                BookingStatus.Cancelled => "bg-danger",
                _ => "bg-secondary"
            };
        }
    }
}

