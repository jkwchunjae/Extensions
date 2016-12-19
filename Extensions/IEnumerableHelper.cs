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
	}
}
