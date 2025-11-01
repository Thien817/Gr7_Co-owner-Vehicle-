using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class FundTransactionRepository : IFundTransactionRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<FundTransaction> _dbSet;

        public FundTransactionRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<FundTransaction>();
        }

        public IQueryable<FundTransaction> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddAsync(FundTransaction entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

