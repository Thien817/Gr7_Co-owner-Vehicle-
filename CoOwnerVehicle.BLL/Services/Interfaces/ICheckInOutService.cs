using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface ICheckInOutService
	{
		Task<CheckInOutRecord> CreateRecordAsync(int vehicleId, int userId, CheckInOutType type, string? notes, int? mileage, string? condition, string qrData);
		Task<CheckInOutRecord?> GetLatestRecordAsync(int vehicleId, int userId);
		Task<List<CheckInOutRecord>> GetTodayRecordsAsync(int? vehicleId = null);
	}
}


