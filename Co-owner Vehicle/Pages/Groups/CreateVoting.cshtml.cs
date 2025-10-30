using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize(Roles = "Co-owner,Admin")]
    public class CreateVotingModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;

        public CreateVotingModel(CoOwnerVehicleDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public CreateVotingRequest Input { get; set; } = new();

        public List<CoOwnerGroupViewModel> Groups { get; set; } = new();

        public async Task OnGetAsync()
        {
            var currentUserId = this.GetCurrentUserId();

            // Load groups where user is a member
            var groups = await _context.GroupMembers
                .Where(gm => gm.UserId == currentUserId && gm.Status == MemberStatus.Active)
                .Include(gm => gm.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Select(gm => gm.CoOwnerGroup)
                .ToListAsync();

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
            var isMember = await _context.GroupMembers
                .AnyAsync(gm => gm.CoOwnerGroupId == Input.CoOwnerGroupId && 
                               gm.UserId == currentUserId && 
                               gm.Status == MemberStatus.Active);

            if (!isMember)
            {
                ModelState.AddModelError("", "Bạn không phải thành viên của nhóm này");
                await OnGetAsync();
                return Page();
            }

            // Get total active members
            var totalMembers = await _context.GroupMembers
                .CountAsync(gm => gm.CoOwnerGroupId == Input.CoOwnerGroupId && 
                                 gm.Status == MemberStatus.Active);

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

            _context.VotingSessions.Add(votingSession);
            await _context.SaveChangesAsync();

            // Activate the voting session
            votingSession.Status = VotingStatus.Active;
            await _context.SaveChangesAsync();

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

