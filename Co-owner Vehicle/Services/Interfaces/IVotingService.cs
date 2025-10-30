using Co_owner_Vehicle.Models;

namespace Co_owner_Vehicle.Services.Interfaces
{
	public interface IVotingService
	{
		Task<VotingSession?> GetSessionByIdAsync(int sessionId);
		Task<List<VotingSession>> GetSessionsByGroupAsync(int groupId);
		Task<VotingSession> CreateSessionAsync(VotingSession session);
		Task<bool> CloseSessionAsync(int sessionId, string? resultNotes = null);
		Task<Vote?> GetUserVoteAsync(int sessionId, int userId);
		Task<Vote> CastOrUpdateVoteAsync(int sessionId, int userId, VoteChoice choice, string? comments);
		Task<(int yes, int no, int abstain)> GetVoteCountsAsync(int sessionId);
	}
}


