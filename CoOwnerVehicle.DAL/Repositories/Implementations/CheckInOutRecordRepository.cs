using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class CheckInOutRecordRepository : ICheckInOutRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<CheckInOutRecord> _dbSet;

        public CheckInOutRecordRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<CheckInOutRecord>();
        }

        public async Task AddAsync(CheckInOutRecord entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public async Task<CheckInOutRecord?> GetLatestAsync(int vehicleId, int userId, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Where(r => r.VehicleId == vehicleId && r.UserId == userId)
                .OrderByDescending(r => r.CheckTime)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<List<CheckInOutRecord>> GetTodayAsync(int? vehicleId = null, CancellationToken cancellationToken = default)
        {
            var start = DateTime.UtcNow.Date;
            var q = _dbSet.Where(r => r.CheckTime >= start);
            if (vehicleId.HasValue) q = q.Where(r => r.VehicleId == vehicleId.Value);
            return await q.OrderByDescending(r => r.CheckTime).ToListAsync(cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


