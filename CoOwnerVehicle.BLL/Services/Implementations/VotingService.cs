using Microsoft.EntityFrameworkCore;
using CoOwnerVehicle.DAL.Repositories.Interfaces;
using Co_owner_Vehicle.Models;
using Co_owner_Vehicle.Services.Interfaces;

namespace Co_owner_Vehicle.Services
{
	public class VotingService : IVotingService
	{
		private readonly IVotingSessionRepository _votingSessionRepository;
		private readonly IVoteRepository _voteRepository;

		public VotingService(IVotingSessionRepository votingSessionRepository, IVoteRepository voteRepository)
		{
			_votingSessionRepository = votingSessionRepository;
			_voteRepository = voteRepository;
		}

		public async Task<VotingSession?> GetSessionByIdAsync(int sessionId)
		{
			return await _votingSessionRepository.GetByIdAsync(sessionId);
		}

		public async Task<List<VotingSession>> GetSessionsByGroupAsync(int groupId)
		{
			return await _votingSessionRepository.GetQueryable()
				.Where(s => s.CoOwnerGroupId == groupId)
				.OrderByDescending(s => s.CreatedAt)
				.ToListAsync();
		}

		public async Task<VotingSession> CreateSessionAsync(VotingSession session)
		{
			session.CreatedAt = DateTime.UtcNow;
			session.Status = VotingStatus.Active;
			await _votingSessionRepository.AddAsync(session);
			await _votingSessionRepository.SaveChangesAsync();
			return session;
		}

		public async Task<bool> CloseSessionAsync(int sessionId, string? resultNotes = null)
		{
			var session = await _votingSessionRepository.GetByIdAsync(sessionId);
			if (session == null) return false;

			var counts = await GetVoteCountsAsync(sessionId);
			session.IsPassed = counts.yes > counts.no; // simple rule
			session.ResultNotes = resultNotes;
			session.Status = VotingStatus.Completed;
			session.EndDate = DateTime.UtcNow;
			session.CompletedAt = DateTime.UtcNow;
			_votingSessionRepository.Update(session);
			await _votingSessionRepository.SaveChangesAsync();
			return true;
		}

		public async Task<Vote?> GetUserVoteAsync(int sessionId, int userId)
		{
			return await _voteRepository.GetQueryable()
				.FirstOrDefaultAsync(v => v.VotingSessionId == sessionId && v.UserId == userId);
		}

		public async Task<Vote> CastOrUpdateVoteAsync(int sessionId, int userId, VoteChoice choice, string? comments)
		{
			var vote = await _voteRepository.GetQueryable()
				.FirstOrDefaultAsync(v => v.VotingSessionId == sessionId && v.UserId == userId);
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
				await _voteRepository.AddAsync(vote);
			}
			else
			{
				vote.Choice = choice;
				vote.Comments = comments;
				vote.VotedAt = DateTime.UtcNow;
				_voteRepository.Update(vote);
			}

			await _voteRepository.SaveChangesAsync();
			return vote;
		}

		public async Task<(int yes, int no, int abstain)> GetVoteCountsAsync(int sessionId)
		{
			var votes = await _voteRepository.GetQueryable()
				.Where(v => v.VotingSessionId == sessionId)
				.ToListAsync();
			return (
				votes.Count(v => v.Choice == VoteChoice.Yes),
				votes.Count(v => v.Choice == VoteChoice.No),
				votes.Count(v => v.Choice == VoteChoice.Abstain)
			);
		}
	}
}


