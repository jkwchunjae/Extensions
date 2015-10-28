﻿using System;
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

		public static string Left(this string value, int length = 1)
		{
			if (value.Length < length)
				return value;
			return value.Substring(0, length);
		}

		public static string Right(this string value, int length = 1)
		{
			if (value.Length < length)
				return value;
			return value.Substring(value.Length - length, length);
		}

		public static int ToInt(this string value, int defaultValue = 0)
		{
			var result = defaultValue;
			if (!int.TryParse(value, out result))
				return defaultValue;
			return result;
		}

		public static bool IsInt(this string value)
		{
			var result = 0;
			return int.TryParse(value, out result);
		}

		public static double ToDouble(this string value, double defaultValue = 0)
		{
			var result = defaultValue;
			if (!double.TryParse(value, out result))
				return defaultValue;
			return result;
		}

		public static bool ToBoolean(this string value, bool defaultValue = false)
		{
			return value.ToLower() == "true";
		}

		public static string ToCamelCase(this string value)
		{
			if (value[0] >= 'A' && value[0] <= 'Z')
				value = value.Substring(0, 1).ToLower() + value.Substring(1);
			return value;
		}
	}
}
