using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.Expenses
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public IndexModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
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
                Vehicles = await _context.Vehicles
                    .Where(v => v.Status == VehicleStatus.Active)
                    .OrderBy(v => v.Brand)
                    .ThenBy(v => v.Model)
                    .ToListAsync();
            }
            else
            {
                // Co-owner chỉ xem xe của mình
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                var vehicleIds = await _context.CoOwnerGroups
                    .Where(g => userGroups.Contains(g.CoOwnerGroupId))
                    .Select(g => g.VehicleId)
                    .Distinct()
                    .ToListAsync();

                Vehicles = await _context.Vehicles
                    .Where(v => vehicleIds.Contains(v.VehicleId) && v.Status == VehicleStatus.Active)
                    .OrderBy(v => v.Brand)
                    .ThenBy(v => v.Model)
                    .ToListAsync();
            }
        }

        private async Task LoadCategoriesAsync()
        {
            Categories = await _context.ExpenseCategories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();
        }

        private async Task LoadStatisticsAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            var today = DateTime.UtcNow.Date;
            var thisMonth = new DateTime(today.Year, today.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);

            var query = _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.CoOwnerGroup)
                .AsQueryable();

            // Apply role-based filtering
            if (!isAdmin && !isStaff)
            {
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                query = query.Where(e => userGroups.Contains(e.CoOwnerGroupId));
            }

            // This month expenses
            var thisMonthExpenses = await query
                .Where(e => e.ExpenseDate >= thisMonth)
                .ToListAsync();

            var totalThisMonth = thisMonthExpenses.Sum(e => e.Amount);
            var approvedThisMonth = thisMonthExpenses.Count(e => e.Status == ExpenseStatus.Approved);
            var pendingThisMonth = thisMonthExpenses.Count(e => e.Status == ExpenseStatus.Pending);

            // Last month for comparison
            var lastMonthExpenses = await query
                .Where(e => e.ExpenseDate >= lastMonth && e.ExpenseDate < thisMonth)
                .ToListAsync();

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

            var query = _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .AsQueryable();

            // Apply role-based filtering
            if (!isAdmin && !isStaff)
            {
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                query = query.Where(e => userGroups.Contains(e.CoOwnerGroupId));
            }

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

            var expenses = await query
                .OrderByDescending(e => e.ExpenseDate)
                .Take(50) // Limit for performance
                .ToListAsync();

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
            var ownershipShare = await _context.OwnershipShares
                .FirstOrDefaultAsync(os => os.UserId == userId && 
                                         os.CoOwnerGroupId == expense.CoOwnerGroupId && 
                                         os.IsActive);

            if (ownershipShare == null) return 0;

            decimal ownershipPercentage = ownershipShare.Percentage;

            return expense.SplitMethod switch
            {
                SplitMethod.ByOwnership => expense.Amount * (ownershipPercentage / 100),
                SplitMethod.Equal => expense.Amount / (await _context.GroupMembers
                    .CountAsync(gm => gm.CoOwnerGroupId == expense.CoOwnerGroupId && gm.Status == MemberStatus.Active)),
                _ => expense.Amount * (ownershipPercentage / 100) // Default to ownership
            };
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

