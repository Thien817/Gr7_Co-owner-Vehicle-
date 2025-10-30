using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly CoOwnerVehicleDbContext _context;

        public ExpenseService(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public async Task<Expense?> GetExpenseByIdAsync(int expenseId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);
        }

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByGroupIdAsync(int groupId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.CoOwnerGroupId == groupId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByVehicleIdAsync(int vehicleId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.VehicleId == vehicleId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByUserIdAsync(int userId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.CoOwnerGroup.GroupMembers.Any(gm => gm.UserId == userId && gm.Status == MemberStatus.Active))
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByCategoryAsync(int categoryId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.ExpenseCategoryId == categoryId)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByStatusAsync(ExpenseStatus status)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.Status == status)
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Include(e => e.ApprovedByUser)
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<Expense> CreateExpenseAsync(Expense expense)
        {
            expense.CreatedAt = DateTime.UtcNow;
            expense.Status = ExpenseStatus.Pending;

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<Expense> UpdateExpenseAsync(Expense expense)
        {
            _context.Expenses.Update(expense);
            await _context.SaveChangesAsync();
            return expense;
        }

        public async Task<bool> DeleteExpenseAsync(int expenseId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null) return false;

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ApproveExpenseAsync(int expenseId, int approvedByUserId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null) return false;

            expense.Status = ExpenseStatus.Approved;
            expense.ApprovedAt = DateTime.UtcNow;
            expense.ApprovedBy = approvedByUserId;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectExpenseAsync(int expenseId, int rejectedByUserId, string reason)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense == null) return false;

            expense.Status = ExpenseStatus.Rejected;
            expense.RejectionReason = reason;
            expense.ApprovedBy = rejectedByUserId;
            await _context.SaveChangesAsync();
            return true;
        }

        // Expense Categories
        public async Task<List<ExpenseCategory>> GetAllExpenseCategoriesAsync()
        {
            return await _context.ExpenseCategories
                .Where(ec => ec.IsActive)
                .OrderBy(ec => ec.CategoryName)
                .ToListAsync();
        }

        public async Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(int categoryId)
        {
            return await _context.ExpenseCategories.FindAsync(categoryId);
        }

        public async Task<ExpenseCategory> CreateExpenseCategoryAsync(ExpenseCategory category)
        {
            category.CreatedAt = DateTime.UtcNow;
            category.IsActive = true;

            _context.ExpenseCategories.Add(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<ExpenseCategory> UpdateExpenseCategoryAsync(ExpenseCategory category)
        {
            _context.ExpenseCategories.Update(category);
            await _context.SaveChangesAsync();
            return category;
        }

        public async Task<bool> DeleteExpenseCategoryAsync(int categoryId)
        {
            var category = await _context.ExpenseCategories.FindAsync(categoryId);
            if (category == null) return false;

            category.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Cost Splitting
        public async Task<Dictionary<int, decimal>> CalculateExpenseSplitAsync(int expenseId)
        {
            var expense = await _context.Expenses
                .Include(e => e.CoOwnerGroup)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null) return new Dictionary<int, decimal>();

            return expense.SplitMethod switch
            {
                SplitMethod.Equal => await CalculateExpenseSplitEqualAsync(expenseId),
                SplitMethod.ByOwnership => await CalculateExpenseSplitByOwnershipAsync(expenseId),
                SplitMethod.ByUsage => await CalculateExpenseSplitByUsageAsync(expenseId),
                _ => await CalculateExpenseSplitEqualAsync(expenseId)
            };
        }

        public async Task<Dictionary<int, decimal>> CalculateExpenseSplitByOwnershipAsync(int expenseId)
        {
            var expense = await _context.Expenses
                .Include(e => e.CoOwnerGroup)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null) return new Dictionary<int, decimal>();

            var ownershipShares = await _context.OwnershipShares
                .Where(os => os.CoOwnerGroupId == expense.CoOwnerGroupId && os.IsActive)
                .ToListAsync();

            var split = new Dictionary<int, decimal>();
            foreach (var share in ownershipShares)
            {
                split[share.UserId] = expense.Amount * (share.Percentage / 100);
            }

            return split;
        }

        public async Task<Dictionary<int, decimal>> CalculateExpenseSplitByUsageAsync(int expenseId)
        {
            var expense = await _context.Expenses
                .Include(e => e.CoOwnerGroup)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null) return new Dictionary<int, decimal>();

            // Get usage data for the group members
            var groupMembers = await _context.GroupMembers
                .Where(gm => gm.CoOwnerGroupId == expense.CoOwnerGroupId && gm.Status == MemberStatus.Active)
                .ToListAsync();

            var usageData = new Dictionary<int, decimal>();
            var totalUsage = 0m;

            foreach (var member in groupMembers)
            {
                var usageHours = await _context.BookingSchedules
                    .Include(bs => bs.Vehicle)
                    .ThenInclude(v => v.CoOwnerGroups)
                    .Where(bs => bs.UserId == member.UserId &&
                               bs.Vehicle.CoOwnerGroups.Any(g => g.CoOwnerGroupId == expense.CoOwnerGroupId) &&
                               bs.Status == BookingStatus.Completed)
                    .SumAsync(bs => (decimal)(bs.EndTime - bs.StartTime).TotalHours);

                usageData[member.UserId] = usageHours;
                totalUsage += usageHours;
            }

            var split = new Dictionary<int, decimal>();
            if (totalUsage > 0)
            {
                foreach (var kvp in usageData)
                {
                    split[kvp.Key] = expense.Amount * (kvp.Value / totalUsage);
                }
            }
            else
            {
                // If no usage data, split equally
                var equalAmount = expense.Amount / groupMembers.Count;
                foreach (var member in groupMembers)
                {
                    split[member.UserId] = equalAmount;
                }
            }

            return split;
        }

        public async Task<Dictionary<int, decimal>> CalculateExpenseSplitEqualAsync(int expenseId)
        {
            var expense = await _context.Expenses
                .Include(e => e.CoOwnerGroup)
                .FirstOrDefaultAsync(e => e.ExpenseId == expenseId);

            if (expense == null) return new Dictionary<int, decimal>();

            var groupMembers = await _context.GroupMembers
                .Where(gm => gm.CoOwnerGroupId == expense.CoOwnerGroupId && gm.Status == MemberStatus.Active)
                .ToListAsync();

            var split = new Dictionary<int, decimal>();
            var equalAmount = expense.Amount / groupMembers.Count;

            foreach (var member in groupMembers)
            {
                split[member.UserId] = equalAmount;
            }

            return split;
        }

        public async Task<bool> ValidateExpenseSplitAsync(int expenseId)
        {
            var split = await CalculateExpenseSplitAsync(expenseId);
            var expense = await _context.Expenses.FindAsync(expenseId);
            
            if (expense == null) return false;

            var totalSplit = split.Values.Sum();
            return Math.Abs(totalSplit - expense.Amount) < 0.01m; // Allow small rounding differences
        }

        // Reports
        public async Task<decimal> GetTotalExpensesByGroupAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Expenses
                .Where(e => e.CoOwnerGroupId == groupId && e.Status == ExpenseStatus.Approved);

            if (startDate.HasValue)
                query = query.Where(e => e.ExpenseDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ExpenseDate <= endDate.Value);

            return await query.SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetTotalExpensesByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var userGroups = await _context.GroupMembers
                .Where(gm => gm.UserId == userId && gm.Status == MemberStatus.Active)
                .Select(gm => gm.CoOwnerGroupId)
                .ToListAsync();

            var query = _context.Expenses
                .Where(e => userGroups.Contains(e.CoOwnerGroupId) && e.Status == ExpenseStatus.Approved);

            if (startDate.HasValue)
                query = query.Where(e => e.ExpenseDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ExpenseDate <= endDate.Value);

            return await query.SumAsync(e => e.Amount);
        }

        public async Task<List<Expense>> GetPendingApprovalExpensesAsync(int groupId)
        {
            return await _context.Expenses
                .Include(e => e.Vehicle)
                .Include(e => e.ExpenseCategory)
                .Include(e => e.CoOwnerGroup)
                .Where(e => e.CoOwnerGroupId == groupId && e.Status == ExpenseStatus.Pending)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Expenses
                .Include(e => e.ExpenseCategory)
                .Where(e => e.CoOwnerGroupId == groupId && e.Status == ExpenseStatus.Approved);

            if (startDate.HasValue)
                query = query.Where(e => e.ExpenseDate >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(e => e.ExpenseDate <= endDate.Value);

            return await query
                .GroupBy(e => e.ExpenseCategory.CategoryName)
                .ToDictionaryAsync(g => g.Key, g => g.Sum(e => e.Amount));
        }
    }
}
