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
		
		public static void Dump(this string str)
		{
			Console.WriteLine(str);
		}
	}
}
