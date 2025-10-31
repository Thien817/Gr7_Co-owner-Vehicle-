using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class VoteRepository : IVoteRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<Vote> _dbSet;

        public VoteRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Vote>();
        }

        public IQueryable<Vote> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddAsync(Vote entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(Vote entity)
        {
            _dbSet.Update(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

