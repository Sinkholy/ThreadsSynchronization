using System.Diagnostics;

namespace PerformanceMeasurement
{
	class Benchmark
	{
		public static TimeSpan Run(Action method, int iterations)
		{
			var sw = new Stopwatch();
			var measurements = new TimeSpan[iterations];
			for (int i = 0; i < iterations; i++)
			{
				sw.Restart();
				method();
				measurements[i] = sw.Elapsed;
			}

			return CalculateAverageTimeSpan(measurements);
		}
		static TimeSpan CalculateAverageTimeSpan(ICollection<TimeSpan> samples)
		{
			var total = new TimeSpan();
			foreach (var sample in samples)
			{
				total += sample;
			}
			return total / samples.Count;
		}
	}
}
