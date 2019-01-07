using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using static SpeedTesting.Testing.MathHelper;

namespace SpeedTesting.Testing
{
	class TestRunner
	{
		// Divide this number by tests.count if more than one test is performed
		public static float maxIterationTime = 0.002f;
		// The higher this is, the better the result. If you increase this, lower maxiterationtime
		public static int timesToPerformAllTests = 1000;

		public static int decimalDigitsRounding = 3;
		public static string TestName;
		public static TestType testType = TestType.None;
		// The list will be the timestamps
		static Dictionary<Testable, List<TimeStamp>> tests = new Dictionary<Testable, List<TimeStamp>>();

		public static void Test(Testable Test)
		{
			tests.Add(Test, new List<TimeStamp>());
		}

		static void Main(string[] args)
		{
			TestMaster.InitializeTests();

			double totalTime = 0;

			Stopwatch stopwatch = new Stopwatch();
			for (int i = 0; i < 10000; i++)
			{
				stopwatch.Start();
				foreach (KeyValuePair<Testable, List<TimeStamp>> test in tests)
				{
					test.Key.test();
				}
				totalTime += stopwatch.Elapsed.TotalSeconds;
				stopwatch.Reset();
			}
			totalTime /= 10000;

			Console.WriteLine($"Estimated test time: {(timesToPerformAllTests * 10000000 * (maxIterationTime / tests.Count) * totalTime * 5 * tests.Count).ToString("0.00")}\n\n\n");

			switch (testType)
			{
				case TestType.Comparison:
					TimeSpan totalExecutionTime = CompareTwoTests();
					WriteResultTwoTests(totalExecutionTime);
					break;

				case TestType.ComparisonMultiple:
				case TestType.BenchMarkSingle:
					throw new NotImplementedException("Not implemented yet");

				case TestType.None:
				default:
					throw new Exception("testType Was not set in TestRunner");
			}

			Console.ReadKey();
		}


		static TimeSpan CompareTwoTests()
		{
			if (tests.Count != 2)
			{
				throw new ArgumentException($"Tests count was not 2 while testType was set to comparison of two");
			}

			double maxIterationTimeTwo = maxIterationTime / 2;

			return CompareTests(maxIterationTimeTwo);
		}

		static TimeSpan CompareMultipleTests()
		{
			if (tests.Count == 0)
			{
				throw new ArgumentException($"No tests exist");
			}

			double maxIterationTimeMultiple = maxIterationTime / tests.Count;

			return CompareTests(maxIterationTimeMultiple);
		}

		static TimeSpan CompareTests(double maxIterationTime)
		{
			Stopwatch testRunTime = Stopwatch.StartNew();

			for (int l = 0; l < timesToPerformAllTests; l++)
			{
				foreach (KeyValuePair<Testable, List<TimeStamp>> test in tests)
				{
					// Doing any possible preparations the tester might want
					test.Key.testSetup?.Invoke();

					int n = 10, count = 1;
					double runTime;
					do
					{
						runTime = 0;
						count *= 2;
						for (int j = 0; j < n; j++)
						{
							Stopwatch timer = Stopwatch.StartNew();
							for (int i = 0; i < count; i++)
							{
								test.Key.test();
							}
							TimeSpan elapsed = timer.Elapsed;

							runTime += elapsed.TotalSeconds;
							test.Value.Add(new TimeStamp(timer.Elapsed, count));
						}
					} while (runTime < maxIterationTime && count < int.MaxValue / 2);
				}
			}

			testRunTime.Stop();
			return testRunTime.Elapsed;
		}


