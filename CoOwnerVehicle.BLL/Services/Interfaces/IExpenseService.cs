using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<Expense?> GetExpenseByIdAsync(int expenseId);
        Task<List<Expense>> GetAllExpensesAsync();
        Task<List<Expense>> GetExpensesByGroupIdAsync(int groupId);
        Task<List<Expense>> GetExpensesByVehicleIdAsync(int vehicleId);
        Task<List<Expense>> GetExpensesByUserIdAsync(int userId);
        Task<List<Expense>> GetExpensesByCategoryAsync(int categoryId);
        Task<List<Expense>> GetExpensesByStatusAsync(ExpenseStatus status);
        Task<List<Expense>> GetExpensesByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<Expense> CreateExpenseAsync(Expense expense);
        Task<Expense> UpdateExpenseAsync(Expense expense);
        Task<bool> DeleteExpenseAsync(int expenseId);
        Task<bool> ApproveExpenseAsync(int expenseId, int approvedByUserId);
        Task<bool> RejectExpenseAsync(int expenseId, int rejectedByUserId, string reason);
        
        // Expense Categories
        Task<List<ExpenseCategory>> GetAllExpenseCategoriesAsync();
        Task<ExpenseCategory?> GetExpenseCategoryByIdAsync(int categoryId);
        Task<ExpenseCategory> CreateExpenseCategoryAsync(ExpenseCategory category);
        Task<ExpenseCategory> UpdateExpenseCategoryAsync(ExpenseCategory category);
        Task<bool> DeleteExpenseCategoryAsync(int categoryId);
        
        // Cost Splitting
        Task<Dictionary<int, decimal>> CalculateExpenseSplitAsync(int expenseId);
        Task<Dictionary<int, decimal>> CalculateExpenseSplitByOwnershipAsync(int expenseId);
        Task<Dictionary<int, decimal>> CalculateExpenseSplitByUsageAsync(int expenseId);
        Task<Dictionary<int, decimal>> CalculateExpenseSplitEqualAsync(int expenseId);
        Task<bool> ValidateExpenseSplitAsync(int expenseId);
        
        // Reports
        Task<decimal> GetTotalExpensesByGroupAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null);
        Task<decimal> GetTotalExpensesByUserAsync(int userId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<Expense>> GetPendingApprovalExpensesAsync(int groupId);
        Task<List<Expense>> GetPendingApprovalExpensesAsync(); // All pending expenses
        Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int groupId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
