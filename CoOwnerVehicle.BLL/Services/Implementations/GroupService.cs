using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
    public class GroupService : IGroupService
    {
        private readonly ICoOwnerGroupRepository _coOwnerGroupRepository;
        private readonly IGroupMemberRepository _groupMemberRepository;
        private readonly IOwnershipShareRepository _ownershipShareRepository;

        public GroupService(
            ICoOwnerGroupRepository coOwnerGroupRepository,
            IGroupMemberRepository groupMemberRepository,
            IOwnershipShareRepository ownershipShareRepository)
        {
            _coOwnerGroupRepository = coOwnerGroupRepository;
            _groupMemberRepository = groupMemberRepository;
            _ownershipShareRepository = ownershipShareRepository;
        }

        public async Task<CoOwnerGroup?> GetGroupByIdAsync(int groupId)
        {
            return await _coOwnerGroupRepository.GetByIdAsync(groupId);
        }

        public async Task<List<CoOwnerGroup>> GetAllGroupsAsync()
        {
            return await _coOwnerGroupRepository.GetQueryable()
                .Include(g => g.Vehicle)
                .Include(g => g.CreatedByUser)
                .Include(g => g.GroupMembers)
                .OrderBy(g => g.GroupName)
                .ToListAsync();
        }

        public async Task<List<CoOwnerGroup>> GetGroupsByVehicleIdAsync(int vehicleId)
        {
            return await _coOwnerGroupRepository.GetQueryable()
                .Include(g => g.Vehicle)
                .Include(g => g.CreatedByUser)
                .Include(g => g.GroupMembers)
                .Where(g => g.VehicleId == vehicleId)
                .ToListAsync();
        }

        public async Task<List<CoOwnerGroup>> GetGroupsByUserIdAsync(int userId)
        {
            return await _coOwnerGroupRepository.GetQueryable()
                .Include(g => g.Vehicle)
                .Include(g => g.CreatedByUser)
                .Include(g => g.GroupMembers)
                .Where(g => g.GroupMembers.Any(gm => gm.UserId == userId && gm.Status == MemberStatus.Active))
                .ToListAsync();
        }

        public async Task<List<CoOwnerGroup>> GetActiveGroupsAsync()
        {
            return await _coOwnerGroupRepository.GetQueryable()
                .Include(g => g.Vehicle)
                .Include(g => g.CreatedByUser)
                .Include(g => g.GroupMembers)
                .Where(g => g.Status == GroupStatus.Active)
                .ToListAsync();
        }

        public async Task<CoOwnerGroup> CreateGroupAsync(CoOwnerGroup group)
        {
            group.CreatedAt = DateTime.UtcNow;
            group.Status = GroupStatus.Pending;

            await _coOwnerGroupRepository.AddAsync(group);
            await _coOwnerGroupRepository.SaveChangesAsync();
            return group;
        }

        public async Task<CoOwnerGroup> UpdateGroupAsync(CoOwnerGroup group)
        {
            group.UpdatedAt = DateTime.UtcNow;
            _coOwnerGroupRepository.Update(group);
            await _coOwnerGroupRepository.SaveChangesAsync();
            return group;
        }

        public async Task<bool> DeleteGroupAsync(int groupId)
        {
            var group = await _coOwnerGroupRepository.GetByIdAsync(groupId);
            if (group == null) return false;

            group.Status = GroupStatus.Dissolved;
            group.DissolvedAt = DateTime.UtcNow;
            group.DissolutionReason = "Deleted by admin";
            _coOwnerGroupRepository.Update(group);
            await _coOwnerGroupRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ActivateGroupAsync(int groupId)
        {
            var group = await _coOwnerGroupRepository.GetByIdAsync(groupId);
            if (group == null) return false;

            group.Status = GroupStatus.Active;
            group.ActivatedAt = DateTime.UtcNow;
            group.UpdatedAt = DateTime.UtcNow;
            _coOwnerGroupRepository.Update(group);
            await _coOwnerGroupRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DissolveGroupAsync(int groupId, string reason)
        {
            var group = await _coOwnerGroupRepository.GetByIdAsync(groupId);
            if (group == null) return false;

            group.Status = GroupStatus.Dissolved;
            group.DissolvedAt = DateTime.UtcNow;
            group.DissolutionReason = reason;
            group.UpdatedAt = DateTime.UtcNow;
            _coOwnerGroupRepository.Update(group);
            await _coOwnerGroupRepository.SaveChangesAsync();
            return true;
        }

        // Group Members
        public async Task<List<GroupMember>> GetGroupMembersAsync(int groupId)
        {
            return await _groupMemberRepository.GetQueryable()
                .Include(gm => gm.User)
                .Include(gm => gm.InvitedByUser)
                .Where(gm => gm.CoOwnerGroupId == groupId)
                .OrderBy(gm => gm.JoinedAt)
                .ToListAsync();
        }

        public async Task<GroupMember?> GetGroupMemberAsync(int groupId, int userId)
        {
            return await _groupMemberRepository.GetQueryable()
                .Include(gm => gm.User)
                .Include(gm => gm.InvitedByUser)
                .FirstOrDefaultAsync(gm => gm.CoOwnerGroupId == groupId && gm.UserId == userId);
        }

        public async Task<bool> AddMemberToGroupAsync(int groupId, int userId, MemberRole role = MemberRole.Member)
        {
            var existingMember = await GetGroupMemberAsync(groupId, userId);
            if (existingMember != null)
            {
                existingMember.Status = MemberStatus.Active;
                existingMember.JoinedAt = DateTime.UtcNow;
                _groupMemberRepository.Update(existingMember);
            }
            else
            {
                var groupMember = new GroupMember
                {
                    CoOwnerGroupId = groupId,
                    UserId = userId,
                    Role = role,
                    Status = MemberStatus.Active,
                    JoinedAt = DateTime.UtcNow
                };
                await _groupMemberRepository.AddAsync(groupMember);
            }

            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveMemberFromGroupAsync(int groupId, int userId)
        {
            var groupMember = await GetGroupMemberAsync(groupId, userId);
            if (groupMember == null) return false;

            groupMember.Status = MemberStatus.Removed;
            groupMember.LeftAt = DateTime.UtcNow;
            groupMember.LeaveReason = "Removed from group";
            _groupMemberRepository.Update(groupMember);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateMemberRoleAsync(int groupId, int userId, MemberRole newRole)
        {
            var groupMember = await GetGroupMemberAsync(groupId, userId);
            if (groupMember == null) return false;

            groupMember.Role = newRole;
            _groupMemberRepository.Update(groupMember);
            await _groupMemberRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserInGroupAsync(int groupId, int userId)
        {
            return await _groupMemberRepository.GetQueryable()
                .AnyAsync(gm => gm.CoOwnerGroupId == groupId && 
                              gm.UserId == userId && 
                              gm.Status == MemberStatus.Active);
        }

        public async Task<bool> IsUserGroupAdminAsync(int groupId, int userId)
        {
            return await _groupMemberRepository.GetQueryable()
                .AnyAsync(gm => gm.CoOwnerGroupId == groupId && 
                              gm.UserId == userId && 
                              gm.Role == MemberRole.Admin &&
                              gm.Status == MemberStatus.Active);
        }

        // Ownership Shares
        public async Task<List<OwnershipShare>> GetOwnershipSharesAsync(int groupId)
        {
            return await _ownershipShareRepository.GetQueryable()
                .Include(os => os.User)
                .Where(os => os.CoOwnerGroupId == groupId && os.IsActive)
                .OrderBy(os => os.Percentage)
                .ToListAsync();
        }

        public async Task<OwnershipShare?> GetUserOwnershipShareAsync(int groupId, int userId)
        {
            return await _ownershipShareRepository.GetQueryable()
                .Include(os => os.User)
                .FirstOrDefaultAsync(os => os.CoOwnerGroupId == groupId && 
                                         os.UserId == userId && 
                                         os.IsActive);
        }

        public async Task<bool> UpdateOwnershipShareAsync(int groupId, int userId, decimal percentage, decimal? investmentAmount)
        {
            var existingShare = await GetUserOwnershipShareAsync(groupId, userId);
            
            if (existingShare != null)
            {
                existingShare.Percentage = percentage;
                existingShare.InvestmentAmount = investmentAmount;
                existingShare.UpdatedAt = DateTime.UtcNow;
                _ownershipShareRepository.Update(existingShare);
            }
            else
            {
                var ownershipShare = new OwnershipShare
                {
                    CoOwnerGroupId = groupId,
                    UserId = userId,
                    Percentage = percentage,
                    InvestmentAmount = investmentAmount,
                    EffectiveFrom = DateTime.UtcNow,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                await _ownershipShareRepository.AddAsync(ownershipShare);
            }

            await _ownershipShareRepository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateOwnershipPercentagesAsync(int groupId)
        {
            var totalPercentage = await GetTotalOwnershipPercentageAsync(groupId);
            return Math.Abs(totalPercentage - 100m) < 0.01m; // Allow small rounding differences
        }

        public async Task<decimal> GetTotalOwnershipPercentageAsync(int groupId)
        {
            return await _ownershipShareRepository.GetQueryable()
                .Where(os => os.CoOwnerGroupId == groupId && os.IsActive)
                .SumAsync(os => os.Percentage);
        }
    }
}
