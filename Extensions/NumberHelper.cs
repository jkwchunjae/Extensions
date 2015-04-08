﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class NumberHelper
	{
		public static string ToComma(this int value)
		{
			return "{0:#,#}".With(value);
		}
	}

	public static class StaticRandom
	{
		public static Random random = new Random((int)DateTime.Now.Ticks);

		public static int Next(int maxValue)
		{
			return random.Next(maxValue);
		}

		public static int Next(int minValue, int maxValue)
		{
			return random.Next(minValue, maxValue);
		}
	}
}
