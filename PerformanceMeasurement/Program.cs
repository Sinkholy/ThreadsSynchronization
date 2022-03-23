using HybridThreadsSynchronization.ThreadsSynchronizer;

using PerformanceMeasurement;

var @default = IncrementBenchmark.BenchmarkDefault();
var shl = IncrementBenchmark.BenchmarkSimpleHybridLock();

Console.WriteLine($"Default performance: {@default}");
Console.WriteLine($"SimpleHybridLock performance: {shl}");

var ratio = shl / @default;

Console.WriteLine($"Default performance was x{Math.Round(ratio, 1)} faster.");

Console.Read();

class IncrementBenchmark
{
	const int incrementIterations = 100000;
	const int benchmarkIterations = 50;

	public static TimeSpan BenchmarkDefault()
	{
		return Benchmark.Run(DefaultIncrement, benchmarkIterations);

		void DefaultIncrement()
		{
			int x = 0;
			while (x < incrementIterations)
			{
				x++;
			}
		}
	}
	public static TimeSpan BenchmarkSimpleHybridLock()
	{
		var @lock = new SimpleHybridLock();

		return Benchmark.Run(Increment, benchmarkIterations);

		void Increment()
		{
			int x = 0;
			while (x < incrementIterations)
			{
				using (@lock.WaitForAccess())
				{
					x++;
				}
			}
		}
	}
}