		static void WriteResultTwoTests(TimeSpan testRunTime)
		{
			string nameTest1 = tests.ElementAt(0).Key.name;
			(double meanTest1, double StdDevTest1) = tests.ElementAt(0).Value.StdDevAndMean();

			string nameTest2 = tests.ElementAt(1).Key.name;
			(double meanTest2, double StdDevTest2) = tests.ElementAt(1).Value.StdDevAndMean();

			string fastestName;
			double fastestTime;
			string slowestName;
			double slowestTime;

			if (meanTest1 > meanTest2)
			{
				slowestName = nameTest1;
				slowestTime = meanTest1;

				fastestTime = meanTest2;
				fastestName = nameTest2;
			}
			else
			{
				slowestName = nameTest2;
				slowestTime = meanTest2;

				fastestTime = meanTest1;
				fastestName = nameTest1;
			}

			int maxNameSpacing = Math.Max(fastestName.Length, slowestName.Length) + 1;

			Console.WriteLine($"Legend:");
			ConsoleExtensions.WriteColorLine($"mt:   Mean execution time in seconds", new ColorArgument(0, "mt".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"sdev: Standard deviation in seconds", new ColorArgument(0, "sdev".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"dm:   Delta mean execution time in seconds", new ColorArgument(0, "dm".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"dp:   Delta execution time in percent", new ColorArgument(0, "dp".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"{TestName}:\n", new ColorArgument(0, TestName.Length, ConsoleColor.White));

			const int prefixLength = 6; // ", dp: " or ", dm: " length

			// Fastestname colored green and some spacing
			ConsoleExtensions.WriteColor($"{fastestName},{new string(' ', maxNameSpacing - fastestName.Length)}mt: ",
				new ColorArgument(0, fastestName.Length, ConsoleColor.Green));


			ConsoleExtensions.WriteColor(fastestTime.ToString("0." + new string('0', decimalDigitsRounding) + "E00"), 
				new ColorArgument(0, "0.".Length + decimalDigitsRounding + "E00".Length, ConsoleColor.White));

			string standardDeviationS = StdDevTest1.ToString("0." + new string('0', decimalDigitsRounding) + "E00");
			ConsoleExtensions.WriteColor($", sdev: {standardDeviationS}", 
				new ColorArgument(", sdev: ".Length, standardDeviationS.Length, ConsoleColor.White));

			double deltaExecTime = (slowestTime - fastestTime);


			string deltaExecTimeP = Math.Floor((deltaExecTime / fastestTime) * 100) + "%";
			ConsoleExtensions.WriteColor($", dp: {deltaExecTimeP}", new ColorArgument(prefixLength, deltaExecTimeP.Length, ConsoleColor.White));

			string deltaExecTimeS = deltaExecTime.ToString("0." + new string('0', decimalDigitsRounding) + "E00");
			ConsoleExtensions.WriteColorLine($", dm: {deltaExecTimeS}", new ColorArgument(prefixLength, deltaExecTimeS.Length, ConsoleColor.White));





			ConsoleExtensions.WriteColor($"{slowestName},{new string(' ', maxNameSpacing - slowestName.Length)}mt: ",
				new ColorArgument(0, slowestName.Length, ConsoleColor.Red));


			ConsoleExtensions.WriteColor(slowestTime.ToString("0." + new string('0', decimalDigitsRounding) + "E00"), 
				new ColorArgument(0, "0.".Length + decimalDigitsRounding + "E00".Length, ConsoleColor.White));

			string standardDeviationS2 = StdDevTest2.ToString("0." + new string('0', decimalDigitsRounding) + "E00");
			ConsoleExtensions.WriteColor($", sdev: {standardDeviationS2}",
				new ColorArgument(", sdev: ".Length, standardDeviationS2.Length, ConsoleColor.White));

			string deltaExecTimeP2 = Math.Floor((deltaExecTime / slowestTime) * 100) + "%";
			ConsoleExtensions.WriteColor($", dp: {deltaExecTimeP2}", new ColorArgument(prefixLength, deltaExecTimeP2.Length, ConsoleColor.White));

			Console.Write($"\n\n\n\n{tests.Count} tests performed in {testRunTime.TotalSeconds.ToString("0.00")} seconds");
			Console.Write($"\n\nPress any key to exit");
		}
	}
}
