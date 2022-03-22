using System.Threading;
using System.Threading.Tasks;

using HybridThreadsSynchronization.ThreadsSynchronizer;

using Xunit;

namespace HybridThreadsSynchronization_tests
{
	public class SimpleHybridLockTests
	{
		[Fact]
		public void SynchronizationTest()
		{
			const int threadsCount = 4;

			var @lock = new SimpleHybridLock();
			int workingThreadsCount = 0;

			var tasks = new Task[threadsCount];
			for (int i = 0; i < threadsCount; i++)
			{
				tasks[i] = Task.Run(() => GetAccess());
			}

			void GetAccess()
			{
				using (@lock.WaitForAccess())
				{
					int currentWorkingThreadsCount = Interlocked.Increment(ref workingThreadsCount);
					Assert.True(currentWorkingThreadsCount == 1, $"Only one thread should run at a time, actual threads count was: {workingThreadsCount}");
					Thread.Sleep(5);
					Interlocked.Decrement(ref workingThreadsCount);
				}
			}
			Task.WaitAll(tasks);
		}

		[Fact]
		public void ControlTransferTest()
		{
			const int threadsCount = 5;

			var @lock = new SimpleHybridLock();
			var sharedArray = new int[threadsCount];
			int sharedCurrentArrayIndex = 0;

			var tasks = new Task[threadsCount];
			for (int i = 0; i < threadsCount; i++)
			{
				tasks[i] = Task.Run(() => DoWork());
			}

			Task.WaitAll(tasks);

			foreach (var elem in sharedArray)
			{
				// TODO: описание ошибки
				Assert.True(elem != default);
			}

			void DoWork()
			{
				using (@lock.WaitForAccess())
				{
					sharedArray[sharedCurrentArrayIndex] = 5;
					sharedCurrentArrayIndex++;
					Thread.Sleep(5);
				}
			}
		}
	}
}