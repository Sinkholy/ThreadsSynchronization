namespace HybridThreadsSynchronization
{
    namespace ThreadsSynchronizer
    {
        public class SimpleHybridLock
        {
            readonly Locker locker;
            readonly AutoResetEvent ARE;
            int threadsWaitingForEvent;

            public SimpleHybridLock()
            {
                locker = new Locker();
                ARE = new AutoResetEvent(false);
                threadsWaitingForEvent = 0;
            }

            public Access WaitForAccess()
            {
                if (!locker.TryToLock())
                {
                    EnqueueForAccess();
                }
                return new Access(this);

                void EnqueueForAccess()
                {
                    var spin = new SpinWait();
                    while (!locker.TryToLock())
                    {
                        Wait();
                    }

                    void Wait()
                    {
                        if (!spin.NextSpinWillYield)
                        {
                            spin.SpinOnce();
                        }
                        else
                        {
                            Interlocked.Increment(ref threadsWaitingForEvent);
                            ARE.WaitOne();
                            Interlocked.Decrement(ref threadsWaitingForEvent);
                        }
                    }
                }
            }
            void PassControlToNextThread()
            {
                locker.Unlock();
                if (threadsWaitingForEvent > 0)
                {
                    ARE.Set();
                }
            }

            public class Access : IDisposable
            {
                readonly SimpleHybridLock synchronizer;

                internal Access(SimpleHybridLock synchronizer)
                {
                    this.synchronizer = synchronizer;
                }

                public void Dispose()
                {
                    synchronizer.PassControlToNextThread();
                }
            }
            class Locker
            {
                const int Locked = 1;
                const int Unlocked = 0;

                int state;

                internal Locker(bool initialUnlocked = true)
                {
                    state = initialUnlocked
                          ? Unlocked
                          : Locked;
                }

                internal bool IsLocked => state == Locked;

                internal bool TryToLock()
                {
                    return Interlocked.CompareExchange(ref state, Locked, Unlocked) == Unlocked;
                }
                internal void Lock()
                {
                    Interlocked.CompareExchange(ref state, Locked, Unlocked);
                }
                internal void Unlock()
                {
                    Interlocked.CompareExchange(ref state, Unlocked, Locked);
                }
            }
        }
    }
}