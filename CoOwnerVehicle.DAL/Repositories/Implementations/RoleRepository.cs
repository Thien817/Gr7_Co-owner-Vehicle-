using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<Role> _dbSet;

        public RoleRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<Role>();
        }

        public IQueryable<Role> GetQueryable() => _dbSet;
        
    }
}


