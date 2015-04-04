using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class DateTimeHelper
	{
		public static DateTime ToDate(this string str)
		{
			return new DateTime(str.Substring(0, 4).ToInt(), str.Substring(4, 2).ToInt(), str.Substring(6, 2).ToInt());
		}
	}
}
