using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
	public class ExpenseRepository : IExpenseRepository
	{
		private readonly CoOwnerVehicleDbContext _context;
		private readonly DbSet<Expense> _dbSet;

		public ExpenseRepository(CoOwnerVehicleDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<Expense>();
		}

		public IQueryable<Expense> GetQueryable()
		{
			return _dbSet.AsQueryable();
		}

		public Task<Expense?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return _dbSet.FirstOrDefaultAsync(e => e.ExpenseId == id, cancellationToken);
		}

		public Task AddAsync(Expense entity, CancellationToken cancellationToken = default)
		{
			return _dbSet.AddAsync(entity, cancellationToken).AsTask();
		}

		public void Update(Expense entity)
		{
			_dbSet.Update(entity);
		}

		public void Remove(Expense entity)
		{
			_dbSet.Remove(entity);
		}

		public Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}
	}
}


