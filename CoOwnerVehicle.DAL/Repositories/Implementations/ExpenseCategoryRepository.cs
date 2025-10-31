using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
	public class ExpenseCategoryRepository : IExpenseCategoryRepository
	{
		private readonly CoOwnerVehicleDbContext _context;
		private readonly DbSet<ExpenseCategory> _dbSet;

		public ExpenseCategoryRepository(CoOwnerVehicleDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<ExpenseCategory>();
		}

		public IQueryable<ExpenseCategory> GetQueryable()
		{
			return _dbSet.AsQueryable();
		}

		public Task<ExpenseCategory?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return _dbSet.FirstOrDefaultAsync(e => e.ExpenseCategoryId == id, cancellationToken);
		}

		public Task AddAsync(ExpenseCategory entity, CancellationToken cancellationToken = default)
		{
			return _dbSet.AddAsync(entity, cancellationToken).AsTask();
		}

		public void Update(ExpenseCategory entity)
		{
			_dbSet.Update(entity);
		}

		public Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}
	}
}


