using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.DTO
{
    public class UserWithVotesDto
    {
        public UserId Id { get; set; }
        public string Email { get; set; } = "";
        public bool IsCandidate { get; set; }
        public int VoteCount { get; set; }
    };

    /*
    public static async Task<List<UserWithVotesDto>> GetUsersWithVotesAsync()
    {
        await using var db = EVoteDbContext.GetDb();

        var voteCounts = await db.Votes
            .GroupBy(v => v.CandidateId)
            .Select(g => new { CandidateId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(g => g.CandidateId, g => g.Count);

        return await db.Users
            .Select(u => new UserWithVotesDto
            {
                Id = u.Id,
                Email = u.Email,
                IsCandidate = u.IsCandidate,
                VoteCount = voteCounts.ContainsKey(u.Id) ? voteCounts[u.Id] : 0
            })
            .ToListAsync();
    }*/

}
