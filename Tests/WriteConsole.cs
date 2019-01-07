using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting
{
	class WriteConsole
	{
		[DllImport("kernel32")]
		public static extern IntPtr GetStdHandle(StdHandle index);

		public enum StdHandle
		{
			OutputHandle = -11,
			InputHandle = -10,
			ErrorHandle = -12
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
		public static extern bool WriteConsoleW(
			IntPtr hConsoleOutput,
			StringBuilder lpBuffer,
			int nNumberOfCharsToWrite,
			out int lpNumberOfCharsWritten,
			IntPtr lpReservedMustBeNull);

		static string[,] sampleStrings = new string[100, 40];
		public static IEnumerable<TimeStamp> DoTest(long timesToRun)
		{
			// Filling sampleStrings with random strings
			Random random = new Random();
			for (int i = 0; i < sampleStrings.GetLength(0); i++)
			{
				for (int j = 0; j < sampleStrings.GetLength(1); j++)
				{
					StringBuilder stringBuilder = new StringBuilder();
					int length = random.Next(3, 10);
					for (int k = 0; k < length; k++)
					{
						stringBuilder.Append((char)random.Next(65, 89));
					}
					sampleStrings[i, j] = stringBuilder.ToString();
				}
			}

			IntPtr outPutHandle = GetStdHandle(StdHandle.OutputHandle);

			SpeedTester.Start("One big stringbuilder");

			StringBuilder allStrings = new StringBuilder(sampleStrings.Length * 8);
			for (long i = 0; i < timesToRun; i++)
			{
				foreach (string jabble in sampleStrings)
				{
					allStrings.Append(jabble.Substring(2));
				}
				WriteConsoleW(
					outPutHandle,
					allStrings,
					allStrings.Length,
					out int ignore,
					IntPtr.Zero
					);
				allStrings.Clear();
				Console.SetCursorPosition(0, 0);
			}

			yield return SpeedTester.Finish();


			SpeedTester.Start("Multithreaded writing sort of");

			StringBuilder currentString = new StringBuilder(20);
			for (long i = 0; i < timesToRun; i++)
			{
				Task write = Task.Run(() => { });
				foreach (string jabble in sampleStrings)
				{
					currentString.Append(jabble.Substring(2));
					write.Wait();
					write = Task.Run(() =>
					{
						WriteConsoleW(
							outPutHandle,
							allStrings,
							allStrings.Length,
							out int ignore,
							IntPtr.Zero
							);
					});
					currentString.Clear();
				}


				Console.SetCursorPosition(0, 0);
			}

			yield return SpeedTester.Finish();

			Console.Clear();
		}
	}
}
