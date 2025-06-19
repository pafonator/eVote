using eVote.src.Models;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public class DbUser
    {

        private static readonly Dictionary<UserId, SemaphoreSlim> _userLocks = new();
        private static readonly object _loginLockGuard = new(); // To protect dictionary access

        private SemaphoreSlim _perUserLock = new(1, 1); // Lock for voting operations
        public UserId userId;
        private DbUser(UserId id)
        {
            lock (_loginLockGuard)
            {
                if (!_userLocks.TryGetValue(id, out var userLock))
                {
                    userLock = new SemaphoreSlim(1, 1);
                    _userLocks[id] = userLock;
                }
                _perUserLock = userLock;
                userId = id;
            }
        }

        public static DbUser Login(string email, string password)
        {
            // This code should be executed on the server side to ensure the user can t get the password
            User? user = DbAccess.GetUserAsync(email).Result;
            if (user == null)
            {
                throw new InvalidOperationException("User does not exist");
            }
            else if (user.Password != password)
            {
                throw new InvalidOperationException("Incorrect password");
            }

            // Acquire the lock for this user
            return new DbUser(user.Id);
        }

        public static DbUser RegisterUser(string email, string password)
        {
            using var db = EVoteDbContext.GetDb();
            // Check if the user already exists
            if (db.Users.Any(u => u.Email == email))
            {
                throw new InvalidOperationException("User already exists with this email.");
            }

            // Create a new user and add it to the database
            User user = new User { Email = email, Password = password };
            db.Users.Add(user);

            // Tries changes to the database
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // DB caught a rare race condition
                throw new InvalidOperationException("User already exists with this email.");
            }

            return new DbUser(user.Id);
        }

        public async Task RegisterAsCandidate()
        {
            _perUserLock.Wait(); 

            await using var db = EVoteDbContext.GetDb();
            var user = await db.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            user.IsCandidate = true;

            await db.SaveChangesAsync();

            _perUserLock.Release();
        }

        // Multiple people can vote in parallel, but no one can vote while a candidate is unregistering
        private static readonly ReaderWriterLockSlim _voteCandidateLock = new ReaderWriterLockSlim();
        public async Task UnregisterAsCandidate()
        {
            _perUserLock.Wait();
            _voteCandidateLock.EnterWriteLock(); // Ensure no votes happen while unregistering

            await using var db = EVoteDbContext.GetDb();
            var user = await db.Users.FindAsync(userId);

            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }
            user.IsCandidate = false;

            // TODO Inform all voters of this candidate that this candidate is no longer valid

            await db.SaveChangesAsync();

            _voteCandidateLock.ExitWriteLock(); // Release the lock after unregistering
            _perUserLock.Release();
        }
        public async Task VoteForCandidate(UserId candidateId)
        {
            _perUserLock.Wait();
            _voteCandidateLock.EnterReadLock(); // Votes can happen in parallel

            await using var db = EVoteDbContext.GetDb();

            var user = await db.Users.FindAsync(userId);
            if (user == null || user.IsCandidate)
            {
                throw new InvalidOperationException("Candidates cannot vote.");
            }

            var candidate = await db.Users.FindAsync(candidateId);
            if (candidate == null || !candidate.IsCandidate)
            {
                throw new InvalidOperationException("The user you voted for is not a candidate.");
            }

            var votes = DbAccess.GetVotesOfUserAsync(userId);
            if (votes.Result.Count >= 2)
            {
                throw new InvalidOperationException("A user can only vote twice, unselect a vote to change it.");
            }

            // Add a new Vote to the database
            db.Votes.Add(new Vote { VoterId = userId, CandidateId = candidateId });

            // Tries changes to the database
            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                // DB caught a rare race condition
                throw new InvalidOperationException("A user cannot vote twice for the same person.");
            }

            _voteCandidateLock.ExitReadLock(); // Release the lock after voting
            _perUserLock.Release();
        }
    }
}

