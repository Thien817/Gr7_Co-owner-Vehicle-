using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
    public interface IGroupService
    {
        Task<CoOwnerGroup?> GetGroupByIdAsync(int groupId);
        Task<List<CoOwnerGroup>> GetAllGroupsAsync();
        Task<List<CoOwnerGroup>> GetGroupsByVehicleIdAsync(int vehicleId);
        Task<List<CoOwnerGroup>> GetGroupsByUserIdAsync(int userId);
        Task<List<CoOwnerGroup>> GetActiveGroupsAsync();
        Task<CoOwnerGroup> CreateGroupAsync(CoOwnerGroup group);
        Task<CoOwnerGroup> UpdateGroupAsync(CoOwnerGroup group);
        Task<bool> DeleteGroupAsync(int groupId);
        Task<bool> ActivateGroupAsync(int groupId);
        Task<bool> DissolveGroupAsync(int groupId, string reason);
        
        // Group Members
        Task<List<GroupMember>> GetGroupMembersAsync(int groupId);
        Task<GroupMember?> GetGroupMemberAsync(int groupId, int userId);
        Task<bool> AddMemberToGroupAsync(int groupId, int userId, MemberRole role = MemberRole.Member);
        Task<bool> RemoveMemberFromGroupAsync(int groupId, int userId);
        Task<bool> UpdateMemberRoleAsync(int groupId, int userId, MemberRole newRole);
        Task<bool> IsUserInGroupAsync(int groupId, int userId);
        Task<bool> IsUserGroupAdminAsync(int groupId, int userId);
        
        // Ownership Shares
        Task<List<OwnershipShare>> GetOwnershipSharesAsync(int groupId);
        Task<OwnershipShare?> GetUserOwnershipShareAsync(int groupId, int userId);
        Task<bool> UpdateOwnershipShareAsync(int groupId, int userId, decimal percentage, decimal? investmentAmount);
        Task<bool> ValidateOwnershipPercentagesAsync(int groupId);
        Task<decimal> GetTotalOwnershipPercentageAsync(int groupId);
    }
}
