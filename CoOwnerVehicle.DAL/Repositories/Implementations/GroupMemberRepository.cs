using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CoOwnerVehicle.DAL.Repositories.Implementations
{
    public class GroupMemberRepository : IGroupMemberRepository
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly DbSet<GroupMember> _dbSet;

        public GroupMemberRepository(CoOwnerVehicleDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<GroupMember>();
        }

        public IQueryable<GroupMember> GetQueryable()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<GroupMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet
                .Include(gm => gm.User)
                .Include(gm => gm.InvitedByUser)
                .FirstOrDefaultAsync(gm => gm.GroupMemberId == id, cancellationToken);
        }

        public async Task AddAsync(GroupMember entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public void Update(GroupMember entity)
        {
            _dbSet.Update(entity);
        }

        public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}

