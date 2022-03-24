using System.Threading;
using System.Threading.Tasks;

using HybridThreadsSynchronization;
using HybridThreadsSynchronization.ThreadsSynchronizer;

using Xunit;

namespace HybridThreadsSynchronization_tests
{
	public class AsynchronousHybridLockTests
	{
		[Fact]
		public async void SynchronizationTest()
		{
			const int threadsCount = 4;
			int workingThreadsCount = 0;
			var @lock = new AsynchronousHybridLock();

			var tasks = new Task[threadsCount];
			for (int i = 0; i < threadsCount; i++)
			{
				tasks[i] = Task.Run(async () => await GetAccess());
			}

			async Task GetAccess()
			{
				using (await @lock.WaitForAccessAsync())
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
		public async void ControlTransferTest()
		{
			const int threadsCount = 5;

			var @lock = new AsynchronousHybridLock();
			var sharedArray = new int[threadsCount];
			int sharedCurrentArrayIndex = 0;

			var tasks = new Task[threadsCount];
			for (int i = 0; i < threadsCount; i++)
			{
				tasks[i] = Task.Run(async () => await DoWork());
			}

			Task.WaitAll(tasks);

			foreach (var elem in sharedArray)
			{
				// TODO: описание ошибки
				Assert.True(elem != default);
			}

			async Task DoWork()
			{
				using (await @lock.WaitForAccessAsync())
				{
					sharedArray[sharedCurrentArrayIndex] = 1;
					sharedCurrentArrayIndex++;
					Thread.Sleep(5);
				}
			}
		}
	}
}
