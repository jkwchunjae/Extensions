using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class ArgumentHelper
	{
		public static Dictionary<string, string> ToCommandArgumentsDictionary(this string[] args)
		{
			if (args == null) return new Dictionary<string, string>();
			return args.Select(e => e.Split('='))
				.Select(e => new { Key = e[0], Value = (e.Count() == 1 ? null : e[1]) })
				.ToDictionary(e => e.Key, e => e.Value);
		}
	}
}
