using System;

namespace SpeedTesting.Testing
{
	class TestInfo : IComparable<TestInfo>
	{
		public double stdDev;
		public double meanTime;
		public string name;

		public TestInfo(double StdDev, double Mean, string Name)
		{
			stdDev = StdDev;
			meanTime = Mean;
			name = Name;
		}

		public int CompareTo(TestInfo other)
		{
			return this.meanTime.CompareTo(other.meanTime);
		}

		public override string ToString()
		{
			return $"stdDev: {stdDev} mean: {meanTime} name: {name}";
		}
	}
}
