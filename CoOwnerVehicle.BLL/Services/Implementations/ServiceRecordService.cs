using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class ServiceRecordService : IServiceRecordService
	{
		private readonly IServiceRecordRepository _serviceRecordRepository;

		public ServiceRecordService(IServiceRecordRepository serviceRecordRepository)
		{
			_serviceRecordRepository = serviceRecordRepository;
		}

		public async Task<ServiceRecord?> GetByIdAsync(int id)
		{
			return await _serviceRecordRepository.GetByIdAsync(id);
		}

		public async Task<List<ServiceRecord>> GetByVehicleAsync(int vehicleId)
		{
			return await _serviceRecordRepository.GetByVehicleIdAsync(vehicleId);
		}

		public async Task<List<ServiceRecord>> GetByStatusAsync(ServiceStatus status)
		{
			return await _serviceRecordRepository.GetByStatusAsync(status);
		}

		public async Task<ServiceRecord> CreateAsync(ServiceRecord record)
		{
			record.CreatedAt = DateTime.UtcNow;
			await _serviceRecordRepository.AddAsync(record);
			await _serviceRecordRepository.SaveChangesAsync();
			return record;
		}

		public async Task<ServiceRecord> UpdateAsync(ServiceRecord record)
		{
			_serviceRecordRepository.Update(record);
			await _serviceRecordRepository.SaveChangesAsync();
			return record;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _serviceRecordRepository.GetByIdAsync(id);
			if (entity == null) return false;
			_serviceRecordRepository.Remove(entity);
			await _serviceRecordRepository.SaveChangesAsync();
			return true;
		}
	}
}


