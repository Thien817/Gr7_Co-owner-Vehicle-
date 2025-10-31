using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Expenses
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IExpenseService _expenseService;
        private readonly IVehicleService _vehicleService;
        private readonly IGroupService _groupService;

        public IndexModel(IExpenseService expenseService, IVehicleService vehicleService, IGroupService groupService)
        {
            _expenseService = expenseService;
            _vehicleService = vehicleService;
            _groupService = groupService;
        }

        public ExpenseStats Stats { get; set; } = new();
        public List<ExpenseViewModel> Expenses { get; set; } = new();
        public List<Vehicle> Vehicles { get; set; } = new();
        public List<ExpenseCategory> Categories { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? VehicleIdFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CategoryIdFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public ExpenseStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? TimeFilter { get; set; } = "Tháng này";

        public async Task OnGetAsync()
        {
            await LoadVehiclesAsync();
            await LoadCategoriesAsync();
            await LoadStatisticsAsync();
            await LoadExpensesAsync();
        }

        private async Task LoadVehiclesAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            if (isAdmin || isStaff)
            {
                Vehicles = (await _vehicleService.GetAllVehiclesAsync())
                    .Where(v => v.Status == VehicleStatus.Active)
                    .OrderBy(v => v.Brand)
                    .ThenBy(v => v.Model)
                    .ToList();
            }
            else
            {
                // Co-owner chỉ xem xe của mình
                var userGroups = await _groupService.GetGroupsByUserIdAsync(currentUserId);
                var vehicleIds = userGroups.Select(g => g.VehicleId).Distinct().ToList();
                var allVehicles = await _vehicleService.GetAllVehiclesAsync();
                Vehicles = allVehicles
                    .Where(v => vehicleIds.Contains(v.VehicleId) && v.Status == VehicleStatus.Active)
                    .OrderBy(v => v.Brand)
                    .ThenBy(v => v.Model)
                    .ToList();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            Categories = (await _expenseService.GetAllExpenseCategoriesAsync())
                .OrderBy(c => c.CategoryName)
                .ToList();
        }

        private async Task LoadStatisticsAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            List<Expense> allExpenses;
            if (isAdmin || isStaff)
            {
                allExpenses = await _expenseService.GetAllExpensesAsync();
            }
            else
            {
                var userGroups = await _groupService.GetGroupsByUserIdAsync(currentUserId);
                var groupIds = userGroups.Select(g => g.CoOwnerGroupId).ToList();
                allExpenses = new List<Expense>();
                foreach (var groupId in groupIds)
                {
                    var groupExpenses = await _expenseService.GetExpensesByGroupIdAsync(groupId);
                    allExpenses.AddRange(groupExpenses);
                }
            }
            var query = allExpenses.AsQueryable();

            // This month expenses
            var thisMonthExpenses = query
                .Where(e => e.ExpenseDate >= thisMonth)
                .ToList();

            var totalThisMonth = thisMonthExpenses.Sum(e => e.Amount);
            var approvedThisMonth = thisMonthExpenses.Count(e => e.Status == ExpenseStatus.Approved);
            var pendingThisMonth = thisMonthExpenses.Count(e => e.Status == ExpenseStatus.Pending);

            // Last month for comparison
            var lastMonthExpenses = query
                .Where(e => e.ExpenseDate >= lastMonth && e.ExpenseDate < thisMonth)
                .ToList();

            var totalLastMonth = lastMonthExpenses.Sum(e => e.Amount);
            var percentageChange = totalLastMonth > 0 
                ? ((totalThisMonth - totalLastMonth) / totalLastMonth) * 100 
                : 0;

            // User's share this month
            decimal userShareThisMonth = 0;
            if (!isAdmin && !isStaff)
            {
                foreach (var expense in thisMonthExpenses)
                {
                    var userShare = await CalculateUserShareAsync(expense, currentUserId);
                    userShareThisMonth += userShare;
                }
            }

            Stats = new ExpenseStats
            {
                TotalThisMonth = totalThisMonth,
                ApprovedThisMonth = approvedThisMonth,
                PendingThisMonth = pendingThisMonth,
                UserShareThisMonth = userShareThisMonth,
                PercentageChange = percentageChange
            };
        }

        private async Task LoadExpensesAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            List<Expense> allExpenses;
            if (isAdmin || isStaff)
            {
                allExpenses = await _expenseService.GetAllExpensesAsync();
            }
            else
            {
                var userGroups = await _groupService.GetGroupsByUserIdAsync(currentUserId);
                var groupIds = userGroups.Select(g => g.CoOwnerGroupId).ToList();
                allExpenses = new List<Expense>();
                foreach (var groupId in groupIds)
                {
                    var groupExpenses = await _expenseService.GetExpensesByGroupIdAsync(groupId);
                    allExpenses.AddRange(groupExpenses);
                }
            }
            var query = allExpenses.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(SearchTerm))
            {
                query = query.Where(e => e.ExpenseTitle.Contains(SearchTerm) || 
                                        e.Description!.Contains(SearchTerm));
            }

            if (VehicleIdFilter.HasValue)
            {
                query = query.Where(e => e.VehicleId == VehicleIdFilter.Value);
            }

            if (CategoryIdFilter.HasValue)
            {
                query = query.Where(e => e.ExpenseCategoryId == CategoryIdFilter.Value);
            }

            if (StatusFilter.HasValue)
            {
                query = query.Where(e => e.Status == StatusFilter.Value);
            }

            // Apply time filter
            var today = DateTime.UtcNow.Date;
            switch (TimeFilter)
            {
                case "Tháng này":
                    var thisMonth = new DateTime(today.Year, today.Month, 1);
                    query = query.Where(e => e.ExpenseDate >= thisMonth);
                    break;
                case "Tháng trước":
                    var lastMonth = new DateTime(today.Year, today.Month, 1).AddMonths(-1);
                    var thisMonthStart = new DateTime(today.Year, today.Month, 1);
                    query = query.Where(e => e.ExpenseDate >= lastMonth && e.ExpenseDate < thisMonthStart);
                    break;
                case "3 tháng":
                    var threeMonthsAgo = today.AddMonths(-3);
                    query = query.Where(e => e.ExpenseDate >= threeMonthsAgo);
                    break;
                // "Tất cả" - no additional filter
            }

            var expenses = query
                .OrderByDescending(e => e.ExpenseDate)
                .Take(50) // Limit for performance
                .ToList();

            Expenses = new List<ExpenseViewModel>();
            foreach (var expense in expenses)
            {
                decimal userShare = 0;
                if (!isAdmin && !isStaff)
                {
                    userShare = await CalculateUserShareAsync(expense, currentUserId);
                }

                Expenses.Add(new ExpenseViewModel
                {
                    ExpenseId = expense.ExpenseId,
                    ExpenseTitle = expense.ExpenseTitle,
                    Description = expense.Description,
                    Amount = expense.Amount,
                    Status = expense.Status,
                    ExpenseDate = expense.ExpenseDate,
                    VendorName = expense.VendorName,
                    ReceiptPath = expense.ReceiptPath,
                    Vehicle = expense.Vehicle,
                    ExpenseCategory = expense.ExpenseCategory,
                    CoOwnerGroup = expense.CoOwnerGroup,
                    ApprovedByUser = expense.ApprovedByUser,
                    UserShare = userShare
                });
            }
        }

        private async Task<decimal> CalculateUserShareAsync(Expense expense, int userId)
        {
            var ownershipShare = await _groupService.GetUserOwnershipShareAsync(expense.CoOwnerGroupId, userId);

            if (ownershipShare == null) return 0;

            decimal ownershipPercentage = ownershipShare.Percentage;

            if (expense.SplitMethod == SplitMethod.Equal)
            {
                var members = await _groupService.GetGroupMembersAsync(expense.CoOwnerGroupId);
                var activeCount = members.Count(gm => gm.Status == MemberStatus.Active);
                return activeCount > 0 ? expense.Amount / activeCount : 0;
            }

            // Use service method for ownership split
            if (expense.SplitMethod == SplitMethod.ByOwnership)
            {
                var split = await _expenseService.CalculateExpenseSplitByOwnershipAsync(expense.ExpenseId);
                return split.ContainsKey(userId) ? split[userId] : 0;
            }

            // Default or other methods
            return expense.Amount * (ownershipPercentage / 100);
        }
    }

    public class ExpenseStats
    {
        public decimal TotalThisMonth { get; set; }
        public int ApprovedThisMonth { get; set; }
        public int PendingThisMonth { get; set; }
        public decimal UserShareThisMonth { get; set; }
        public decimal PercentageChange { get; set; }
    }

    public class ExpenseViewModel
    {
        public int ExpenseId { get; set; }
        public string ExpenseTitle { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public ExpenseStatus Status { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string? VendorName { get; set; }
        public string? ReceiptPath { get; set; }
        public Vehicle Vehicle { get; set; } = null!;
        public ExpenseCategory ExpenseCategory { get; set; } = null!;
        public CoOwnerGroup CoOwnerGroup { get; set; } = null!;
        public User? ApprovedByUser { get; set; }
        public decimal UserShare { get; set; }
    }
}

