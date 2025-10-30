using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.Contracts
{
    [Authorize(Roles = "Admin,Staff,Co-owner")]
    public class IndexModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public IndexModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
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

            IQueryable<EContract> contractsQuery;

            if (isAdmin || isStaff)
            {
                // Admin and Staff can see all contracts
                contractsQuery = _context.EContracts
                    .Include(c => c.CoOwnerGroup)
                        .ThenInclude(g => g.Vehicle)
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.SignedByUser);
            }
            else
            {
                // Co-owners can only see contracts of their groups
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                contractsQuery = _context.EContracts
                    .Include(c => c.CoOwnerGroup)
                        .ThenInclude(g => g.Vehicle)
                    .Include(c => c.CreatedByUser)
                    .Include(c => c.SignedByUser)
                    .Where(c => userGroups.Contains(c.CoOwnerGroupId));
            }

            // Apply filters
            if (StatusFilter.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.Status == StatusFilter.Value);
            }

            if (GroupFilter.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.CoOwnerGroupId == GroupFilter.Value);
            }

            var contracts = await contractsQuery
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

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
            Statistics.TotalContracts = await _context.EContracts.CountAsync();
            Statistics.ActiveContracts = await _context.EContracts.CountAsync(c => c.Status == ContractStatus.Active);
            Statistics.PendingContracts = await _context.EContracts.CountAsync(c => c.Status == ContractStatus.Pending);
            Statistics.ExpiringSoon = await _context.EContracts.CountAsync(c => 
                c.Status == ContractStatus.Active && 
                c.ExpiresAt.HasValue && 
                c.ExpiresAt.Value <= DateTime.UtcNow.AddDays(30));
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

