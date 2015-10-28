using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public interface IGetString
	{ }

	public static class ObjectHelper
	{
		public static string GetString<T>(this T obj) where T : IGetString
		{
			var typeInfo = typeof(T);

			return typeInfo.GetFields()
				.Select(x => "{0}: {1}".With(x.Name, x.GetValue(obj)))
				.StringJoin(", ");
		}
	}
}
