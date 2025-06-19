using eVote.src.Models;
using eVote.src.Repository;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public static class DbAccess
    {
        public static async Task<List<Vote>> GetAllVotesAsync()
        {
            await using var db = new EVoteDbContext();
            return await db.Votes.ToListAsync();
        }

        public static async Task<List<Vote>> GetVotesOfUserAsync(UserId userId)
        {
            await using var db = new EVoteDbContext();
            return await db.Votes
                .Where(v => v.VoterId == userId)
                .ToListAsync();
        }

        public static async Task<bool> GetUserAsync(string email)
        {
            await using var db = new EVoteDbContext();
            return await db.Users.AnyAsync(u => u.Email == email);
        }
        public static async Task<User?> GetUserAsync(string email, string password)
        {
            await using var db = new EVoteDbContext();
            return await db.Users
                .Where(v => v.Email == email && v.Password == password)
                .FirstAsync();
        }

        public static async Task<User?> GetUserAsync(UserId userId)
        {
            await using var db = new EVoteDbContext();
            return await db.Users
                .Where(v => v.Id == userId)
                .FirstOrDefaultAsync(u => u.Id == userId); //TODO
        }

        public static async Task<List<User>> GetAllUsersAsync()
        {
            await using var db = new EVoteDbContext();
            return await db.Users.ToListAsync();
        }

        public static async Task<List<User>> GetAllCandidatesAsync()
        {
            await using var db = new EVoteDbContext();
            return await db.Users
                .Where(u => u.IsCandidate)
                .ToListAsync();
        }
    }
}