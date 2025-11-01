using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class CommonFundService : ICommonFundService
	{
		private readonly ICommonFundRepository _commonFundRepository;
		private readonly IFundTransactionRepository _fundTransactionRepository;

		public CommonFundService(ICommonFundRepository commonFundRepository, IFundTransactionRepository fundTransactionRepository)
		{
			_commonFundRepository = commonFundRepository;
			_fundTransactionRepository = fundTransactionRepository;
		}

		public async Task<CommonFund?> GetFundByIdAsync(int fundId)
		{
			return await _commonFundRepository.GetByIdAsync(fundId);
		}

		public async Task<CommonFund?> GetFundByGroupIdAsync(int groupId)
		{
			return await _commonFundRepository.GetQueryable()
				.Include(f => f.CoOwnerGroup)
				.FirstOrDefaultAsync(f => f.CoOwnerGroupId == groupId);
		}

		public async Task<List<FundTransaction>> GetTransactionsAsync(int fundId)
		{
			return await _fundTransactionRepository.GetQueryable()
				.Where(t => t.CommonFundId == fundId)
				.OrderByDescending(t => t.TransactionDate)
				.ToListAsync();
		}

		public async Task<FundTransaction> AddTransactionAsync(FundTransaction transaction)
		{
			transaction.TransactionDate = DateTime.UtcNow;
			await _fundTransactionRepository.AddAsync(transaction);
			await _fundTransactionRepository.SaveChangesAsync();
			return transaction;
		}

		public async Task<decimal> GetFundBalanceAsync(int fundId)
		{
			var income = await _fundTransactionRepository.GetQueryable()
				.Where(t => t.CommonFundId == fundId && 
				       (t.TransactionType == TransactionType.Deposit || 
				        t.TransactionType == TransactionType.Interest ||
				        t.TransactionType == TransactionType.Refund))
				.SumAsync(t => t.Amount);
			var expense = await _fundTransactionRepository.GetQueryable()
				.Where(t => t.CommonFundId == fundId && 
				       (t.TransactionType == TransactionType.Withdrawal || 
				        t.TransactionType == TransactionType.Expense))
				.SumAsync(t => t.Amount);
			return income - expense;
		}
	}
}


