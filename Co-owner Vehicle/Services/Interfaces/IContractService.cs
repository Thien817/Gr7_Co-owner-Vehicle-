using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface IContractService
	{
		Task<EContract?> GetByIdAsync(int id);
		Task<List<EContract>> GetByGroupAsync(int groupId);
		Task<EContract> CreateAsync(EContract contract);
		Task<EContract> UpdateAsync(EContract contract);
		Task<bool> DeleteAsync(int id);
	}
}


