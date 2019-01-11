using System;
using System.Text;

namespace SpeedTesting.Tests
{
	class Sample
	{
		const string escapeStartNormal = "\x1b[";
		readonly Random rnd = new Random();

		StringBuilder testString = new StringBuilder(64);

		public double test1()
		{
			testString.Append(escapeStartNormal);
			testString.Append(rnd.Next(1, 999));
			testString.Append("C");
			string s = testString.ToString();
			testString.Clear();
			return s.Length;
		}

		readonly string[] sequence = new string[3];

		public void Preparation()
		{
			sequence[0] = escapeStartNormal;
			sequence[2] = "C";
		}

		public double test2()
		{
			sequence[1] = rnd.Next(1, 999).ToString();
			string s = string.Join("", sequence);
			return s.Length;
		}

		public double test3()
		{
			string s = string.Concat(escapeStartNormal, rnd.Next(1, 999), "C");
			return s.Length;
		}

	}
}
