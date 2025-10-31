using Co_owner_Vehicle.Models;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IVehicleRepository
    {
        IQueryable<Vehicle> GetQueryable();
        Task<Vehicle?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task AddAsync(Vehicle entity, CancellationToken cancellationToken = default);
        void Update(Vehicle entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


