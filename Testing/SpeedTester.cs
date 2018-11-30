using System.Diagnostics;
using System;

namespace SpeedTesting
{
	static class SpeedTester
	{
		static bool isRunning;

		static Stopwatch timer;
		static string testName;

		public static void Start(string TestName)
		{
			if (isRunning)
				throw new Exception("Cannot start a new timer while another one is running");


			isRunning = true;
			testName = TestName;
			timer = Stopwatch.StartNew();
		}

		public static TestResult Finish()
		{
			if (!isRunning)
				throw new Exception("Cannot return a result when no timer is running");


			isRunning = false;
			return new TestResult(testName, timer.Elapsed.TotalSeconds);
		}
	}
}
