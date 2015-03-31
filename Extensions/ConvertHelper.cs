using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class ConvertHelper
	{
		public static int ConvertToInt32(this byte[] bytes, int startIndex = 0)
		{
			return BitConverter.ToInt32(bytes, startIndex);
		}

		public static string ConvertToString(this byte[] bytes, Encoding encoding)
		{
			return encoding.GetString(bytes);
		}
	}
}
