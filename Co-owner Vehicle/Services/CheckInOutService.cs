using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class CheckInOutService : ICheckInOutService
	{
		private readonly CoOwnerVehicleDbContext _context;

		public CheckInOutService(CoOwnerVehicleDbContext context)
		{
			_context = context;
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
			_context.CheckInOutRecords.Add(record);
			await _context.SaveChangesAsync();
			return record;
		}

		public async Task<CheckInOutRecord?> GetLatestRecordAsync(int vehicleId, int userId)
		{
			return await _context.CheckInOutRecords
				.Where(r => r.VehicleId == vehicleId && r.UserId == userId)
				.OrderByDescending(r => r.CheckTime)
				.FirstOrDefaultAsync();
		}

		public async Task<List<CheckInOutRecord>> GetTodayRecordsAsync(int? vehicleId = null)
		{
			var start = DateTime.UtcNow.Date;
			var q = _context.CheckInOutRecords.Where(r => r.CheckTime >= start);
			if (vehicleId.HasValue) q = q.Where(r => r.VehicleId == vehicleId.Value);
			return await q.OrderByDescending(r => r.CheckTime).ToListAsync();
		}
	}
}


