using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting
{
	class ContainsSpeed
	{
		static List<int> sample1 = new List<int>();
		static List<int> sample2 = new List<int>();

		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			bool contains = false;

			for (int i = 0; i < 7; i++)
			{
				sample1.Add(i);
			}

			for (int i = 5; i < 13; i++)
			{
				sample2.Add(i);
			}


			SpeedTester.Start("Convert to hashset");

			for (long i = 0; i < timesToRun; i++)
			{
				HashSet<int> hashedSample2 = new HashSet<int>(sample2);

				for (int j = 0; j < sample1.Count; j++)
				{
					if (hashedSample2.Contains(sample1[j]))
					{
						contains = true;
					}
				}
			}


			int intersectCount2 = 0;

			yield return SpeedTester.Finish();

			SpeedTester.Start("Normal contains");

			for (long i = 0; i < timesToRun; i++)
			{
				for (int j = 0; j < sample1.Count; j++)
				{
					if (sample2.Contains(sample1[j]))
					{
						contains = true;
					}
				}
			}

			yield return SpeedTester.Finish();




			SpeedTester.Start("Linq");


			for (int i = 0; i < timesToRun; i++)
			{
				foreach (int j in sample2.Intersect(sample1))
				{

					contains = true;
				}
			}



			yield return SpeedTester.Finish();


		}
	}
}
