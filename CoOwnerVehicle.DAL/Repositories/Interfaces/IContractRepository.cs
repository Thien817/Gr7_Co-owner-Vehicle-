using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IContractRepository
    {
        Task<EContract?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<EContract>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<List<EContract>> GetByGroupIdAsync(int groupId, CancellationToken cancellationToken = default);
        Task<List<EContract>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default);
        Task AddAsync(EContract entity, CancellationToken cancellationToken = default);
        void Update(EContract entity);
        void Remove(EContract entity);
        Task<int> CountAsync(ContractStatus? status = null, CancellationToken cancellationToken = default);
        Task<int> CountExpiringSoonAsync(DateTime expiryDate, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

