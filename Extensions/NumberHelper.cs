using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class NumberHelper
	{
		public static string ToComma(this int value)
		{
			return "{0:#,#}".With(value);
		}
	}
}
