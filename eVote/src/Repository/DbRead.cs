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
                .FirstOrDefaultAsync(u => u.Id == userId); //TODO
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

        
        public static async Task<List<TableRow>> GetUsersWithVotesAsync()
        {
            await using var db = EVoteDbContext.GetDb();

            var voteCounts = await db.Votes
                .GroupBy(v => v.CandidateId)
                .Select(g => new { CandidateId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(g => g.CandidateId, g => g.Count);

            return await db.Users
                .Select(u => new TableRow
                {
                    Id = u.Id,
                    Email = u.Email,
                    IsCandidate = u.IsCandidate,
                    VoteCount = voteCounts.ContainsKey(u.Id) ? voteCounts[u.Id] : 0
                })
                .ToListAsync();
        }
    }
}