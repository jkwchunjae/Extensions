﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Extensions
{
	public static class DateTimeHelper
	{
		public static DateTime ToDate(this string str)
		{
			return new DateTime(str.Substring(0, 4).ToInt(), str.Substring(4, 2).ToInt(), str.Substring(6, 2).ToInt());
		}

		public static DateTime ToDate(this int value)
		{
			return new DateTime(value / 10000, (value / 100) % 100, value % 100);
		}

		public static int ToInt(this DateTime date)
		{
			return date.Year * 10000 + date.Month * 100 + date.Day;
		}

		public static int AddDays(this int date, int days)
		{
			return date.ToDate().AddDays(days).ToInt();
		}

		public static int Year(this int date)
		{
			return date / 10000;
		}

		public static int Month(this int date)
		{
			return (date / 100) % 100;
		}

		public static int Day(this int date)
		{
			return date % 100;
		}

		public static IEnumerable<int> DateRange(this int beginDate, int endDate, int step = 1)
		{
			int fromDate = beginDate;
			int toDate = endDate;

			while ((step >= 0 ? (fromDate <= toDate) : (fromDate >= toDate)))
			{
				yield return fromDate;
				fromDate = fromDate.AddDays(step);
			}
		}
	}
}
