using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class CommonFundRepository : ICommonFundRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<CommonFund> _dbSet;

        public CommonFundRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<CommonFund>();
        }

        public IQueryable<CommonFund> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<CommonFund?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(f => f.CoOwnerGroup)
                .FirstOrDefaultAsync(f => f.CommonFundId == id, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

