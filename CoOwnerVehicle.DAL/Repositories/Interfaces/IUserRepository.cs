using Co_owner_Vehicle.Models;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> GetQueryable();
        Task<User?> GetByIdAsync(object id, CancellationToken cancellationToken = default);
        Task AddAsync(User entity, CancellationToken cancellationToken = default);
        void Update(User entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


