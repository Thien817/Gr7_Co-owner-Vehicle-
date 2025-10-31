using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IGroupMemberRepository
    {
        IQueryable<GroupMember> GetQueryable();
        Task<GroupMember?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task AddAsync(GroupMember entity, CancellationToken cancellationToken = default);
        void Update(GroupMember entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

