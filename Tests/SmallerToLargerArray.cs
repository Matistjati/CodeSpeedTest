using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting.Tests
{
	class SmallerToLargerArray
	{
		static int[,] array = new int[10, 10];
		static int[,] sourceArray = new int[5, 5];

		public static IEnumerable<TestResult> DoTest(long timesToRun)
		{
			// Filling the arrays
			for (int x = 0; x < array.GetLength(0); x++)
			{
				for (int y = 0; y < array.GetLength(1); y++)
				{
					array[x, y] = 70;
				}
			}

			for (int x = 0; x < sourceArray.GetLength(0); x++)
			{
				for (int y = 0; y < sourceArray.GetLength(1); y++)
				{
					sourceArray[x, y] = 40;
				}
			}





			SpeedTester.Start("Filling normal");

			for (long i = 0; i < timesToRun; i++)
			{
				int startX = 3;
				int startY = 3;

				int xIndex;
				int yIndex;
				for (int x = 0; x < sourceArray.GetLength(0); x++)
				{
					if ((xIndex = x + startX) < 0)
						continue;

					for (int y = 0; y < sourceArray.GetLength(1); y++)
					{
						if ((yIndex = y + startY) < 0)
							continue;

						array[xIndex, yIndex] = sourceArray[x, y];
					}
				}
			}

			yield return SpeedTester.Finish();




			SpeedTester.Start("Filling using pointers");


			IterateUnsafeSetCutout(timesToRun);

			yield return SpeedTester.Finish();
		}

		static void IterateUnsafeSetCutout(long timesToRun)
		{
			// Assert that source is smaller than destination + start
			int startX = 3;
			int startY = 3;

			//Console.Write("\n\n\n\n\n\n");

			unsafe
			{
				fixed (int* dest = &array[0, 0], source = &sourceArray[0, 0])
				{
					for (long i = 0; i < timesToRun; i++)
					{
						int* sourceTail = source + sourceArray.Length;

						int moveAmountDest = array.GetLength(1) - sourceArray.GetLength(1);
						int* destPtr = (dest + (startX * array.GetLength(1)) + startY);
						destPtr--;

						for (int* sourcePtr = source; sourcePtr < sourceTail; destPtr += moveAmountDest)
						{
							for (int* rowEnd = sourcePtr + sourceArray.GetLength(1); sourcePtr < rowEnd; sourcePtr++)
							{
								destPtr++;
								*destPtr = *sourcePtr;
							}
						}
					}
				}
			}

			//for (int x = 0; x < array.GetLength(0); x++)
			//{
			//	for (int y = 0; y < array.GetLength(1); y++)
			//	{
			//		int val = array[x, y];
			//		if (val == 70)
			//		{
			//			Console.ForegroundColor = ConsoleColor.Green;
			//		}
			//		else
			//		{
			//			Console.ForegroundColor = ConsoleColor.Red;
			//		}

			//		Console.Write($"{array[x, y]} ");

			//	}
			//	Console.Write("\n");
			//}
			//Console.ReadKey();
		}

	}
}
