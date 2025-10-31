using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface ICommonFundService
	{
		Task<CommonFund?> GetFundByIdAsync(int fundId);
		Task<CommonFund?> GetFundByGroupIdAsync(int groupId);
		Task<List<FundTransaction>> GetTransactionsAsync(int fundId);
		Task<FundTransaction> AddTransactionAsync(FundTransaction transaction);
		Task<decimal> GetFundBalanceAsync(int fundId);
	}
}


