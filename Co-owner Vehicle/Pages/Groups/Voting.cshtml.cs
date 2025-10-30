using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Helpers;
using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Pages.Groups
{
    [Authorize(Roles = "Co-owner,Admin,Staff")]
    public class VotingModel : PageModel
    {
        private readonly CoOwnerVehicleDbContext _context;
        private readonly IVotingService _votingService;

        public VotingModel(CoOwnerVehicleDbContext context, IVotingService votingService)
        {
            _context = context;
            _votingService = votingService;
        }

        public VotingSessionViewModel? VotingSession { get; set; }
        public bool HasVoted { get; set; }
        public VoteChoice? UserVote { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
                return NotFound();

            var currentUserId = this.GetCurrentUserId();

            var votingSession = await _context.VotingSessions
                .Include(v => v.CoOwnerGroup)
                    .ThenInclude(g => g.Vehicle)
                .Include(v => v.CreatedByUser)
                .Include(v => v.Votes)
                    .ThenInclude(v => v.User)
                .FirstOrDefaultAsync(v => v.VotingSessionId == id.Value);

            if (votingSession == null)
                return NotFound();

            // Check if user has voted
            var userVote = votingSession.Votes.FirstOrDefault(v => v.UserId == currentUserId);
            HasVoted = userVote != null;
            UserVote = userVote?.Choice;

            // Calculate vote counts
            var yesCount = votingSession.Votes.Count(v => v.Choice == VoteChoice.Yes);
            var noCount = votingSession.Votes.Count(v => v.Choice == VoteChoice.No);
            var abstainCount = votingSession.Votes.Count(v => v.Choice == VoteChoice.Abstain);

            // Get active members count
            var totalMembers = await _context.GroupMembers
                .CountAsync(gm => gm.CoOwnerGroupId == votingSession.CoOwnerGroupId && 
                                 gm.Status == MemberStatus.Active);

            VotingSession = new VotingSessionViewModel
            {
                VotingSessionId = votingSession.VotingSessionId,
                CoOwnerGroupId = votingSession.CoOwnerGroupId,
                CoOwnerGroup = votingSession.CoOwnerGroup,
                GroupName = votingSession.CoOwnerGroup?.GroupName ?? "Nhóm",
                VehicleInfo = votingSession.CoOwnerGroup?.Vehicle != null 
                    ? $"{votingSession.CoOwnerGroup.Vehicle.LicensePlate}" 
                    : "",
                Title = votingSession.Title,
                Description = votingSession.Description,
                DecisionType = votingSession.DecisionType,
                Status = votingSession.Status,
                CreatedAt = votingSession.CreatedAt,
                CreatedBy = votingSession.CreatedByUser?.FullName ?? "System",
                CreatedByUser = votingSession.CreatedByUser,
                StartDate = votingSession.StartDate,
                EndDate = votingSession.EndDate,
                RequiredVotes = votingSession.RequiredVotes ?? totalMembers,
                YesVotes = yesCount,
                NoVotes = noCount,
                AbstainVotes = abstainCount,
                IsPassed = votingSession.IsPassed,
                ResultNotes = votingSession.ResultNotes,
                Votes = votingSession.Votes.Select(v => new VoteViewModel
                {
                    VoteId = v.VoteId,
                    UserId = v.UserId,
                    UserName = v.User?.FullName ?? "Unknown",
                    User = v.User,
                    UserInitials = GetInitials(v.User?.FullName ?? "U"),
                    Choice = v.Choice,
                    Comments = v.Comments,
                    VotedAt = v.VotedAt
                }).ToList(),
                TotalMembers = totalMembers
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, VoteChoice choice, string? comments)
        {
            if (!ModelState.IsValid)
                return Page();

            var currentUserId = this.GetCurrentUserId();

            // Check if voting session exists and is active
            var votingSession = await _context.VotingSessions
                .FirstOrDefaultAsync(v => v.VotingSessionId == id);

            if (votingSession == null)
                return NotFound();

            if (votingSession.Status != VotingStatus.Active)
            {
                ModelState.AddModelError("", "Voting session is not active");
                return Page();
            }

            // Check if user has already voted
            await _votingService.CastOrUpdateVoteAsync(id, currentUserId, choice, comments);

            return RedirectToPage(new { id });
        }

        private string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name))
                return "U";
            
            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
                return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
            
            return parts[0].Substring(0, Math.Min(2, parts[0].Length)).ToUpper();
        }
    }

    public class VotingSessionViewModel
    {
        public int VotingSessionId { get; set; }
        public int CoOwnerGroupId { get; set; }
        public CoOwnerGroup? CoOwnerGroup { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string VehicleInfo { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DecisionType DecisionType { get; set; }
        public VotingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public User? CreatedByUser { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int RequiredVotes { get; set; }
        public int YesVotes { get; set; }
        public int NoVotes { get; set; }
        public int AbstainVotes { get; set; }
        public bool IsPassed { get; set; }
        public string? ResultNotes { get; set; }
        public List<VoteViewModel> Votes { get; set; } = new();
        public int TotalMembers { get; set; }
    }

    public class VoteViewModel
    {
        public int VoteId { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserInitials { get; set; } = string.Empty;
        public VoteChoice Choice { get; set; }
        public string? Comments { get; set; }
        public DateTime VotedAt { get; set; }
    }
}

