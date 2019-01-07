using System;
using System.Text;

namespace SpeedTesting.Tests
{
	class Sample
	{
		const string escapeStartNormal = "\x1b[";
		readonly Random rnd = new Random();

		StringBuilder testString = new StringBuilder(64);

		public void test1()
		{
			testString.Append(escapeStartNormal);
			testString.Append(rnd.Next(1, 999));
			testString.Append("C");
			string s = testString.ToString();
			testString.Clear();
		}

		readonly string[] sequence = new string[3];

		public void Preparation()
		{
			sequence[0] = escapeStartNormal;
			sequence[2] = "C";
		}

		public void test2()
		{
			sequence[1] = rnd.Next(1, 999).ToString();
			string s = string.Join("", sequence);
		}

	}
}
