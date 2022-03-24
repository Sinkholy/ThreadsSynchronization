
namespace HybridThreadsSynchronization
{
	public class AsynchronousHybridLock
	{
        readonly Locker locker;
        readonly SemaphoreSlim semaphore;

		public AsynchronousHybridLock()
		{
            semaphore = new SemaphoreSlim(1);
            locker = new Locker();
        }

		public async Task<Access> WaitForAccessAsync()
		{
			if (!locker.TryToLock())
			{
				await EnqueueForAccess();
			}

			return new Access(this);

            async Task EnqueueForAccess()
            {
                var spin = new SpinWait();
                while (!locker.TryToLock())
                {
                    await Wait();
                }

                async Task Wait()
                {
                    if (!spin.NextSpinWillYield)
                    {
                        spin.SpinOnce();
                    }
                    else
                    {
                        await semaphore.WaitAsync();
                    }
                }
            }
        }
        void PassControlToNextThread()
        {
            locker.Unlock();
            semaphore.Release();
        }

        public class Access : IDisposable
        {
            readonly AsynchronousHybridLock synchronizer;

            internal Access(AsynchronousHybridLock synchronizer)
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
