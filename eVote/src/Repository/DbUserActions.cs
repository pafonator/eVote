using eVote.src.Model;
using Microsoft.EntityFrameworkCore;

namespace eVote.src.Repository
{
    public class DbUserActions
    {
        private static readonly Dictionary<UserId, ReaderWriterLockSlim> _userLocks = new();
        private static readonly ReaderWriterLockSlim _userLocksGuard = new();

        private ReaderWriterLockSlim _currentUserLock; // Lock for voting operations
        public UserId userId;

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

        private DbUserActions(UserId id)
        {
            userId = id;
            _currentUserLock = GetUserLock(id);
        }

        public static DbUserActions Login(string email, string password)
        {
            var user = DbRead.GetUserAsync(email).Result;
            if (user == null)
                throw new InvalidOperationException("User does not exist");
            if (user.Password != password)
                throw new InvalidOperationException("Incorrect password");

            return new DbUserActions(user.Id);
        }

        public static DbUserActions RegisterUser(string email, string password)
        {
            using var db = EVoteDbContext.GetDb();
            if (db.Users.Any(u => u.Email == email))
                throw new InvalidOperationException("User already exists with this email.");

            var user = new User { Email = email, Password = password };
            db.Users.Add(user);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("UNIQUE constraint failed") == true)
            {
                throw new InvalidOperationException("User already exists with this email.");
            }

            return new DbUserActions(user.Id);
        }

        public async Task RegisterAsCandidate()
        {
            _currentUserLock.EnterWriteLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();
                var user = await db.Users.FindAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("User not found.");

                user.IsCandidate = true;
                await db.SaveChangesAsync();
            }
            finally
            {
                _currentUserLock.ExitWriteLock();
            }
        }

        public async Task UnregisterAsCandidate()
        {
            _currentUserLock.EnterWriteLock();
            try
            {
                await using var db = EVoteDbContext.GetDb();
                var user = await db.Users.FindAsync(userId);
                if (user == null)
                    throw new InvalidOperationException("User not found.");

                user.IsCandidate = false;

                // TODO: Inform all voters of this candidate that this candidate is no longer valid

                await db.SaveChangesAsync();
            }
            finally
            {
                _currentUserLock.ExitWriteLock();
            }
        }

        public async Task VoteForCandidate(UserId candidateId)
        {
            if (candidateId == userId)
            {
                throw new InvalidOperationException("Really... You just voted for yourself (-_-)");
            }
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
    }
}
