using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Data;

namespace Co_owner_Vehicle.Pages.Dashboard
{
    [Authorize(Roles = "Co-owner")]
    public class CoOwnerModel : PageModel
    {
        private readonly IGroupService _groupService;
        private readonly IBookingService _bookingService;
        private readonly IExpenseService _expenseService;
        private readonly IPaymentService _paymentService;
        private readonly CoOwnerVehicleDbContext _context;

        public CoOwnerModel(
            IGroupService groupService,
            IBookingService bookingService,
            IExpenseService expenseService,
            IPaymentService paymentService,
            CoOwnerVehicleDbContext context)
        {
            _groupService = groupService;
            _bookingService = bookingService;
            _expenseService = expenseService;
            _paymentService = paymentService;
            _context = context;
        }

        // User Groups và Vehicles
        public List<CoOwnerGroup> UserGroups { get; set; } = new();
        public List<Vehicle> UserVehicles { get; set; } = new();
        public Dictionary<int, decimal> OwnershipShares { get; set; } = new(); // VehicleId -> Percentage

        // Recent Bookings
        public List<BookingSchedule> RecentBookings { get; set; } = new();
        public List<BookingSchedule> UpcomingBookings { get; set; } = new();

        // Expenses & Payments
        public List<Expense> RecentExpenses { get; set; } = new();
        public List<Expense> PendingExpenses { get; set; } = new();
        public List<Payment> OutstandingPayments { get; set; } = new();

        // Notifications
        public List<Notification> Notifications { get; set; } = new();

        // Statistics
        public DashboardStats Stats { get; set; } = new();

        public class DashboardStats
        {
            public int TotalVehicles { get; set; }
            public int ActiveVehicles { get; set; }
            public int BookingsThisMonth { get; set; }
            public int UpcomingBookings { get; set; }
            public decimal TotalExpensesThisMonth { get; set; }
            public decimal OutstandingAmount { get; set; }
            public int PendingExpenseCount { get; set; }
            public int PendingPaymentCount { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = this.GetCurrentUserId();
            if (userId == 0) return;

            // Load user's groups
            UserGroups = await _groupService.GetGroupsByUserIdAsync(userId);

            // Extract vehicles from groups
            UserVehicles = UserGroups
                .Where(g => g.Vehicle != null)
                .Select(g => g.Vehicle!)
                .Distinct()
                .ToList();

            // Load ownership shares
            foreach (var group in UserGroups)
            {
                var shares = await _groupService.GetOwnershipSharesAsync(group.CoOwnerGroupId);
                var userShare = shares.FirstOrDefault(s => s.UserId == userId);
                if (userShare != null && group.Vehicle != null)
                {
                    OwnershipShares[group.Vehicle.VehicleId] = userShare.Percentage;
                }
            }

            // Load recent bookings (last 30 days)
            var recentBookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            RecentBookings = recentBookings
                .Where(b => b.StartTime >= DateTime.UtcNow.AddDays(-30))
                .OrderByDescending(b => b.StartTime)
                .Take(5)
                .ToList();

            // Load upcoming bookings
            UpcomingBookings = recentBookings
                .Where(b => b.StartTime >= DateTime.UtcNow && b.Status == BookingStatus.Confirmed)
                .OrderBy(b => b.StartTime)
                .Take(5)
                .ToList();

            // Load expenses from user's groups
            var allExpenses = new List<Expense>();
            foreach (var group in UserGroups)
            {
                var groupExpenses = await _expenseService.GetExpensesByGroupIdAsync(group.CoOwnerGroupId);
                allExpenses.AddRange(groupExpenses);
            }

            // Recent expenses (last 30 days)
            RecentExpenses = allExpenses
                .Where(e => e.ExpenseDate >= DateTime.UtcNow.AddDays(-30))
                .OrderByDescending(e => e.ExpenseDate)
                .Take(5)
                .ToList();

            // Pending expenses
            PendingExpenses = allExpenses
                .Where(e => e.Status == ExpenseStatus.Pending)
                .OrderByDescending(e => e.ExpenseDate)
                .Take(5)
                .ToList();

            // Load outstanding payments
            OutstandingPayments = await _paymentService.GetOutstandingPaymentsAsync(userId);

            // Load recent notifications
            Notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Calculate statistics
            Stats = new DashboardStats
            {
                TotalVehicles = UserVehicles.Count,
                ActiveVehicles = UserVehicles.Count(v => v.Status == VehicleStatus.Active),
                BookingsThisMonth = recentBookings.Count(b => 
                    b.StartTime.Month == DateTime.UtcNow.Month && 
                    b.StartTime.Year == DateTime.UtcNow.Year),
                UpcomingBookings = UpcomingBookings.Count,
                TotalExpensesThisMonth = await CalculateTotalExpensesThisMonth(userId),
                OutstandingAmount = await _paymentService.GetOutstandingAmountAsync(userId),
                PendingExpenseCount = PendingExpenses.Count,
                PendingPaymentCount = OutstandingPayments.Count
            };
        }

        private async Task<decimal> CalculateTotalExpensesThisMonth(int userId)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;
            var startDate = new DateTime(currentYear, currentMonth, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            return await _expenseService.GetTotalExpensesByUserAsync(userId, startDate, endDate);
        }

        public decimal GetUserExpenseShare(int expenseId, int userId)
        {
            var expense = RecentExpenses.FirstOrDefault(e => e.ExpenseId == expenseId);
            if (expense == null) return 0;

            // This is simplified - actual calculation should use ExpenseService.CalculateExpenseSplitAsync
            // For now, return approximate based on ownership percentage
            var group = UserGroups.FirstOrDefault(g => g.CoOwnerGroupId == expense.CoOwnerGroupId);
            if (group == null || group.Vehicle == null) return 0;

            var ownershipPercentage = OwnershipShares.GetValueOrDefault(group.Vehicle.VehicleId, 0);
            return expense.Amount * (ownershipPercentage / 100);
        }
    }
}

