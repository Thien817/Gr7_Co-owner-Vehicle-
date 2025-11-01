using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class ReportsService : IReportsService
	{
		private readonly IExpenseRepository _expenseRepository;
		private readonly IPaymentRepository _paymentRepository;
		private readonly IBookingRepository _bookingRepository;

		public ReportsService(IExpenseRepository expenseRepository, IPaymentRepository paymentRepository, IBookingRepository bookingRepository)
		{
			_expenseRepository = expenseRepository;
			_paymentRepository = paymentRepository;
			_bookingRepository = bookingRepository;
		}

		public async Task<decimal> GetTotalExpensesAsync(int groupId, DateTime? from, DateTime? to)
		{
			var q = _expenseRepository.GetQueryable().Where(e => e.CoOwnerGroupId == groupId);
			if (from.HasValue) q = q.Where(e => e.ExpenseDate >= from.Value);
			if (to.HasValue) q = q.Where(e => e.ExpenseDate <= to.Value);
			return await q.SumAsync(e => e.Amount);
		}

		public async Task<decimal> GetTotalPaymentsAsync(int groupId, DateTime? from, DateTime? to)
		{
			var q = _paymentRepository.GetQueryable().Include(p => p.Expense)
				.Where(p => p.Expense!.CoOwnerGroupId == groupId && p.Status == Models.PaymentStatus.Completed);
			if (from.HasValue) q = q.Where(p => p.PaymentDate >= from.Value);
			if (to.HasValue) q = q.Where(p => p.PaymentDate <= to.Value);
			return await q.SumAsync(p => p.Amount);
		}

		public async Task<Dictionary<string, decimal>> GetExpensesByCategoryAsync(int groupId, DateTime? from, DateTime? to)
		{
			var q = _expenseRepository.GetQueryable().Include(e => e.ExpenseCategory)
				.Where(e => e.CoOwnerGroupId == groupId);
			if (from.HasValue) q = q.Where(e => e.ExpenseDate >= from.Value);
			if (to.HasValue) q = q.Where(e => e.ExpenseDate <= to.Value);
			return await q.GroupBy(e => e.ExpenseCategory.CategoryName)
				.ToDictionaryAsync(g => g.Key, g => g.Sum(e => e.Amount));
		}

		public async Task<Dictionary<int, double>> GetUsageHoursByUserAsync(int groupId, DateTime? from, DateTime? to)
		{
			var q = _bookingRepository.GetQueryable()
				.Include(bs => bs.Vehicle)
				.ThenInclude(v => v.CoOwnerGroups)
				.Where(bs => bs.Vehicle.CoOwnerGroups.Any(g => g.CoOwnerGroupId == groupId) && bs.Status == Models.BookingStatus.Completed);
			if (from.HasValue) q = q.Where(bs => bs.StartTime >= from.Value);
			if (to.HasValue) q = q.Where(bs => bs.EndTime <= to.Value);
			return await q.GroupBy(bs => bs.UserId)
				.ToDictionaryAsync(g => g.Key, g => g.Sum(bs => (bs.EndTime - bs.StartTime).TotalHours));
		}

		public async Task<Dictionary<string, decimal>> GetExpensesByVehicleAsync(int groupId, DateTime? from, DateTime? to)
		{
			var q = _expenseRepository.GetQueryable()
				.Include(e => e.Vehicle)
				.Where(e => e.CoOwnerGroupId == groupId);
			if (from.HasValue) q = q.Where(e => e.ExpenseDate >= from.Value);
			if (to.HasValue) q = q.Where(e => e.ExpenseDate <= to.Value);
			return await q
				.GroupBy(e => e.Vehicle != null ? (e.Vehicle.Brand + " " + e.Vehicle.Model) : "Không rõ xe")
				.ToDictionaryAsync(g => g.Key, g => g.Sum(e => e.Amount));
		}

		public async Task<List<Models.Expense>> GetTopExpensesAsync(int groupId, DateTime? from, DateTime? to, int count = 5)
		{
			var q = _expenseRepository.GetQueryable()
				.Include(e => e.Vehicle)
				.Include(e => e.ExpenseCategory)
				.Where(e => e.CoOwnerGroupId == groupId);
			if (from.HasValue) q = q.Where(e => e.ExpenseDate >= from.Value);
			if (to.HasValue) q = q.Where(e => e.ExpenseDate <= to.Value);
			return await q.OrderByDescending(e => e.Amount).Take(count).ToListAsync();
		}
	}
}


