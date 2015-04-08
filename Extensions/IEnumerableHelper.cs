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
	}
}
