using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeedTesting.Testing
{
	static class ConsoleExtensions
	{
		public static void WriteColorLine(string value, params ColorArgument[] colors)
		{
			WriteColor(value + "\n", colors);
		}

		public static void WriteColor(string value, params ColorArgument[] colors)
		{
			if (colors.Length == 0)
			{
				Console.Write(value);
				return;
			}

			Array.Sort(colors);
			Dictionary<string, ConsoleColor?> strings = new Dictionary<string, ConsoleColor?>();
			int lastEndIndex = 0;

			for (int i = 0; i < colors.Length; i++)
			{
				ColorArgument color = colors[i];
				if (lastEndIndex != color.start)
				{
					strings.Add(value.Substring(lastEndIndex, color.start), null);
				}
				strings.Add(value.Substring(color.start, color.length), color.color);
				lastEndIndex = color.length;
			}

			ColorArgument last = colors.Last();
			if (last.length + last.start < value.Length)
			{
				strings.Add(value.Substring(last.length + last.start), null);
			}

			foreach (KeyValuePair<string, ConsoleColor?> stringToWrite in strings)
			{
				if (stringToWrite.Value is null)
				{
					Console.Write(stringToWrite.Key);
				}
				else
				{
					Console.ForegroundColor = (ConsoleColor)stringToWrite.Value;
					Console.Write(stringToWrite.Key);
					Console.ResetColor();
				}
			}
		}
	}

	class ColorArgument : IComparable<ColorArgument>
	{
		public int start;
		public int length;
		public ConsoleColor color;

		public ColorArgument(int Start, int Length, ConsoleColor Color)
		{
			if (start < 0 || Length < 0)
			{
				throw new ArgumentException("start or end was negative");
			}

			start = Start;
			length = Length;
			color = Color;
		}

		public int CompareTo(ColorArgument other)
		{
			return start.CompareTo(other.start);
		}

		public override string ToString()
		{
			return $"Start: {start}, end: {length}, color: {color}";
		}
	}
}
