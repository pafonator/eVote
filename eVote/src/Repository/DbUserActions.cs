using eVote.src.Model;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public class DbUserActions
    {
        private static readonly Dictionary<UserId, ReaderWriterLockSlim> _userLocks = new();
        private static readonly ReaderWriterLockSlim _userLocksGuard = new();

        public static ReaderWriterLockSlim GetUserLock(UserId id)
        {
            _userLocksGuard.EnterUpgradeableReadLock();
            try
            {
                // Check if the lock for this user already exists
                if (!_userLocks.TryGetValue(id, out var userLock))
                {
                    // If not, create a new lock
                    _userLocksGuard.EnterWriteLock();
                    try
                    {
                        userLock = new ReaderWriterLockSlim();
                        _userLocks[id] = userLock;
                    }
                    finally
                    {
                        _userLocksGuard.ExitWriteLock();
                    }
                }
                return userLock;
            }
            finally
            {
                _userLocksGuard.ExitUpgradeableReadLock();
            }
        }

        public static async Task<User?> Login(string email, string password)
        {
            var user = await DbRead.GetUserAsync(email);
            if (user == null)
                throw new InvalidOperationException("User does not exist");
            if (user.Password != password)
                throw new InvalidOperationException("Incorrect password");
            return user;
        }

        public static async Task<User?> RegisterUser(string email, string password)
        {
            await using var db = EVoteDbContext.GetDb();
            if (db.Users.Any(u => u.Email == email))
                throw new InvalidOperationException("User already exists with this email.");

            var user = new User { Email = email, Password = password };
            db.Users.Add(user);

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                throw new InvalidOperationException("User already exists with this email.");
            }
            return user;
        }

        public static async Task RegisterAsCandidate(UserId userId)
        {
            var _currentUserLock = GetUserLock(userId);
            _currentUserLock.EnterWriteLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();
                var user = await db.Users.FindAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("User not found.");
                if (user.IsCandidate)
                    throw new InvalidOperationException("User is already a candidate.");

                user.IsCandidate = true;
                await db.SaveChangesAsync();
            }
            finally
            {
                _currentUserLock.ExitWriteLock();
            }
        }

        public static async Task UnregisterAsCandidate(UserId userId)
        {
            var _currentUserLock = GetUserLock(userId);
            _currentUserLock.EnterWriteLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();
                var user = await db.Users.FindAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("User not found.");
                if (!user.IsCandidate)
                    throw new InvalidOperationException("User is already not a candidate.");

                user.IsCandidate = false;

                // TODO: Inform all voters of this candidate that this candidate is no longer valid

                await db.SaveChangesAsync();
            }
            finally
            {
                _currentUserLock.ExitWriteLock();
            }
        }

        public static async Task AddVote(UserId userId ,UserId candidateId)
        {
            if (candidateId == userId)
            {
                throw new InvalidOperationException("Really... You just voted for yourself (-_-)");
            }

            var _currentUserLock = GetUserLock(userId);
            var _candidateLock = GetUserLock(candidateId);

            _currentUserLock.EnterWriteLock();
            _candidateLock.EnterReadLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();

                var user = await db.Users.FindAsync(userId);
                if (user == null || user.IsCandidate)
                    throw new InvalidOperationException("Candidates cannot vote.");

                var votes = await DbRead.GetVotesOfUserAsync(userId);
                if (votes.Count >= 2)
                    throw new InvalidOperationException("A user can only vote twice, unselect a vote to change it.");

                var candidate = await db.Users.FindAsync(candidateId);
                if (candidate == null || !candidate.IsCandidate)
                    throw new InvalidOperationException("The user you voted for is not a candidate.");

                db.Votes.Add(new Vote { VoterId = userId, CandidateId = candidateId });

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
                {
                    throw new InvalidOperationException("A user cannot vote twice for the same person.");
                }
            }
            finally
            {
                _candidateLock.ExitReadLock();
                _currentUserLock.ExitWriteLock();
            }
        }

        public static async Task RemoveVote(UserId userId, UserId candidateId)
        {
            if (candidateId == userId)
            {
                throw new InvalidOperationException("The candidate and voter are the same person");
            }

            var _currentUserLock = GetUserLock(userId);
            var _candidateLock = GetUserLock(candidateId);

            _currentUserLock.EnterWriteLock();
            _candidateLock.EnterReadLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();

                var vote = await db.Votes.FindAsync(new { VoterId = userId, CandidateId = candidateId });
                if (vote == null)
                {
                    throw new InvalidOperationException("No vote found for this user and candidate.");
                }

                db.Votes.Remove(vote);
                await db.SaveChangesAsync();
            }
            finally
            {
                _candidateLock.ExitReadLock();
                _currentUserLock.ExitWriteLock();
            }
        }
    }
}
