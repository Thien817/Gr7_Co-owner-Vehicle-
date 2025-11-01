using Co_owner_Vehicle.Models;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        IQueryable<UserRole> GetQueryable();
        Task AddAsync(UserRole entity, CancellationToken cancellationToken = default);
        void Update(UserRole entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


