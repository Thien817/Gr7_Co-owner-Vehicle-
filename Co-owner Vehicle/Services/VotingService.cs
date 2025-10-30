using Microsoft.EntityFrameworkCore;
using Co_owner_Vehicle.Data;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class VotingService : IVotingService
	{
		private readonly CoOwnerVehicleDbContext _context;

		public VotingService(CoOwnerVehicleDbContext context)
		{
			_context = context;
		}

		public async Task<VotingSession?> GetSessionByIdAsync(int sessionId)
		{
			return await _context.VotingSessions
				.Include(s => s.CoOwnerGroup)
				.Include(s => s.Votes)
				.FirstOrDefaultAsync(s => s.VotingSessionId == sessionId);
		}

		public async Task<List<VotingSession>> GetSessionsByGroupAsync(int groupId)
		{
			return await _context.VotingSessions
				.Where(s => s.CoOwnerGroupId == groupId)
				.OrderByDescending(s => s.CreatedAt)
				.ToListAsync();
		}

		public async Task<VotingSession> CreateSessionAsync(VotingSession session)
		{
			session.CreatedAt = DateTime.UtcNow;
			session.Status = VotingStatus.Active;
			_context.VotingSessions.Add(session);
			await _context.SaveChangesAsync();
			return session;
		}

		public async Task<bool> CloseSessionAsync(int sessionId, string? resultNotes = null)
		{
			var session = await _context.VotingSessions.FindAsync(sessionId);
			if (session == null) return false;

			var counts = await GetVoteCountsAsync(sessionId);
			session.IsPassed = counts.yes > counts.no; // simple rule
			session.ResultNotes = resultNotes;
			session.Status = VotingStatus.Completed;
			session.EndDate = DateTime.UtcNow;
			session.CompletedAt = DateTime.UtcNow;
			await _context.SaveChangesAsync();
			return true;
		}

		public async Task<Vote?> GetUserVoteAsync(int sessionId, int userId)
		{
			return await _context.Votes.FirstOrDefaultAsync(v => v.VotingSessionId == sessionId && v.UserId == userId);
		}

		public async Task<Vote> CastOrUpdateVoteAsync(int sessionId, int userId, VoteChoice choice, string? comments)
		{
			var vote = await _context.Votes.FirstOrDefaultAsync(v => v.VotingSessionId == sessionId && v.UserId == userId);
			if (vote == null)
			{
				vote = new Vote
				{
					VotingSessionId = sessionId,
					UserId = userId,
					Choice = choice,
					Comments = comments,
					VotedAt = DateTime.UtcNow
				};
				_context.Votes.Add(vote);
			}
			else
			{
				vote.Choice = choice;
				vote.Comments = comments;
				vote.VotedAt = DateTime.UtcNow;
			}

			await _context.SaveChangesAsync();
			return vote;
		}

		public async Task<(int yes, int no, int abstain)> GetVoteCountsAsync(int sessionId)
		{
			var votes = await _context.Votes.Where(v => v.VotingSessionId == sessionId).ToListAsync();
			return (
				votes.Count(v => v.Choice == VoteChoice.Yes),
				votes.Count(v => v.Choice == VoteChoice.No),
				votes.Count(v => v.Choice == VoteChoice.Abstain)
			);
		}
	}
}


