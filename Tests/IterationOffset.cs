using System.Collections.Generic;

namespace SpeedTesting.Tests
{
	class IterationOffset
	{
		class SmallRectangle
		{
			public static readonly SmallRectangle empty = new SmallRectangle(0, 0, 0, 0);

			public short X;

			public short Y;

			public short Width;

			public short Height;

			public SmallRectangle(short x, short y, short width, short height)
			{
				X = x;
				Y = y;
				Width = width;
				Height = height;
			}

			public override string ToString()
			{
				return $"X: {X} Y: {Y} W: {Width} H: {Height}";
			}
		}

		static readonly string[,] colors = new string[1920, 1080];
		static List<SmallRectangle> spritePositions = new List<SmallRectangle>(40);

		public static IEnumerable<TestResult> DoTest(long timesToRun)
		{
			// Sample data 
			spritePositions.Add(new SmallRectangle(458, 0, 108, 108));
			spritePositions.Add(new SmallRectangle(494, 14, 36, 36));
			spritePositions.Add(new SmallRectangle(145, 97, 94, 94));
			spritePositions.Add(new SmallRectangle(174, 126, 36, 36));
			spritePositions.Add(new SmallRectangle(750, 8, 3, 36));
			spritePositions.Add(new SmallRectangle(1070, 54, 36, 36));


			SpeedTester.Start("Iteration precalculated");

			for (long i = 0; i < timesToRun; i++)
			{
				foreach (SmallRectangle rect in spritePositions)
				{
					int width = rect.Width + rect.X;
					int height = rect.Height + rect.Y;

					for (short x = rect.X; x < width; x++)
					{
						for (short y = rect.Y; y < height; y++)
						{
							if (colors[x, y] is null)
							{
								colors[x, y] = " ";
							}
						}
					}
				}
			}

			yield return SpeedTester.Finish();




			SpeedTester.Start("Iteration addition");

			for (long i = 0; i < timesToRun; i++)
			{
				foreach (SmallRectangle rect in spritePositions)
				{
					for (short x = 0; x < rect.Width; x++)
					{
						for (short y = 0; y < rect.Height; y++)
						{
							if (colors[x + rect.X, y + rect.Y] is null)
							{
								colors[x + rect.X, y + rect.Y] = " ";
							}
						}
					}
				}
			}

			yield return SpeedTester.Finish();
		}
	}
}
