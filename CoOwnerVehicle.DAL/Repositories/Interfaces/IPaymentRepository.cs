using Co_owner_Vehicle.Models;
using System.Linq;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
	public interface IPaymentRepository
	{
		IQueryable<Payment> GetQueryable();
		Task<Payment?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
		Task AddAsync(Payment entity, CancellationToken cancellationToken = default);
		void Update(Payment entity);
		void Remove(Payment entity);
		Task SaveChangesAsync(CancellationToken cancellationToken = default);
	}
}


