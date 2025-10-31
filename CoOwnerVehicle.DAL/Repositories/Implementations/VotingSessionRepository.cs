using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class VotingSessionRepository : IVotingSessionRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<VotingSession> _dbSet;

        public VotingSessionRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<VotingSession>();
        }

        public IQueryable<VotingSession> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<VotingSession?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(s => s.CoOwnerGroup)
                .Include(s => s.Votes)
                .FirstOrDefaultAsync(s => s.VotingSessionId == id, cancellationToken);
        }

        public async Task AddAsync(VotingSession entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(VotingSession entity)
        {
            _dbSet.Update(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

