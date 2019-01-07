using System;
using System.Collections.Generic;

namespace SpeedTesting.Testing
{
	static class MathHelper
	{
		public static (double mean, double StdDev) StdDevAndMean(this IEnumerable<TimeStamp> values)
		{
			// ref: http://warrenseen.com/blog/2006/03/13/how-to-calculate-standard-deviation/
			double mean = 0.0;
			double sum = 0.0;
			double stdDev = 0.0;
			int n = 0;

			foreach (TimeStamp stamp in values)
			{
				n++;
				double doubleVal = stamp.timestamp.TotalSeconds / stamp.iterations;
				double delta = stamp.timestamp.TotalSeconds / stamp.iterations - mean;
				mean += delta / n;
				sum += delta * delta;
			}
			if (n > 1)
				stdDev = Math.Sqrt(sum / (n - 1));

			return (mean, stdDev);
		}
	}
}
