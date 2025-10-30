using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.CommonFunds
{
    [Authorize(Roles = "Co-owner,Admin,Staff")]
    public class IndexModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public IndexModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        public List<CommonFundViewModel> Funds { get; set; } = new();
        public FundStatistics Statistics { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            IQueryable<CommonFund> fundsQuery;

            if (isAdmin || isStaff)
            {
                // Admin and Staff can see all funds
                fundsQuery = _context.CommonFunds
                    .Include(f => f.CoOwnerGroup)
                        .ThenInclude(g => g.Vehicle)
                    .Include(f => f.FundTransactions)
                    .Where(f => f.IsActive);
            }
            else
            {
                // Co-owners can only see funds of their groups
                var userGroups = await _context.GroupMembers
                    .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                    .Select(gm => gm.CoOwnerGroupId)
                    .ToListAsync();

                fundsQuery = _context.CommonFunds
                    .Include(f => f.CoOwnerGroup)
                        .ThenInclude(g => g.Vehicle)
                    .Include(f => f.FundTransactions)
                    .Where(f => f.IsActive && userGroups.Contains(f.CoOwnerGroupId));
            }

            var funds = await fundsQuery.ToListAsync();

            Funds = funds.Select(f => new CommonFundViewModel
            {
                CommonFundId = f.CommonFundId,
                CoOwnerGroupId = f.CoOwnerGroupId,
                GroupName = f.CoOwnerGroup?.Vehicle != null 
                    ? $"{f.CoOwnerGroup.Vehicle.Brand} {f.CoOwnerGroup.Vehicle.Model}" 
                    : "Nhóm",
                FundName = f.FundName,
                FundType = f.FundType,
                Description = f.Description,
                CurrentBalance = f.CurrentBalance,
                TargetAmount = f.TargetAmount,
                CreatedAt = f.CreatedAt,
                TransactionCount = f.FundTransactions.Count,
                LastTransactionDate = f.FundTransactions
                    .OrderByDescending(t => t.TransactionDate)
                    .FirstOrDefault()?.TransactionDate
            }).ToList();

            // Calculate statistics
            Statistics.TotalFunds = funds.Count;
            Statistics.TotalBalance = funds.Sum(f => f.CurrentBalance);
            Statistics.TotalTransactions = funds.Sum(f => f.FundTransactions.Count);
            
            // Recent transactions
            var recentTransactions = await _context.FundTransactions
                .Include(t => t.CommonFund)
                .Include(t => t.ProcessedByUser)
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .ToListAsync();

            Statistics.RecentTransactions = recentTransactions.Select(t => new TransactionSummary
            {
                TransactionId = t.FundTransactionId,
                FundName = t.CommonFund?.FundName ?? "Unknown",
                Type = t.TransactionType,
                Amount = t.Amount,
                TransactionDate = t.TransactionDate,
                Description = t.Description
            }).ToList();
        }
    }

    public class CommonFundViewModel
    {
        public int CommonFundId { get; set; }
        public int CoOwnerGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string FundName { get; set; } = string.Empty;
        public FundType FundType { get; set; }
        public string? Description { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal? TargetAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TransactionCount { get; set; }
        public DateTime? LastTransactionDate { get; set; }
    }

    public class FundStatistics
    {
        public int TotalFunds { get; set; }
        public decimal TotalBalance { get; set; }
        public int TotalTransactions { get; set; }
        public List<TransactionSummary> RecentTransactions { get; set; } = new();
    }

    public class TransactionSummary
    {
        public int TransactionId { get; set; }
        public string FundName { get; set; } = string.Empty;
        public TransactionType Type { get; set; }
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? Description { get; set; }
    }
}

