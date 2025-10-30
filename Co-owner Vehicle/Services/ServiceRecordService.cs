using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class ServiceRecordService : IServiceRecordService
	{
		private readonly CoOwnerVehicleDbContext _context;

		public ServiceRecordService(CoOwnerVehicleDbContext context)
		{
			_context = context;
		}

		public async Task<ServiceRecord?> GetByIdAsync(int id)
		{
			return await _context.ServiceRecords
				.Include(r => r.Vehicle)
				.Include(r => r.VehicleService)
				.Include(r => r.PerformedByUser)
				.FirstOrDefaultAsync(r => r.ServiceRecordId == id);
		}

		public async Task<List<ServiceRecord>> GetByVehicleAsync(int vehicleId)
		{
			return await _context.ServiceRecords
				.Where(r => r.VehicleId == vehicleId)
				.OrderByDescending(r => r.ScheduledDate)
				.ToListAsync();
		}

		public async Task<List<ServiceRecord>> GetByStatusAsync(ServiceStatus status)
		{
			return await _context.ServiceRecords
				.Where(r => r.Status == status)
				.OrderByDescending(r => r.ScheduledDate)
				.ToListAsync();
		}

		public async Task<ServiceRecord> CreateAsync(ServiceRecord record)
		{
			record.CreatedAt = DateTime.UtcNow;
			_context.ServiceRecords.Add(record);
			await _context.SaveChangesAsync();
			return record;
		}

		public async Task<ServiceRecord> UpdateAsync(ServiceRecord record)
		{
			_context.ServiceRecords.Update(record);
			await _context.SaveChangesAsync();
			return record;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _context.ServiceRecords.FindAsync(id);
			if (entity == null) return false;
			_context.ServiceRecords.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}


