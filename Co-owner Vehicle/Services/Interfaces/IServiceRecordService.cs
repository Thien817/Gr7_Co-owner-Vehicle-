using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface IServiceRecordService
	{
		Task<ServiceRecord?> GetByIdAsync(int id);
		Task<List<ServiceRecord>> GetByVehicleAsync(int vehicleId);
		Task<List<ServiceRecord>> GetByStatusAsync(ServiceStatus status);
		Task<ServiceRecord> CreateAsync(ServiceRecord record);
		Task<ServiceRecord> UpdateAsync(ServiceRecord record);
		Task<bool> DeleteAsync(int id);
	}
}


