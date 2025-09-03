using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

public enum LockMode { Read, Write, UpgradeableRead }

public sealed class MultiLock : IDisposable
{
    private readonly List<(ReaderWriterLockSlim Lock, LockMode Mode)> _locks;

    private MultiLock(IEnumerable<(ReaderWriterLockSlim Lock, LockMode Mode)> locks)
    {
        _locks = locks.ToList();

        // Acquire in order
        foreach (var (l, m) in _locks)
        {
            switch (m)
            {
                case LockMode.Read:
                    l.EnterReadLock();
                    break;
                case LockMode.Write:
                    l.EnterWriteLock();
                    break;
                case LockMode.UpgradeableRead:
                    l.EnterUpgradeableReadLock();
                    break;
            }
        }
    }

    public void Dispose()
    {
        // Release in reverse order
        for (int i = _locks.Count - 1; i >= 0; i--)
        {
            var (l, m) = _locks[i];
            switch (m)
            {
                case LockMode.Read:
                    l.ExitReadLock();
                    break;
                case LockMode.Write:
                    l.ExitWriteLock();
                    break;
                case LockMode.UpgradeableRead:
                    l.ExitUpgradeableReadLock();
                    break;
            }
        }
    }

    public static MultiLock Acquire(params (ReaderWriterLockSlim Lock, LockMode Mode)[] locks)
    {
        if (locks == null || locks.Length == 0)
            throw new ArgumentException("At least one lock required");

        // enforce deterministic ordering across all threads
        var ordered = locks
            .OrderBy(l => RuntimeHelpers.GetHashCode(l.Lock))
            .ToList();

        return new MultiLock(ordered);
    }
}
