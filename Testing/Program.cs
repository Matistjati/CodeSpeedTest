using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SpeedTesting.Tests
{
	class Program
	{
		static readonly long timesToRunTests = 5000000;
		static readonly int decimals = 3;
		static readonly bool diplayAverageTime = true;

		static void Main(string[] args)
		{
			Console.WriteLine($"Performing {timesToRunTests} iterations\n\n\n");

			List<TestResult> results = new List<TestResult>();

			Stopwatch testRunTime = Stopwatch.StartNew();

			// Doing the tests
			results.AddRange(DistanceSpeed.DoTest(timesToRunTests));

			testRunTime.Stop();

			results.Sort((x, y) => x.time.CompareTo(y.time));

			int maxSpacing = results.Max(x => x.testName.Length);
			maxSpacing += 2;

			for (int i = 0; i < results.Count; i++)
			{
				string spacing = new string(' ', maxSpacing - results[i].testName.Length);
				string time = results[i].time.ToString("0." + new string('0', decimals));
				string averageTime = (diplayAverageTime ? $", took {(results[i].time / timesToRunTests).ToString("0.0E00")} on average" : "");
				Console.WriteLine($"{results[i].testName}:{spacing}{time}{averageTime}");
			}


			Console.WriteLine($"\n\n\n\nAll tests done, took {testRunTime.Elapsed.TotalSeconds.ToString("0." + new string('0', decimals))} seconds to finish");
			Console.WriteLine($"Fastest one was {results[0].testName}, taking {results[0].time} seconds");
			Console.WriteLine("Press any key to exit . . .");
			Console.ReadKey();
		}
	}
}
