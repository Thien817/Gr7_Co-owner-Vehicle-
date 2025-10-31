using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IVotingSessionRepository
    {
        IQueryable<VotingSession> GetQueryable();
        Task<VotingSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(VotingSession entity, CancellationToken cancellationToken = default);
        void Update(VotingSession entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

