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

		public static TimeStamp Finish()
		{
			if (!isRunning)
				throw new Exception("Cannot return a result when no timer is running");


			isRunning = false;
			// Temp
			return new TimeStamp(new TimeSpan(), 0);
		}
	}
}
