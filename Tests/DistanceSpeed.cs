using System;
using System.Collections.Generic;

namespace SpeedTesting.Tests
{
	class DistanceSpeed
	{
		static bool insideCircumference;
		public static IEnumerable<TestResult> DoTest(long timesToRun)
		{
			int x1 = 10;
			int y1 = 240;

			int x2 = 50;
			int y2 = 129;

			int radius = 118;

			SpeedTester.Start("Direct translated formula");

			for (long i = 0; i < timesToRun; i++)
			{
				double distance = Math.Sqrt(
					Math.Pow(x1 - x2, 2) +
					Math.Pow(y1 - y2, 2));

				// Include content so that the compiler does not remove this check
				if (distance <= radius)
				{
					insideCircumference = true;
				}
				else
				{
					insideCircumference = false;
				}
			}

			yield return SpeedTester.Finish();

			SpeedTester.Start("Optimized formula no pow");

			for (long i = 0; i < timesToRun; i++)
			{
				int xDifference = x1 - x2;
				int yDifference = y1 - y2;

				double distance = Math.Sqrt(
					xDifference * xDifference +
					yDifference * yDifference);

				if (distance <= radius)
				{
					insideCircumference = true;
				}
				else
				{
					insideCircumference = false;
				}
			}

			yield return SpeedTester.Finish();


			int radiusSquared = radius * radius;

			SpeedTester.Start("Optimized formula no pow or sqrt");

			for (long i = 0; i < timesToRun; i++)
			{
				int xDifference = x1 - x2;
				int yDifference = y1 - y2;

				double distance = (xDifference * xDifference) + (yDifference * yDifference);

				if (distance <= radiusSquared)
				{
					insideCircumference = true;
				}
				else
				{
					insideCircumference = false;
				}
			}

			yield return SpeedTester.Finish();
		}
	}
}
