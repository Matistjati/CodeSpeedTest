namespace SpeedTesting
{
	struct TestResult
	{
		public string testName;

		public double time;

		public TestResult(string testName, double time)
		{
			this.testName = testName;
			this.time = time;
		}

		public override string ToString()
		{
			return $"Name: {testName} Time: {time}";
		}
	}
}
