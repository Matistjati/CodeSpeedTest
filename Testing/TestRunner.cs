using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

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

			TimeSpan executionTime = ExecuteTests();

			if (tests.Count == 0)
			{
				throw new Exception("No tests have been added");
			}
			else if (tests.Count == 1)
			{

			}
			else if (tests.Count == 2)
			{
				WriteResultTwoTests(executionTime);
			}
			else
			{
				WriteResultMultipleTests(executionTime);
			}

			Console.ReadKey();
		}


		static TimeSpan ExecuteTests()
		{
			double maxIterationTimeWeighted = maxIterationTime / tests.Count;

			Stopwatch testRunTime = Stopwatch.StartNew();

			Stopwatch testTimer = new Stopwatch();

			// Keep compiler from optimizing away function contents
			double _ = 0;

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
							testTimer.Reset();
							testTimer.Start();
							for (int i = 0; i < count; i++)
							{
								// Keep compiler from optimizing away function contents
								_ += test.Key.test();
							}
							TimeSpan elapsed = testTimer.Elapsed;

							runTime += elapsed.TotalSeconds;
							test.Value.Add(new TimeStamp(elapsed, count));
						}
					} while (runTime < maxIterationTimeWeighted && count < int.MaxValue / 2);
				}
			}

			testRunTime.Stop();

			// Keep compiler from optimizing away function contents
			if (!File.Exists("Temp.txt"))
			{
				using (FileStream file = File.Create("Temp.txt"))
				{
					file.Write(BitConverter.GetBytes(_), 0, 8);
				}
				File.Delete("Temp.txt");
			}

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

		static void WriteResultMultipleTests(TimeSpan testRunTime)
		{
			List<TestInfo> testInfos = new List<TestInfo>();

			foreach (KeyValuePair<Testable, List<TimeStamp>> testResult in tests)
			{
				(double mean, double StdDev) = testResult.Value.StdDevAndMean();
				testInfos.Add(new TestInfo(StdDev, mean, testResult.Key.name));
			}

			testInfos.Sort();

			int maxNameSpacing = testInfos.Max(x => x.name.Length) + 1;

			Console.WriteLine($"Legend:");
			ConsoleExtensions.WriteColorLine($"mt:   Mean execution time in seconds", new ColorArgument(0, "mt".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"sdev: Standard deviation in seconds", new ColorArgument(0, "sdev".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"dm:   Delta mean execution time in seconds (Compared to the test below)", new ColorArgument(0, "dm".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"dp:   Delta execution time in percent (Compared to the test below)", new ColorArgument(0, "dp".Length, ConsoleColor.White));
			ConsoleExtensions.WriteColorLine($"{TestName}:\n", new ColorArgument(0, TestName.Length, ConsoleColor.White));

			const int prefixLength = 6; // ", dp: " or ", dm: " length


			for (int i = 0; i < testInfos.Count; i++)
			{
				TestInfo test = testInfos[i];
				// Fastestname colored green and some spacing
				ConsoleExtensions.WriteColor($"{test.name},{new string(' ', maxNameSpacing - test.name.Length)}mt: ",
					new ColorArgument(0, test.name.Length, ConsoleColor.White));

				ConsoleExtensions.WriteColor(test.meanTime.ToString("0." + new string('0', decimalDigitsRounding) + "E00"),
					new ColorArgument(0, "0.".Length + decimalDigitsRounding + "E00".Length, ConsoleColor.White));

				string standardDeviationS = test.stdDev.ToString("0." + new string('0', decimalDigitsRounding) + "E00");
				ConsoleExtensions.WriteColor($", sdev: {standardDeviationS}",
					new ColorArgument(", sdev: ".Length, standardDeviationS.Length, ConsoleColor.White));

				if (i != testInfos.Count - 1)
				{
					double deltaExecTime = (testInfos[i + 1].meanTime - test.meanTime);


					string deltaExecTimeP = Math.Floor((deltaExecTime / test.meanTime) * 100) + "%";
					ConsoleExtensions.WriteColor($", dp: {deltaExecTimeP}", new ColorArgument(prefixLength, deltaExecTimeP.Length, ConsoleColor.White));

					string deltaExecTimeS = deltaExecTime.ToString("0." + new string('0', decimalDigitsRounding) + "E00");
					ConsoleExtensions.WriteColorLine($", dm: {deltaExecTimeS}", new ColorArgument(prefixLength, deltaExecTimeS.Length, ConsoleColor.White));
				}
			}

			Console.Write($"\n\n\n\n{tests.Count} tests performed in {testRunTime.TotalSeconds.ToString("0.00")} seconds");
			Console.Write($"\n\nPress any key to exit");
		}
	}
}
