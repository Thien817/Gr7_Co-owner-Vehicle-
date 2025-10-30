using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class ContractService : IContractService
	{
		private readonly CoOwnerVehicleDbContext _context;

		public ContractService(CoOwnerVehicleDbContext context)
		{
			_context = context;
		}

		public async Task<EContract?> GetByIdAsync(int id)
		{
			return await _context.EContracts
				.Include(c => c.CoOwnerGroup)
				.FirstOrDefaultAsync(c => c.EContractId == id);
		}

		public async Task<List<EContract>> GetByGroupAsync(int groupId)
		{
			return await _context.EContracts
				.Where(c => c.CoOwnerGroupId == groupId)
				.OrderByDescending(c => c.CreatedAt)
				.ToListAsync();
		}

		public async Task<EContract> CreateAsync(EContract contract)
		{
			contract.CreatedAt = DateTime.UtcNow;
			_context.EContracts.Add(contract);
			await _context.SaveChangesAsync();
			return contract;
		}

		public async Task<EContract> UpdateAsync(EContract contract)
		{
			contract.UpdatedAt = DateTime.UtcNow;
			_context.EContracts.Update(contract);
			await _context.SaveChangesAsync();
			return contract;
		}

		public async Task<bool> DeleteAsync(int id)
		{
			var entity = await _context.EContracts.FindAsync(id);
			if (entity == null) return false;
			_context.EContracts.Remove(entity);
			await _context.SaveChangesAsync();
			return true;
		}
	}
}


