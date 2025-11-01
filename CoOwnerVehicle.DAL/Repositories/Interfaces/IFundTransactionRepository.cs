using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IFundTransactionRepository
    {
        IQueryable<FundTransaction> GetQueryable();
        Task AddAsync(FundTransaction entity, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

