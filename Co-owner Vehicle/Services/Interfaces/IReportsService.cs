namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface IReportsService
	{
		Task<decimal> GetTotalExpensesAsync(int groupId, DateTime? from, DateTime? to);
		Task<decimal> GetTotalPaymentsAsync(int groupId, DateTime? from, DateTime? to);
		Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int groupId, DateTime? from, DateTime? to);
		Task<Dictionary<int, double>> GetUsageHoursByUserAsync(int groupId, DateTime? from, DateTime? to);
		Task<Dictionary<string, decimal>> GetExpensesByVehicleAsync(int groupId, DateTime? from, DateTime? to);
		Task<List<Co_owner_Vehicle.Models.Expense>> GetTopExpensesAsync(int groupId, DateTime? from, DateTime? to, int count = 5);
	}
}


