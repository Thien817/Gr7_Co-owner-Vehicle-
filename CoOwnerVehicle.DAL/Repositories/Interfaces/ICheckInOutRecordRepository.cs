using Co_owner_Vehicle.Models;

namespace CoOwnerVehicle.DAL.Repositories.Interfaces
{
    public interface ICheckInOutRepository
    {
        Task AddAsync(CheckInOutRecord entity, CancellationToken cancellationToken = default);
        Task<CheckInOutRecord?> GetLatestAsync(int vehicleId, int userId, CancellationToken cancellationToken = default);
        Task<List<CheckInOutRecord>> GetTodayAsync(int? vehicleId = null, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}


