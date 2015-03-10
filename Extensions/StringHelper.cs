using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Extensions
{
	public static class StringHelper
	{
		public static string With(this string str, params object[] param)
		{
			return string.Format(str, param);
		}

		public static string StringJoin(this IEnumerable<string> strs, string separator)
		{
			return string.Join(separator, strs);
		}

		public static string StringJoin(this IEnumerable<string> strs, string left, string separator, string right)
		{
			return "{0}{1}{2}".With(left, strs.StringJoin(separator), right);
		}

		public static string RegexReplace(this string input, string pattern, string replacement)
		{
			if (input == null) return null;
			return Regex.Replace(input, pattern, replacement);
		}

		public static int ToInt(this string value, int defaultValue = 0)
		{
			var result = defaultValue;
			int.TryParse(value, out result);
			return result;
		}

	}
}
