using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Contracts
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class IndexModel : PageModel
    {
        private readonly IContractService _contractService;
        private readonly IGroupService _groupService;

        public IndexModel(IContractService contractService, IGroupService groupService)
        {
            _contractService = contractService;
            _groupService = groupService;
        }

        public List<ContractViewModel> Contracts { get; set; } = new();
        public ContractStatistics Statistics { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public ContractStatus? StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? GroupFilter { get; set; }

        public async Task OnGetAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            List<EContract> contracts;

            if (isAdmin || isStaff)
            {
                // Admin and Staff can see all contracts
                contracts = await _contractService.GetAllAsync(groupId: GroupFilter, status: StatusFilter);
            }
            else
            {
                // Co-owners can only see contracts of their groups
                var userGroups = await _groupService.GetGroupsByUserIdAsync(currentUserId);
                var userGroupIds = userGroups.Select(g => g.CoOwnerGroupId).ToList();

                if (GroupFilter.HasValue && !userGroupIds.Contains(GroupFilter.Value))
                {
                    // User doesn't have access to this group
                    Contracts = new List<ContractViewModel>();
                    var contractStats = await _contractService.GetStatisticsAsync();
                    Statistics.TotalContracts = contractStats.total;
                    Statistics.ActiveContracts = contractStats.active;
                    Statistics.PendingContracts = contractStats.pending;
                    Statistics.ExpiringSoon = contractStats.expiringSoon;
                    return;
                }

                var allContracts = await _contractService.GetAllAsync(groupId: GroupFilter, status: StatusFilter);
                contracts = allContracts.Where(c => userGroupIds.Contains(c.CoOwnerGroupId)).ToList();
            }

            Contracts = contracts.Select(c => new ContractViewModel
            {
                EContractId = c.EContractId,
                CoOwnerGroupId = c.CoOwnerGroupId,
                GroupName = c.CoOwnerGroup?.Vehicle != null 
                    ? $"{c.CoOwnerGroup.Vehicle.Brand} {c.CoOwnerGroup.Vehicle.Model}" 
                    : "Nhóm",
                ContractTitle = c.ContractTitle,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                SignedAt = c.SignedAt,
                ExpiresAt = c.ExpiresAt,
                CreatedBy = c.CreatedByUser?.FullName ?? "System",
                SignedBy = c.SignedByUser?.FullName,
                TerminationReason = c.TerminationReason,
                TerminatedAt = c.TerminatedAt,
                HasDocument = !string.IsNullOrEmpty(c.ContractFilePath),
                DaysUntilExpiry = c.ExpiresAt.HasValue 
                    ? (c.ExpiresAt.Value - DateTime.UtcNow).Days 
                    : null
            }).ToList();

            // Calculate statistics
            var stats = await _contractService.GetStatisticsAsync();
            Statistics.TotalContracts = stats.total;
            Statistics.ActiveContracts = stats.active;
            Statistics.PendingContracts = stats.pending;
            Statistics.ExpiringSoon = stats.expiringSoon;
        }
    }

    public class ContractViewModel
    {
        public int EContractId { get; set; }
        public int CoOwnerGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string ContractTitle { get; set; } = string.Empty;
        public ContractStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? SignedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string? SignedBy { get; set; }
        public string? TerminationReason { get; set; }
        public DateTime? TerminatedAt { get; set; }
        public bool HasDocument { get; set; }
        public int? DaysUntilExpiry { get; set; }
    }

    public class ContractStatistics
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int PendingContracts { get; set; }
        public int ExpiringSoon { get; set; }
    }
}

