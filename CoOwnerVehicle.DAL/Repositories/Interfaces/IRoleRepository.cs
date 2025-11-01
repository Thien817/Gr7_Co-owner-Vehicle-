using Co_owner_Vehicle.Models;
using System.Linq.Expressions;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        IQueryable<Role> GetQueryable();
    }
}


