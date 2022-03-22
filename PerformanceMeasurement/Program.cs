using System.Diagnostics;

using HybridThreadsSynchronization.ThreadsSynchronizer;

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
	public TimeSpan MeasureDefaultPerformance()
	{
		const int iterations = 100000;
		const int samplesCount = 50;
		var sw = new Stopwatch();
		var measurements = new TimeSpan[samplesCount];

		for (int i = 0; i < samplesCount; i++)
		{
			int x = 0;
			sw.Restart();
			
			while (x < iterations)
			{
				x++;
			}

			measurements[i] = sw.Elapsed;
		}

		return CalculateAverageTimeSpan(measurements);
	}
	public TimeSpan MeasureSimpleHybridLockPerformance()
	{
		const int iterations = 100000;
		const int samplesCount = 50;
		var @lock = new SimpleHybridLock();
		var sw = new Stopwatch();
		var measurements = new TimeSpan[samplesCount];

		for (int i = 0; i < samplesCount; i++)
		{
			int x = 0;
			sw.Restart();

			while (x < iterations)
			{
				using (@lock.WaitForAccess())
				{
					x++;
				}
			}

			measurements[i] = sw.Elapsed;
		}

		return CalculateAverageTimeSpan(measurements);
	}
	TimeSpan CalculateAverageTimeSpan(ICollection<TimeSpan> samples)
	{
		var total = new TimeSpan();
		foreach (var sample in samples)
		{
			total += sample;
		}
		return total / samples.Count;
	}
}