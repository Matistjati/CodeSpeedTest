using System.Collections.Generic;

namespace SpeedTesting
{
	class CacheVsDirect
	{
		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			string[,] array = new string[10, 10];

			SpeedTester.Start("Direct");

			for (long i = 0; i < timesToRun; i++)
			{
				int xx = array.GetLength(0);
				int yy = array.GetLength(1);
			}

			yield return SpeedTester.Finish();


			int x = array.GetLength(0);
			int y = array.GetLength(1);

			SpeedTester.Start("Cache");

			for (long i = 0; i < timesToRun; i++)
			{
				int xx = x;
				int yy = y;
			}

			yield return SpeedTester.Finish();
		}
	}
}
