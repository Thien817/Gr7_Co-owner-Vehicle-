using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class UserRoleRepository : IUserRoleRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<UserRole> _dbSet;

        public UserRoleRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<UserRole>();
        }

        public IQueryable<UserRole> GetQueryable() => _dbSet;
        public async Task AddAsync(UserRole entity, CancellationToken cancellationToken = default) => await _dbSet.AddAsync(entity, cancellationToken);
        public void Update(UserRole entity) => _dbSet.Update(entity);

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


