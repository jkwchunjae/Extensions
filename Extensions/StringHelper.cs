using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

		/// <summary>
		/// ex) "{a}, {a:000}, {b}".WithVar(new {a, b});
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="str"></param>
		/// <param name="arg"></param>
		/// <returns></returns>
		public static string WithVar<T>(this string str, T arg) where T : class
		{
			var type = typeof(T);
			foreach (var member in type.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
			{
				if (!(member is FieldInfo || member is PropertyInfo))
					continue;
				var pattern = @"\{" + member.Name + @"(\:.*?)?\}";
				var alreadyMatch = new HashSet<string>();
				foreach (Match m in Regex.Matches(str, pattern))
				{
					if (alreadyMatch.Contains(m.Value)) continue; else alreadyMatch.Add(m.Value);
					string oldValue = m.Value;
					string newValue = null;
					string format = "{0" + m.Groups[1].Value + "}";
					if (member is FieldInfo)
						newValue = format.With(((FieldInfo)member).GetValue(arg));
					if (member is PropertyInfo)
						newValue = format.With(((PropertyInfo)member).GetValue(arg));
					if (newValue != null)
						str = str.Replace(oldValue, newValue);
				}
			}
			return str;
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

		public static bool IsInt(this string value)
		{
			var result = 0;
			return int.TryParse(value, out result);
		}
	}
}
