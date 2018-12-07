using System.Collections.Generic;

namespace SpeedTesting
{
	class IterationSpeed
	{
		public struct Coord
		{
			internal static readonly Coord empty = new Coord();

			public int X;

			public int Y;

			public Coord(int x, int y)
			{
				X = x;
				Y = y;
			}
		}

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

		private struct Distance
		{
			public int start;
			public int length;

			public Distance(int Start, int Length)
			{
				start = Start;
				length = Length;
			}

			public override string ToString()
			{
				return $"s: {start}  l: {length}";
			}
		}

		static List<SmallRectangle> spritePositions = new List<SmallRectangle>(40);
		static Coord displaySize = new Coord(1920, 1080);

		static Dictionary<int, Distance> filledRows = new Dictionary<int, Distance>(40);

		public static IEnumerable<TestResult> DoTest(long timesToRun)
		{
			// sample data
			spritePositions.Add(new SmallRectangle(458, -22, 108, 108));
			spritePositions.Add(new SmallRectangle(494, 14, 36, 36));
			spritePositions.Add(new SmallRectangle(145, 97, 94, 94));
			spritePositions.Add(new SmallRectangle(174, 126, 36, 36));
			spritePositions.Add(new SmallRectangle(750, 8, 3, 36));
			spritePositions.Add(new SmallRectangle(1070, 54, 36, 36));

			SpeedTester.Start("Iterating foreach loop 1 foreach");

			for (long a = 0; a < timesToRun; a++)
			{
				IterateForeach();
			}

			yield return SpeedTester.Finish();

			SpeedTester.Start("Iterating foreach loop optimized");

			for (long a = 0; a < timesToRun; a++)
			{
				IterateForeachOptimized();
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Iterating for loop");

			for (long a = 0; a < timesToRun; a++)
			{
				Iteratefor();
			}

			yield return SpeedTester.Finish();
			filledRows.Clear();
		}

		static void Iteratefor()
		{
			for (int i = 0; i < spritePositions.Count; i++)
			{
				for (short y = 0; y < spritePositions[i].Height; y++)
				{
					short index = (short)(spritePositions[i].Y + y);
					// spritePositions[i].Y + y (spritePositions[i].Y might be negative)
					if (index < 0)
						continue;

					if (filledRows.TryGetValue((ushort)index, out Distance currentRow))
					{
						if (spritePositions[i].X < currentRow.start && spritePositions[i].X > 0)
						{
							currentRow.length += currentRow.start - spritePositions[i].X;

							currentRow.start = spritePositions[i].X;
						}
						else
						{
							currentRow.length += spritePositions[i].X - currentRow.start;

							if (currentRow.length + currentRow.start > displaySize.X)
							{
								currentRow.length = displaySize.X - currentRow.start;
							}
						}

						// Enable if we want to use sprites of different sizes
						if (spritePositions[i].Width > currentRow.length)
						{
							currentRow.length = spritePositions[i].Width;
						}

						filledRows[(ushort)index] = currentRow;
					}
					else
					{
						int positiveX = (spritePositions[i].X < 0)
							? 0
							: spritePositions[i].X;

						filledRows[(ushort)index] = new Distance(positiveX, spritePositions[i].Width);
					}
				}
			}
		}

		static void IterateForeach()
		{
			foreach (SmallRectangle rectangle in spritePositions)
			{
				for (short y = 0; y < rectangle.Height; y++)
				{
					short index = (short)(rectangle.Y + y);
					// spritePositions[i].Y + y (spritePositions[i].Y might be negative)
					if (index < 0)
						continue;

					if (filledRows.TryGetValue((ushort)index, out Distance currentRow))
					{
						if (rectangle.X < currentRow.start && rectangle.X > 0)
						{
							currentRow.length += currentRow.start - rectangle.X;

							currentRow.start = rectangle.X;
						}
						else
						{
							currentRow.length += rectangle.X - currentRow.start;

							if (currentRow.length + currentRow.start > displaySize.X)
							{
								currentRow.length = displaySize.X - currentRow.start;
							}
						}

						// Enable if we want to use sprites of different sizes
						if (rectangle.Width > currentRow.length)
						{
							currentRow.length = rectangle.Width;
						}

						filledRows[(ushort)index] = currentRow;
					}
					else
					{
						int positiveX = (rectangle.X < 0)
							? 0
							: rectangle.X;

						filledRows[(ushort)index] = new Distance(positiveX, rectangle.Width);
					}
				}
			}
		}

		static void IterateForeachOptimized()
		{
			foreach (SmallRectangle rectangle in spritePositions)
			{
				if (rectangle.Y < 0)
				{
					rectangle.Height += rectangle.Y;
					rectangle.Y = 0;
				}



				for (short y = rectangle.Y; y < rectangle.Height + rectangle.Y; y++)
				{
					if (filledRows.TryGetValue(y, out Distance currentRow))
					{
						if (rectangle.X < currentRow.start && rectangle.X > 0)
						{
							currentRow.length += currentRow.start - rectangle.X;

							currentRow.start = rectangle.X;
						}
						else
						{
							currentRow.length += rectangle.X - currentRow.start;

							if (currentRow.length + currentRow.start > displaySize.X)
							{
								currentRow.length = displaySize.X - currentRow.start;
							}
						}

						// Enable if we want to use sprites of different sizes
						if (currentRow.length < rectangle.Width)
						{
							currentRow.length = rectangle.Width;
						}

						filledRows[y] = currentRow;
					}
					else
					{
						int positiveX = (rectangle.X < 0)
							? 0
							: rectangle.X;

						filledRows[y] = new Distance(positiveX, rectangle.Width);
					}
				}
			}
		}
	}
}
