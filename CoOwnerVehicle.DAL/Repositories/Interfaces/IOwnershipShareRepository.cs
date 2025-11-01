using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IOwnershipShareRepository
    {
        IQueryable<OwnershipShare> GetQueryable();
        Task AddAsync(OwnershipShare entity, CancellationToken cancellationToken = default);
        void Update(OwnershipShare entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

