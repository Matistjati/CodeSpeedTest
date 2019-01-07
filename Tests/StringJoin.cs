using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting
{
	class StringJoin
	{
		const string escapeStartNormal = "\x1b[";

		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			StringBuilder escapeSequence = new StringBuilder(escapeStartNormal.Length + "1920".Length + "C".Length);

			SpeedTester.Start("Join");

			string[] sequence = new string[3];
			sequence[0] = escapeStartNormal;
			sequence[2] = "C";

			for (long i = 0; i < timesToRun; i++)
			{
				sequence[1] = i.ToString();
				escapeSequence.Append(sequence);

				escapeSequence.Clear();
			}

			yield return SpeedTester.Finish();

			SpeedTester.Start("Append");

			for (long i = 0; i < timesToRun; i++)
			{
				escapeSequence.Append(escapeStartNormal);
				escapeSequence.Append(i);
				escapeSequence.Append("C");
				escapeSequence.Clear();
			}

			yield return SpeedTester.Finish();



		}
	}
}
