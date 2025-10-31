using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<User> _dbSet;

        public UserRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<User>();
        }

        public IQueryable<User> GetQueryable() => _dbSet;

        public async Task<User?> GetByIdAsync(object id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync([id], cancellationToken);
        }

        public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(User entity)
        {
            _dbSet.Update(entity);
        }

        

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}


