using System;

namespace SpeedTesting
{
	class TimeStamp
	{
		public TimeSpan timestamp;

		public int iterations;

		public TimeStamp(TimeSpan TimeStamp, int Iterations)
		{
			timestamp = TimeStamp;
			iterations = Iterations;
		}

		public override string ToString()
		{
			return $"Time: {timestamp.TotalSeconds.ToString("0.00000")} Count: {iterations}";
		}
	}
}
