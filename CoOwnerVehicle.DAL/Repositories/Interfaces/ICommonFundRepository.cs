using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface ICommonFundRepository
    {
        IQueryable<CommonFund> GetQueryable();
        Task<CommonFund?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

