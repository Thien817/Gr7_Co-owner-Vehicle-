using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Helpers;
using Co_owner_Vehicle.Services.Interfaces;
using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly IGroupService _groupService;
        private readonly IExpenseService _expenseService;

        public IndexModel(
            IGroupService groupService,
            IExpenseService expenseService)
        {
            _groupService = groupService;
            _expenseService = expenseService;
        }

        public List<GroupViewModel> Groups { get; set; } = new();
        
        [BindProperty(SupportsGet = true)]
        public GroupStatus? StatusFilter { get; set; }

        public class GroupViewModel
        {
            public CoOwnerGroup Group { get; set; } = null!;
            public Vehicle Vehicle { get; set; } = null!;
            public List<GroupMemberViewModel> Members { get; set; } = new();
            public decimal CommonFundBalance { get; set; }
            public decimal MonthlyExpenses { get; set; }
            public bool HasActiveVoting { get; set; }
            public VotingSession? ActiveVoting { get; set; }
        }

        public class GroupMemberViewModel
        {
            public User User { get; set; } = null!;
            public MemberRole Role { get; set; }
            public decimal OwnershipPercentage { get; set; }
            public decimal InvestmentAmount { get; set; }
        }

        public async Task OnGetAsync()
        {
            var userId = this.GetCurrentUserId();
            var isAdmin = this.IsAdmin();
            var isStaff = this.IsStaff();

            List<CoOwnerGroup> groups;

            if (isAdmin || isStaff)
            {
                // Admin/Staff: See all groups
                groups = await _groupService.GetAllGroupsAsync();
            }
            else
            {
                // Co-owner: Only see groups they belong to
                groups = await _groupService.GetGroupsByUserIdAsync(userId);
            }

            // Apply status filter
            if (StatusFilter.HasValue)
            {
                groups = groups.Where(g => g.Status == StatusFilter.Value).ToList();
            }

            // Build view models
            foreach (var group in groups)
            {
                // Get group members with ownership info
                var members = await _groupService.GetGroupMembersAsync(group.CoOwnerGroupId);
                var ownershipShares = await _groupService.GetOwnershipSharesAsync(group.CoOwnerGroupId);
                
                var memberViewModels = new List<GroupMemberViewModel>();
                foreach (var member in members)
                {
                    var share = ownershipShares.FirstOrDefault(s => s.UserId == member.UserId);
                    memberViewModels.Add(new GroupMemberViewModel
                    {
                        User = member.User,
                        Role = member.Role,
                        OwnershipPercentage = share?.Percentage ?? 0,
                        InvestmentAmount = share?.InvestmentAmount ?? 0
                    });
                }

                // Get common fund balance (simplified for now)
                var commonFundBalance = group.CommonFunds?.Sum(f => f.CurrentBalance) ?? 0;

                // Get monthly expenses
                var currentMonth = DateTime.UtcNow.Date;
                var monthStart = new DateTime(currentMonth.Year, currentMonth.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);
                
                var monthlyExpenses = await _expenseService.GetExpensesByGroupIdAsync(group.CoOwnerGroupId);
                var monthlyTotal = monthlyExpenses
                    .Where(e => e.CreatedAt >= monthStart && e.CreatedAt <= monthEnd && e.Status == ExpenseStatus.Approved)
                    .Sum(e => e.Amount);

                // Check for active voting
                var activeVoting = group.VotingSessions
                    .Where(v => v.Status == VotingStatus.Active && v.EndDate > DateTime.UtcNow)
                    .OrderByDescending(v => v.CreatedAt)
                    .FirstOrDefault();

                Groups.Add(new GroupViewModel
                {
                    Group = group,
                    Vehicle = group.Vehicle,
                    Members = memberViewModels.OrderByDescending(m => m.Role).ThenByDescending(m => m.OwnershipPercentage).ToList(),
                    CommonFundBalance = commonFundBalance,
                    MonthlyExpenses = monthlyTotal,
                    HasActiveVoting = activeVoting != null,
                    ActiveVoting = activeVoting
                });
            }

            // Sort by group status and creation date
            Groups = Groups.OrderBy(g => g.Group.Status).ThenByDescending(g => g.Group.CreatedAt).ToList();
        }
    }
}

