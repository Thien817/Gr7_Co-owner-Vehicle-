using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize(Roles = "Co-owner,Admin")]
    public class CreateVotingModel : PageModel
    {
        private readonly IVotingService _votingService;
        private readonly IGroupService _groupService;

        public CreateVotingModel(IVotingService votingService, IGroupService groupService)
        {
            _votingService = votingService;
            _groupService = groupService;
        }

        [BindProperty]
        public CreateVotingRequest Input { get; set; } = new();

        public List<CoOwnerGroupViewModel> Groups { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = this.GetCurrentUserId();

            // Load groups where user is a member
            var groups = await _groupService.GetGroupsByUserIdAsync(currentUserId);

            Groups = groups.Select(g => new CoOwnerGroupViewModel
            {
                CoOwnerGroupId = g.CoOwnerGroupId,
                GroupName = g.Vehicle != null 
                    ? $"{g.Vehicle.Brand} {g.Vehicle.Model}" 
                    : "Nhóm",
                VehicleId = g.VehicleId
            }).ToList();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await OnGetAsync();
                return Page();
            }

            var currentUserId = this.GetCurrentUserId();

            // Verify user is member of the group
            var isMember = await _groupService.IsUserInGroupAsync(Input.CoOwnerGroupId, currentUserId);

            if (!isMember)
            {
                ModelState.AddModelError("", "Bạn không phải thành viên của nhóm này");
                await OnGetAsync();
                return Page();
            }

            // Get total active members
            var members = await _groupService.GetGroupMembersAsync(Input.CoOwnerGroupId);
            var totalMembers = members.Count(m => m.Status == MemberStatus.Active);

            var votingSession = new VotingSession
            {
                CoOwnerGroupId = Input.CoOwnerGroupId,
                Title = Input.Title,
                Description = Input.Description,
                DecisionType = Input.DecisionType,
                Status = VotingStatus.Draft,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow,
                StartDate = Input.StartDate,
                EndDate = Input.EndDate,
                RequiredVotes = Input.RequiredVotes > 0 ? Input.RequiredVotes : totalMembers
            };

            // Activate on create
            votingSession.Status = VotingStatus.Active;
            await _votingService.CreateSessionAsync(votingSession);

            return RedirectToPage("/Groups/Voting", new { id = votingSession.VotingSessionId });
        }
    }

    public class CreateVotingRequest
    {
        public int CoOwnerGroupId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DecisionType DecisionType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RequiredVotes { get; set; }
    }

    public class CoOwnerGroupViewModel
    {
        public int CoOwnerGroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int VehicleId { get; set; }
    }
}

