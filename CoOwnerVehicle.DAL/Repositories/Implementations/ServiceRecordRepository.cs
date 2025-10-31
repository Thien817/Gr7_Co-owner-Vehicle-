using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class ServiceRecordRepository : IServiceRecordRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<ServiceRecord> _dbSet;

        public ServiceRecordRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<ServiceRecord>();
        }

        public async Task<ServiceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Vehicle)
                .Include(r => r.VehicleService)
                .Include(r => r.PerformedByUser)
                .FirstOrDefaultAsync(r => r.ServiceRecordId == id, cancellationToken);
        }

        public async Task<List<ServiceRecord>> GetByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Vehicle)
                .Include(r => r.VehicleService)
                .Include(r => r.PerformedByUser)
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<ServiceRecord>> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(r => r.Vehicle)
                .Include(r => r.VehicleService)
                .Include(r => r.PerformedByUser)
                .Where(r => r.Status == status)
                .OrderByDescending(r => r.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task AddAsync(ServiceRecord entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(ServiceRecord entity)
        {
            _dbSet.Update(entity);
        }

        public void Remove(ServiceRecord entity)
        {
            _dbSet.Remove(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

