using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface ICoOwnerGroupRepository
    {
        IQueryable<CoOwnerGroup> GetQueryable();
        Task<CoOwnerGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(CoOwnerGroup entity, CancellationToken cancellationToken = default);
        void Update(CoOwnerGroup entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

