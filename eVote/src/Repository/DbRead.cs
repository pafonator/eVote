using eVote.src.Model;
using eVote.src.Model.DTO;
using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public static class DbRead
    {
        public static async Task<List<Vote>> GetAllVotesAsync()
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Votes.ToListAsync();
        }

        public static async Task<List<Vote>> GetVotesOfUserAsync(UserId userId)
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Votes
                .Where(v => v.VoterId == userId)
                .ToListAsync();
        }

        public static async Task<User?> GetUserAsync(string email)
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Users
                .FirstOrDefaultAsync(v => v.Email == email);
        }

        public static async Task<User?> GetUserAsync(UserId userId)
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public static async Task<List<User>> GetAllUsersAsync()
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Users.ToListAsync();
        }

        public static async Task<List<User>> GetAllCandidatesAsync()
        {
            await using var db = EVoteDbContext.GetDb();
            return await db.Users
                .Where(u => u.IsCandidate)
                .ToListAsync();
        }

        
        public static async Task<List<UserVoteInfo>> GetUsersWithVotesAsync()
        {
            await using var db = EVoteDbContext.GetDb();

            var voteCounts = await db.Votes
                .Join(
                    db.Users, // Join to get the voter
                    vote => vote.VoterId,
                    voter => voter.Id,
                    (vote, voter) => new { Vote = vote, Voter = voter }
                )
                .Where(x => !x.Voter.IsCandidate) // Only consider votes from non-candidates
                .GroupBy(v => v.Vote.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CandidateId, g => g.Count);

            return db.Users
                .AsEnumerable()
                .Select(u => new UserVoteInfo
                {
                    Id = u.Id,
                    Email = u.Email,
                    IsCandidate = u.IsCandidate,
                    VoteCount = voteCounts.TryGetValue(u.Id, out var count) ? count : 0
                })
                .OrderByDescending(u => u.VoteCount)
                .ToList();
        }

        public static async Task<int> GetRemainingVotes()
        {
            await using var db = EVoteDbContext.GetDb();

            // Count total registered non-candidate users
            var totalVoters = await db.Users
                .Where(u => !u.IsCandidate)
                .CountAsync();
            int maxVotes = totalVoters*2;

            // Count nb votes of each user
            var userVoteCounts = await db.Votes
                .GroupBy(v => v.VoterId)
                .Select(g => new { VoterId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.VoterId, g => g.Count);


            // Count total votes that are valid
            var validVotes = db.Votes
                .Join(
                    db.Users, // Join to get the voter
                    vote => vote.VoterId,
                    voter => voter.Id,
                    (vote, voter) => new { Vote = vote, Voter = voter }
                )
                .Join(
                    db.Users, // Join to get the candidate
                    temp => temp.Vote.CandidateId,
                    candidate => candidate.Id,
                    (temp, candidate) => new { Vote = temp.Vote, Voter = temp.Voter, Candidate = candidate }
                )
                .Where(x => !x.Voter.IsCandidate)
                .Where(x => x.Candidate.IsCandidate)
                .AsEnumerable()
                // Ignore votes from users with more than 2 votes (this should never happen)
                .Where(x => (userVoteCounts.TryGetValue(x.Vote.VoterId, out int count) ? count : 0) <= 2)
                .Count();

            return maxVotes - validVotes;

        }

    }
}