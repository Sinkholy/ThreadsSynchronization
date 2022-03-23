using HybridThreadsSynchronization.ThreadsSynchronizer;

using PerformanceMeasurement;

var performanceTester = new SimpleIncrementPerformanceMeasurement();
var defaultPerformance = performanceTester.MeasureDefaultPerformance();
var simpleHybridLockPerformance = performanceTester.MeasureSimpleHybridLockPerformance();

Console.WriteLine($"Default performance: {defaultPerformance}");
Console.WriteLine($"SimpleHybridLock performance: {simpleHybridLockPerformance}");

var ratio = simpleHybridLockPerformance / defaultPerformance;

Console.WriteLine($"Default performance was x{Math.Round(ratio, 1)} faster.");

Console.Read();

class SimpleIncrementPerformanceMeasurement
{
	const int iterations = 100000;
	const int samplesCount = 50;

	public TimeSpan MeasureDefaultPerformance()
	{
		return Benchmark.Run(DefaultIncrement, samplesCount);

		void DefaultIncrement()
		{
			int x = 0;
			while (x < iterations)
			{
				x++;
			}
		}
	}
	public TimeSpan MeasureSimpleHybridLockPerformance()
	{
		var @lock = new SimpleHybridLock();

		return Benchmark.Run(Increment, samplesCount);

		void Increment()
		{
			int x = 0;
			while (x < iterations)
			{
				using (@lock.WaitForAccess())
				{
					x++;
				}
			}
		}
	}
}