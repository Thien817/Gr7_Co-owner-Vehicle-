using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class CheckInOutService : ICheckInOutService
	{
        private readonly ICheckInOutRepository _checkInOutRepository;

        public CheckInOutService(ICheckInOutRepository checkInOutRepository)
		{
            _checkInOutRepository = checkInOutRepository;
		}

		public async Task<CheckInOutRecord> CreateRecordAsync(int vehicleId, int userId, CheckInOutType type, string? notes, int? mileage, string? condition, string qrData)
		{
			var record = new CheckInOutRecord
			{
				VehicleId = vehicleId,
				UserId = userId,
				Type = type,
				Status = CheckInOutStatus.Completed,
				CheckTime = DateTime.UtcNow,
				Notes = notes,
				Mileage = mileage,
				VehicleCondition = condition,
				QRCodeData = qrData,
				ProcessedBy = userId,
				ProcessedAt = DateTime.UtcNow
			};
            await _checkInOutRepository.AddAsync(record);
            await _checkInOutRepository.SaveChangesAsync();
			return record;
		}

		public async Task<CheckInOutRecord?> GetLatestRecordAsync(int vehicleId, int userId)
		{
            return await _checkInOutRepository.GetLatestAsync(vehicleId, userId);
		}

		public async Task<List<CheckInOutRecord>> GetTodayRecordsAsync(int? vehicleId = null)
		{
            return await _checkInOutRepository.GetTodayAsync(vehicleId);
		}
	}
}


