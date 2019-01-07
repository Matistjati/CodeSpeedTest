using System;

namespace SpeedTesting.Testing
{
	class Testable
	{
		public readonly Action testSetup;
		public readonly Action test;
		public readonly String name;

		public Testable(string Name, Action Test) : this(Name, Test, null) { }

		public Testable(string Name, Action Test, Action TestSetup)
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
