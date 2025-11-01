using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class ContractService : IContractService
	{
		private readonly IContractRepository _contractRepository;

		public ContractService(IContractRepository contractRepository)
		{
			_contractRepository = contractRepository;
		}

		public async Task<EContract?> GetByIdAsync(int id)
		{
			return await _contractRepository.GetByIdAsync(id);
		}

		public async Task<List<EContract>> GetByGroupAsync(int groupId)
		{
			return await _contractRepository.GetByGroupIdAsync(groupId);
		}

		public async Task<List<EContract>> GetAllAsync(int? groupId = null, ContractStatus? status = null)
		{
			if (groupId.HasValue)
			{
				var contracts = await _contractRepository.GetByGroupIdAsync(groupId.Value);
				if (status.HasValue)
					return contracts.Where(c => c.Status == status.Value).ToList();
				return contracts;
			}

			if (status.HasValue)
			{
				return await _contractRepository.GetByStatusAsync(status.Value);
			}

			return await _contractRepository.GetAllAsync();
		}

		public async Task<(int total, int active, int pending, int expiringSoon)> GetStatisticsAsync()
		{
			var total = await _contractRepository.CountAsync();
			var active = await _contractRepository.CountAsync(ContractStatus.Active);
			var pending = await _contractRepository.CountAsync(ContractStatus.Pending);
			var expiringSoon = await _contractRepository.CountExpiringSoonAsync(DateTime.UtcNow.AddDays(30));
			
			return (total, active, pending, expiringSoon);
		}

		public async Task<EContract> CreateAsync(EContract contract)
		{
			contract.CreatedAt = DateTime.UtcNow;
			await _contractRepository.AddAsync(contract);
			await _contractRepository.SaveChangesAsync();
			return contract;
		}

		public async Task<EContract> UpdateAsync(EContract contract)
		{
			contract.UpdatedAt = DateTime.UtcNow;
			_contractRepository.Update(contract);
			await _contractRepository.SaveChangesAsync();
			return contract;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _contractRepository.GetByIdAsync(id);
			if (entity == null) return false;
			_contractRepository.Remove(entity);
			await _contractRepository.SaveChangesAsync();
			return true;
		}
	}
}


