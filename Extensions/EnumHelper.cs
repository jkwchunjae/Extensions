using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class EnumHelper
	{
		public static T ParseEnum<T>(this string value)
		{
			return (T)Enum.Parse(typeof(T), value);
		}
	}
}
