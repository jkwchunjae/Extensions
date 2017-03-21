using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class IEnumerableHelper
	{
		public static T GetRandom<T>(this IEnumerable<T> source)
		{
			return source.ElementAt(StaticRandom.Next(source.Count()));
		}

		public static IEnumerable<T> RandomShuffle<T>(this IEnumerable<T> source)
		{
			return source.Select(x => new { Index = StaticRandom.Next(999999999), T = x }).OrderBy(x => x.Index).Select(x => x.T);
		}

		public static bool Empty<T>(this IEnumerable<T> source)
		{
			return !source.Any();
		}

		public static string ToCsv<T>(this IEnumerable<T> source)
		{
			var t = typeof(T);
			var props = t.GetProperties();

			return source.Select(x => props.Select(e => e.PropertyType == typeof(DateTime) ? ((DateTime)e.GetValue(x)).ToString("yyyy-MM-dd HH:mm:ss") : e.GetValue(x)).StringJoin(",")).StringJoin("\n");
		}

		public static string ToJsonText<T>(this IEnumerable<T> source, Formatting formatting = Formatting.Indented)
		{
			return JsonConvert.SerializeObject(source, formatting);
		}
	}
}
