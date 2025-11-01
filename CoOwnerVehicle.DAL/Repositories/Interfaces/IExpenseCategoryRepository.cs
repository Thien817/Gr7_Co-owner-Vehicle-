using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
	public interface IExpenseCategoryRepository
	{
		IQueryable<ExpenseCategory> GetQueryable();
		Task<ExpenseCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
		Task AddAsync(ExpenseCategory entity, CancellationToken cancellationToken = default);
		void Update(ExpenseCategory entity);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}


