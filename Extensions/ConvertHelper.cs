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

		public static string GetString(this byte[] bytes, Encoding encoding)
		{
			return encoding.GetString(bytes);
		}

		public static string GetString(this byte[] bytes, int index, int count, Encoding encoding)
		{
			return encoding.GetString(bytes, index, count);
		}

		public static string GetStringUTF8(this byte[] bytes)
		{
			return bytes.GetString(Encoding.UTF8);
		}

		public static string GetStringUTF8(this byte[] bytes, int index, int count)
		{
			return bytes.GetString(index, count, Encoding.UTF8);
		}

		public static T ConvertToEnum<T>(this byte[] bytes, Encoding encoding)
		{
			return (T)Enum.Parse(typeof(T), bytes.GetString(encoding));
		}

		public static T ConvertToEnumUTF8<T>(this byte[] bytes)
		{
			return bytes.ConvertToEnum<T>(Encoding.UTF8);
		}
	}
}
