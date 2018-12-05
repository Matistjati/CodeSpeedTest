using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting.Tests
{
	class ArrayIndexing
	{
		static int[,] array = new int[10, 10];
		static int[,] sourceArray = new int[10, 10];

		public static IEnumerable<TestResult> DoTest(long timesToRun)
		{
			// Filling the array
			int count = 0;
			for (int x = 0; x < array.GetLength(0); x++)
			{
				for (int y = 0; y < array.GetLength(1); y++)
				{
					array[x, y] = count++;
				}
			}

			for (int x = 0; x < sourceArray.GetLength(0); x++)
			{
				for (int y = 0; y < sourceArray.GetLength(1); y++)
				{
					sourceArray[x, y] = 40;
				}
			}





			SpeedTester.Start("Normal Index get");

			for (long i = 0; i < timesToRun; i++)
			{
				for (int x = 0; x < array.GetLength(0); x++)
				{
					for (int y = 0; y < array.GetLength(1); y++)
					{
						int value = array[x, y];
					}
				}
			}

			yield return SpeedTester.Finish();




			SpeedTester.Start("Pointer Index get multidimensional iteration");

			IterateUnsafeGet(timesToRun);

			yield return SpeedTester.Finish();





			SpeedTester.Start("Pointer Index get linear iteration");

			IterateUnsafeGetLinear(timesToRun);

			yield return SpeedTester.Finish();





			SpeedTester.Start("Pointer Index get linear optimized");

			IterateUnsafeGetLinearOptimized(timesToRun);

			yield return SpeedTester.Finish();









			SpeedTester.Start("Pointer Index set iteration");

			IterateUnsafeSetLinear(timesToRun);

			yield return SpeedTester.Finish();





			SpeedTester.Start("Pointer Index set linear optimized");

			IterateUnsafeSetLinearOptimized(timesToRun);

			yield return SpeedTester.Finish();








			SpeedTester.Start("Pointer Index set cutout");

			IterateUnsafeSetCutout(timesToRun);

			yield return SpeedTester.Finish();














			SpeedTester.Start("Pointer Index set multidimensional iteration");

			IterateUnsafeSet(timesToRun);

			yield return SpeedTester.Finish();



			SpeedTester.Start("Normal Index set");

			for (long i = 0; i < timesToRun; i++)
			{
				for (int x = 0; x < array.GetLength(0); x++)
				{
					for (int y = 0; y < array.GetLength(1); y++)
					{
						array[x, y] = 10;
					}
				}
			}

			yield return SpeedTester.Finish();
		}

		static void IterateUnsafeGetLinearOptimized(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					int* tail = array.Length + p;
					for (long i = 0; i < timesToRun; i++)
					{
						for (int* indexer = p; indexer < tail; indexer++)
						{
							int value = *indexer;
						}
					}
				}
			}
		}


		static void IterateUnsafeSetLinearOptimized(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					int* tail = array.Length + p;
					for (long i = 0; i < timesToRun; i++)
					{
						for (int* indexer = p; indexer < tail; indexer++)
						{
							*indexer = 70;
						}
					}
				}
			}
		}



		static void IterateUnsafeSetCutout(long timesToRun)
		{
			// Assert that source is smaller than destination + start
			int startX = 0;
			int startY = 0;

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

		

		static void IterateUnsafeGetLinear(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					for (long i = 0; i < timesToRun; i++)
					{
						for (int offset = 0; offset < array.Length; offset++)
						{
							int value = *(p + offset);
						}
					}
				}
			}
		}

		static void IterateUnsafeSetLinear(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					for (long i = 0; i < timesToRun; i++)
					{
						for (int offset = 0; offset < array.Length; offset++)
						{
							*(p + offset) = 20;
						}
					}
				}
			}
		}



		static void IterateUnsafeGet(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					for (long i = 0; i < timesToRun; i++)
					{
						for (int x = 0; x < array.Length; x += array.GetLength(0))
						{
							for (int y = 0; y < array.GetLength(1); y++)
							{
								int value = *(p + x + y);
							}
						}
					}
				}
			}
		}

		static void IterateUnsafeSet(long timesToRun)
		{
			unsafe
			{
				fixed (int* p = &array[0, 0])
				{
					for (long i = 0; i < timesToRun; i++)
					{
						for (int x = 0; x < array.Length; x += array.GetLength(0))
						{
							for (int y = 0; y < array.GetLength(1); y++)
							{
								*(p + x + y) = 0;
							}
						}
					}
				}
			}
		}
	}
}
