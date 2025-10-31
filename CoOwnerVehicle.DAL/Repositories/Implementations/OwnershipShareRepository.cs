using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class OwnershipShareRepository : IOwnershipShareRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<OwnershipShare> _dbSet;

        public OwnershipShareRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<OwnershipShare>();
        }

        public IQueryable<OwnershipShare> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddAsync(OwnershipShare entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(OwnershipShare entity)
        {
            _dbSet.Update(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

