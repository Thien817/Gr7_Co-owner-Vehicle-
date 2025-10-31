using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IVoteRepository
    {
        IQueryable<Vote> GetQueryable();
        Task AddAsync(Vote entity, CancellationToken cancellationToken = default);
        void Update(Vote entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

