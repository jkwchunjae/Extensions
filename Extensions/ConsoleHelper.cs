using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class ConsoleHelper
	{
		public static void Dump(this object obj)
		{
			Console.WriteLine(obj);
		}
	}
}
