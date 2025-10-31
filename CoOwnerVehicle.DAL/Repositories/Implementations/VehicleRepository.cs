using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class VehicleRepository : IVehicleRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<Vehicle> _dbSet;

        public VehicleRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Vehicle>();
        }

        public IQueryable<Vehicle> GetQueryable() => _dbSet;
        public async Task<Vehicle?> GetByIdAsync(object id, CancellationToken cancellationToken = default) => await _dbSet.FindAsync([id], cancellationToken);
        public async Task AddAsync(Vehicle entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);
        public void Update(Vehicle entity) => _dbSet.Update(entity);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


