using System.Collections.Generic;
using System.Text;

namespace SpeedTesting.Tests
{
	class StringAppend
	{
		const string whiteSpace = " ";
		const string escapeStartRGB = "\x1b[38;2;";
		const string escapeEnd = "m█";
		const char colorSeparator = ';';

		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			byte r = 155;
			byte g = 250;
			byte b = 190;
			StringBuilder colorStringBuilder = new StringBuilder(24);

			StringBuilder result2 = new StringBuilder();

			SpeedTester.Start("Stringbuilder append stringbuilder");

			for (long i = 0; i < timesToRun; i++)
			{
				colorStringBuilder.Append(escapeStartRGB);
				colorStringBuilder.Append(r);
				colorStringBuilder.Append(colorSeparator);
				colorStringBuilder.Append(g);
				colorStringBuilder.Append(colorSeparator);
				colorStringBuilder.Append(b);
				colorStringBuilder.Append(escapeEnd);

				result2.Append(colorStringBuilder.ToString());
				colorStringBuilder.Clear();
			}

			yield return SpeedTester.Finish();

			StringBuilder result1 = new StringBuilder();

			SpeedTester.Start("Stringbuilder append string");

			for (long i = 0; i < timesToRun; i++)
			{
				colorStringBuilder.Append(escapeStartRGB);
				colorStringBuilder.Append(r);
				colorStringBuilder.Append(colorSeparator);
				colorStringBuilder.Append(g);
				colorStringBuilder.Append(colorSeparator);
				colorStringBuilder.Append(b);
				colorStringBuilder.Append(escapeEnd);

				result1.Append(colorStringBuilder.ToString());
				colorStringBuilder.Clear();
			}

			yield return SpeedTester.Finish();


		}
	}
}
