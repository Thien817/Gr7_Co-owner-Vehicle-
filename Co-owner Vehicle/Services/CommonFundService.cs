using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class CommonFundService : ICommonFundService
	{
		private readonly CoOwnerVehicleDbContext _context;

		public CommonFundService(CoOwnerVehicleDbContext context)
		{
			_context = context;
		}

		public async Task<CommonFund?> GetFundByIdAsync(int fundId)
		{
			return await _context.CommonFunds
				.Include(f => f.CoOwnerGroup)
				.FirstOrDefaultAsync(f => f.CommonFundId == fundId);
		}

		public async Task<CommonFund?> GetFundByGroupIdAsync(int groupId)
		{
			return await _context.CommonFunds
				.Include(f => f.CoOwnerGroup)
				.FirstOrDefaultAsync(f => f.CoOwnerGroupId == groupId);
		}

		public async Task<List<FundTransaction>> GetTransactionsAsync(int fundId)
		{
			return await _context.FundTransactions
				.Where(t => t.CommonFundId == fundId)
				.OrderByDescending(t => t.TransactionDate)
				.ToListAsync();
		}

		public async Task<FundTransaction> AddTransactionAsync(FundTransaction transaction)
		{
			transaction.TransactionDate = DateTime.UtcNow;
			_context.FundTransactions.Add(transaction);
			await _context.SaveChangesAsync();
			return transaction;
		}

		public async Task<decimal> GetFundBalanceAsync(int fundId)
		{
			var income = await _context.FundTransactions
				.Where(t => t.CommonFundId == fundId && 
				       (t.TransactionType == TransactionType.Deposit || 
				        t.TransactionType == TransactionType.Interest ||
				        t.TransactionType == TransactionType.Refund))
				.SumAsync(t => t.Amount);
			var expense = await _context.FundTransactions
				.Where(t => t.CommonFundId == fundId && 
				       (t.TransactionType == TransactionType.Withdrawal || 
				        t.TransactionType == TransactionType.Expense))
				.SumAsync(t => t.Amount);
			return income - expense;
		}
	}
}


