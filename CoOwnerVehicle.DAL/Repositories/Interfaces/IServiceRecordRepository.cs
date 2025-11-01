using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface IServiceRecordRepository
    {
        Task<ServiceRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        Task<List<ServiceRecord>> GetByVehicleIdAsync(int vehicleId, CancellationToken cancellationToken = default);
        Task<List<ServiceRecord>> GetByStatusAsync(ServiceStatus status, CancellationToken cancellationToken = default);
        Task AddAsync(ServiceRecord entity, CancellationToken cancellationToken = default);
        void Update(ServiceRecord entity);
        void Remove(ServiceRecord entity);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}

