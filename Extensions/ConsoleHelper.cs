using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class ConsoleHelper
	{
		public static void ConsoleWriteLine(this object obj)
		{
			Console.WriteLine(obj);
		}

		public static string ReadInput(string message)
		{
			Console.Write(message + ": ");
			return Console.ReadLine();
		}

		public static void Dump(this object value, string title = "")
		{
			if (title == "")
			{
				Console.WriteLine(value);
			}
			else
			{
				Console.WriteLine(title + ": " + value);
			}
		}

		public static void Dump(this IEnumerable<string> strList, string title = "")
		{
			title.Dump();
			foreach (var tuple in strList.Select((x, i) => Tuple.Create(x, i)))
				"[{1}] {0}".With(tuple.Item1, tuple.Item2).Dump();
		}
	}
}
