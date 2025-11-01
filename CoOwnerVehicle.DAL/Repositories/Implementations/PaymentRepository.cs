using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
	public class PaymentRepository : IPaymentRepository
	{
		private readonly CoOwnerVehicleDbContext _context;
		private readonly DbSet<Payment> _dbSet;

		public PaymentRepository(CoOwnerVehicleDbContext context)
		{
			_context = context;
			_dbSet = _context.Set<Payment>();
		}

		public IQueryable<Payment> GetQueryable()
		{
			return _dbSet.AsQueryable();
		}

		public Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
		{
			return _dbSet.FirstOrDefaultAsync(p => p.PaymentId == id, cancellationToken);
		}

		public Task AddAsync(Payment entity, CancellationToken cancellationToken = default)
		{
			return _dbSet.AddAsync(entity, cancellationToken).AsTask();
		}

		public void Update(Payment entity)
		{
			_dbSet.Update(entity);
		}

		public void Remove(Payment entity)
		{
			_dbSet.Remove(entity);
		}

		public Task SaveChangesAsync(CancellationToken cancellationToken = default)
		{
			return _context.SaveChangesAsync(cancellationToken);
		}
	}
}


