using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class CoOwnerGroupRepository : ICoOwnerGroupRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<CoOwnerGroup> _dbSet;

        public CoOwnerGroupRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<CoOwnerGroup>();
        }

        public IQueryable<CoOwnerGroup> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<CoOwnerGroup?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(g => g.Vehicle)
                .Include(g => g.CreatedByUser)
                .Include(g => g.GroupMembers)
                    .ThenInclude(gm => gm.User)
                .Include(g => g.OwnershipShares)
                    .ThenInclude(os => os.User)
                .Include(g => g.VotingSessions)
                    .ThenInclude(vs => vs.Votes)
                .FirstOrDefaultAsync(g => g.CoOwnerGroupId == id, cancellationToken);
        }

        public async Task AddAsync(CoOwnerGroup entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(CoOwnerGroup entity)
        {
            _dbSet.Update(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

