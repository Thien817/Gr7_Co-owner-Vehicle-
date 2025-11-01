using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Dashboard
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IVehicleService _vehicleService;
        private readonly IGroupService _groupService;
        private readonly IBookingService _bookingService;
        private readonly IExpenseService _expenseService;
        private readonly IPaymentService _paymentService;

        public AdminModel(
            IUserService userService,
            IVehicleService vehicleService,
            IGroupService groupService,
            IBookingService bookingService,
            IExpenseService expenseService,
            IPaymentService paymentService)
        {
            _userService = userService;
            _vehicleService = vehicleService;
            _groupService = groupService;
            _bookingService = bookingService;
            _expenseService = expenseService;
            _paymentService = paymentService;
        }

        // System Statistics
        public AdminStats Stats { get; set; } = new();

        // Recent Activities
        public List<ActivityItem> RecentActivities { get; set; } = new();

        // Pending Actions
        public PendingActions Pending { get; set; } = new();

        public class AdminStats
        {
            public int TotalUsers { get; set; }
            public int ActiveUsers { get; set; }
            public int NewUsersThisMonth { get; set; }
            public int TotalVehicles { get; set; }
            public int ActiveVehicles { get; set; }
            public int NewVehiclesThisMonth { get; set; }
            public int TotalGroups { get; set; }
            public int ActiveGroups { get; set; }
            public int TotalBookings { get; set; }
            public int BookingsThisMonth { get; set; }
            public decimal TotalRevenue { get; set; } // Total payments
        }

        public class ActivityItem
        {
            public string Type { get; set; } = string.Empty; // "user", "vehicle", "booking", "expense"
            public string IconClass { get; set; } = string.Empty;
            public string BgClass { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public string TimeAgo { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
        }

        public class PendingActions
        {
            public int PendingBookings { get; set; }
            public int PendingExpenses { get; set; }
            public int UnverifiedUsers { get; set; }
            public int PendingVerifications { get; set; }
        }

        public async Task OnGetAsync()
        {
            await LoadSystemStatsAsync();
            await LoadRecentActivitiesAsync();
            await LoadPendingActionsAsync();
        }

        private async Task LoadSystemStatsAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var startOfLastMonth = startOfMonth.AddMonths(-1);
            var endOfLastMonth = startOfMonth.AddDays(-1);

            // Users
            var allUsers = await _userService.GetAllUsersAsync();
            Stats.TotalUsers = allUsers.Count;
            Stats.ActiveUsers = allUsers.Count(u => u.IsActive);
            Stats.NewUsersThisMonth = allUsers.Count(u => u.CreatedAt >= startOfMonth);

            // Vehicles
            var allVehicles = await _vehicleService.GetAllVehiclesAsync();
            Stats.TotalVehicles = allVehicles.Count;
            Stats.ActiveVehicles = allVehicles.Count(v => v.Status == VehicleStatus.Active);
            Stats.NewVehiclesThisMonth = allVehicles.Count(v => v.CreatedAt >= startOfMonth);

            // Groups
            var allGroups = await _groupService.GetAllGroupsAsync();
            Stats.TotalGroups = allGroups.Count;
            Stats.ActiveGroups = allGroups.Count(g => g.Status == GroupStatus.Active);

            // Bookings
            var allBookings = await _bookingService.GetAllBookingsAsync();
            Stats.TotalBookings = allBookings.Count;
            Stats.BookingsThisMonth = allBookings.Count(b => b.CreatedAt >= startOfMonth);

            // Revenue (total payments)
            Stats.TotalRevenue = await _paymentService.GetTotalRevenueAsync(startOfMonth, null);
        }

        private async Task LoadRecentActivitiesAsync()
        {
            var activities = new List<ActivityItem>();
            var now = DateTime.UtcNow;

            // Recent users (last 10)
            var recentUsers = await _userService.GetRecentUsersAsync(5);

            foreach (var user in recentUsers)
            {
                var timeAgo = GetTimeAgo(user.CreatedAt, now);
                activities.Add(new ActivityItem
                {
                    Type = "user",
                    IconClass = "bi-person-plus",
                    BgClass = "bg-success",
                    Title = "Người dùng mới đăng ký",
                    Description = $"{user.FullName} - {user.Email}",
                    TimeAgo = timeAgo,
                    CreatedAt = user.CreatedAt
                });
            }

            // Recent vehicles (last 5)
            var recentVehicles = await _vehicleService.GetRecentVehiclesAsync(5);

            foreach (var vehicle in recentVehicles)
            {
                var timeAgo = GetTimeAgo(vehicle.CreatedAt, now);
                activities.Add(new ActivityItem
                {
                    Type = "vehicle",
                    IconClass = "bi-car-front",
                    BgClass = "bg-primary",
                    Title = "Xe mới được thêm",
                    Description = $"{vehicle.Brand} {vehicle.Model} - {vehicle.LicensePlate}",
                    TimeAgo = timeAgo,
                    CreatedAt = vehicle.CreatedAt
                });
            }

            // Recent pending bookings (last 5)
            var allPendingBookings = await _bookingService.GetPendingBookingsAsync();
            var pendingBookings = allPendingBookings
                .OrderByDescending(b => b.CreatedAt)
                .Take(5)
                .ToList();

            foreach (var booking in pendingBookings)
            {
                var timeAgo = GetTimeAgo(booking.CreatedAt, now);
                activities.Add(new ActivityItem
                {
                    Type = "booking",
                    IconClass = "bi-calendar-check",
                    BgClass = "bg-warning",
                    Title = "Lịch đặt cần duyệt",
                    Description = $"{booking.User?.FullName} - {booking.Vehicle?.Brand} {booking.Vehicle?.Model}",
                    TimeAgo = timeAgo,
                    CreatedAt = booking.CreatedAt
                });
            }

            // Recent pending expenses (last 5)
            var allPendingExpenses = await _expenseService.GetPendingApprovalExpensesAsync();
            var pendingExpenses = allPendingExpenses
                .Take(5)
                .ToList();

            foreach (var expense in pendingExpenses)
            {
                var timeAgo = GetTimeAgo(expense.CreatedAt, now);
                activities.Add(new ActivityItem
                {
                    Type = "expense",
                    IconClass = "bi-receipt",
                    BgClass = "bg-danger",
                    Title = "Chi phí cần phê duyệt",
                    Description = $"{expense.ExpenseTitle} - {expense.Amount:N0}₫",
                    TimeAgo = timeAgo,
                    CreatedAt = expense.CreatedAt
                });
            }

            // Sort by date and take most recent
            RecentActivities = activities
                .OrderByDescending(a => a.CreatedAt)
                .Take(10)
                .ToList();
        }

        private async Task LoadPendingActionsAsync()
        {
            // Pending bookings
            var allPendingBookings = await _bookingService.GetPendingBookingsAsync();
            Pending.PendingBookings = allPendingBookings.Count;

            // Pending expenses
            var allPendingExpenses = await _expenseService.GetPendingApprovalExpensesAsync();
            Pending.PendingExpenses = allPendingExpenses.Count;

            // Unverified users
            Pending.UnverifiedUsers = await _userService.GetUnverifiedUsersCountAsync();

            // Pending verifications
            Pending.PendingVerifications = await _userService.GetPendingVerificationsCountAsync();
        }

        private string GetTimeAgo(DateTime dateTime, DateTime now)
        {
            var timeSpan = now - dateTime;

            if (timeSpan.TotalMinutes < 1)
                return "Vừa xong";
            else if (timeSpan.TotalMinutes < 60)
                return $"{(int)timeSpan.TotalMinutes} phút trước";
            else if (timeSpan.TotalHours < 24)
                return $"{(int)timeSpan.TotalHours} giờ trước";
            else if (timeSpan.TotalDays < 30)
                return $"{(int)timeSpan.TotalDays} ngày trước";
            else if (timeSpan.TotalDays < 365)
                return $"{(int)(timeSpan.TotalDays / 30)} tháng trước";
            else
                return $"{(int)(timeSpan.TotalDays / 365)} năm trước";
        }
    }
}

