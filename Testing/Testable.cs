using System;

namespace SpeedTesting.Testing
{
	class Testable
	{
		public readonly Action testSetup;
		public readonly Func<double> test;
		public readonly string name;

		public Testable(string Name, Func<double> Test) : this(Name, Test, null) { }

		public Testable(string Name, Func<double> Test, Action TestSetup)
		{
			if (Test is null)
			{
				throw new ArgumentNullException(nameof(Test), "Test cannot be null");
			}

			testSetup = TestSetup;
			test = Test;
			name = Name;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}
}
