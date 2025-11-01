using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
	public interface IExpenseRepository
	{
		IQueryable<Expense> GetQueryable();
		Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
		Task AddAsync(Expense entity, CancellationToken cancellationToken = default);
		void Update(Expense entity);
		void Remove(Expense entity);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}


