using eVote.src.Models;
using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public static class DbAccess
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
    }
}