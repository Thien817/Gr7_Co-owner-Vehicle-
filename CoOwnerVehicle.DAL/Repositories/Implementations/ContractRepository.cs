using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class ContractRepository : IContractRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<EContract> _dbSet;

        public ContractRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<EContract>();
        }

        public async Task<EContract?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Include(c => c.CreatedByUser)
                .Include(c => c.SignedByUser)
                .FirstOrDefaultAsync(c => c.EContractId == id, cancellationToken);
        }

        public async Task<List<EContract>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Include(c => c.CreatedByUser)
                .Include(c => c.SignedByUser)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<EContract>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Include(c => c.CreatedByUser)
                .Include(c => c.SignedByUser)
                .Where(c => c.CoOwnerGroupId == groupId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<EContract>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(c => c.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Include(c => c.CreatedByUser)
                .Include(c => c.SignedByUser)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(EContract entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(EContract entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(EContract entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> CountAsync(ContractStatus? status = null, CancellationToken cancellationToken = default)
        {
            if (status.HasValue)
                return await _dbSet.CountAsync(c => c.Status == status.Value, cancellationToken);
            return await _dbSet.CountAsync(cancellationToken);
        }

        public async Task<int> CountExpiringSoonAsync(DateTime expiryDate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.CountAsync(c => 
                c.Status == ContractStatus.Active && 
                c.ExpiresAt.HasValue && 
                c.ExpiresAt.Value <= expiryDate, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

