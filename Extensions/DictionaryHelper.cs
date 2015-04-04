using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public class DefaultDictionary<TKey, TValue> : Dictionary<TKey, TValue>
	{
		TValue _default;
		public TValue DefaultValue
		{
			get { return _default; }
			set { _default = value; }
		}

		public DefaultDictionary() : base() { }
		public DefaultDictionary(TValue defaultValue)
			: base()
		{
			_default = defaultValue;
		}

		public DefaultDictionary(Dictionary<TKey, TValue> dict)
			: base(dict)
		{
		}

		public DefaultDictionary(Dictionary<TKey, TValue> dict, TValue defaultValue)
			: base(dict)
		{
			_default = defaultValue;
		}

		public new TValue this[TKey key]
		{
			get
			{
				TValue t = _default;
				if (!base.TryGetValue(key, out t))
				{
					t = _default;
				}
				return t;
			}
			set
			{
				base[key] = DefaultValue;
			}
		}
	}

	public static class DictionaryHelper
	{
		public static DefaultDictionary<TKey, TValue> ToDefaultDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dict)
		{
			var defaultDict = new DefaultDictionary<TKey, TValue>(dict);
			return defaultDict;
		}

		public static DefaultDictionary<TKey, TValue> ToDefaultDictionary<TKey, TValue>(this Dictionary<TKey, TValue> dict, TValue defaultValue)
		{
			var defaultDict = new DefaultDictionary<TKey, TValue>(dict, defaultValue);
			return defaultDict;
		}

		public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
		{
			return new HashSet<T>(source);
		}
	}
}
