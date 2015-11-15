using System;
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

		/// <summary> yyyymmdd hhmmss 형태의 str 을 DateTime으로 변환한다.  </summary>
		public static DateTime ToDateTime(this string str)
		{
			return new DateTime(
				str.Substring(0, 4).ToInt(),
				str.Substring(4, 2).ToInt(),
				str.Substring(6, 2).ToInt(),
				str.Substring(9, 2).ToInt(),
				str.Substring(11, 2).ToInt(),
				str.Substring(13, 2).ToInt());
		}

		public static int ToInt(this DateTime date)
		{
			return date.Year * 10000 + date.Month * 100 + date.Day;
		}

		public static int AddDays(this int date, int days)
		{
			return date.ToDate().AddDays(days).ToInt();
		}

		public static int AddWeeks(this int date, int weeks)
		{
			return date.AddDays(7 * weeks);
		}

		public static int AddMonths(this int date, int months)
		{
			return date.ToDate().AddMonths(months).ToInt();
		}

		public static int AddYears(this int date, int years)
		{
			return date.ToDate().AddYears(years).ToInt();
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

		public static int GetDate(this int date)
		{
			if (date < 20000000)
				return DateTime.Today.AddDays(date).ToInt();
			return date;
		}

		public static bool IsBetween(this int date, int beginDate, int endDate)
		{
			return date >= beginDate && date <= endDate;
		}
	}
}
