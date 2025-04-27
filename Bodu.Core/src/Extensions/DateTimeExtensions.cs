// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides a set of <see langword="static" /> ( <see langword="Shared" /> in Visual Basic) methods that extend the
	/// <see cref="System.DateTime" /> class.
	/// </summary>
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Represents a small threshold used for comparing floating-point numbers where exact equality is unreliable due to binary
		/// precision. This field is constant.
		/// </summary>
		internal const double Epsilon = 1e-10;

		/// <summary>
		/// The minimum year supported by <see cref="DateTime" />.
		/// </summary>
		internal const int MinYear = 1;

		/// <summary>
		/// The maximum year supported by <see cref="DateTime" />.
		/// </summary>
		internal const int MaxYear = 9999;

		/// <summary>
		/// Represents the number of milliseconds in 1 day. This field is constant.
		/// </summary>
		internal const int MillisecondsPerDay = DateTimeExtensions.MillisecondsPerHour * 24;

		/// <summary>
		/// Represents the number of milliseconds in 1 hour. This field is constant.
		/// </summary>
		internal const int MillisecondsPerHour = DateTimeExtensions.MillisecondsPerMinute * 60;

		/// <summary>
		/// Represents the number of milliseconds in 1 minute. This field is constant.
		/// </summary>
		internal const int MillisecondsPerMinute = DateTimeExtensions.MillisecondsPerSecond * 60;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 second. This field is constant.
		/// </summary>
		internal const int MillisecondsPerSecond = 1000;

		/// <summary>
		/// Represents the number of ticks (100ns) in 30 days. This field is constant.
		/// </summary>
		internal const long TicksPer30Days = DateTimeExtensions.TicksPerDay * 30;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 day. This field is constant.
		/// </summary>
		internal const long TicksPerDay = DateTimeExtensions.TicksPerHour * 24;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 hour. This field is constant.
		/// </summary>
		internal const long TicksPerHour = DateTimeExtensions.TicksPerMinute * 60;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 millisecond. This field is constant.
		/// </summary>
		internal const long TicksPerMillisecond = 10000;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 minute. This field is constant.
		/// </summary>
		internal const long TicksPerMinute = DateTimeExtensions.TicksPerSecond * 60;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 second. This field is constant.
		/// </summary>
		internal const long TicksPerSecond = DateTimeExtensions.TicksPerMillisecond * 1000;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 week (7 days). This field is constant.
		/// </summary>
		internal const long TicksPerWeek = DateTimeExtensions.TicksPerDay * 7;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 year. This field is constant.
		/// </summary>
		internal const long TicksPerYear = DateTimeExtensions.TicksPerDay * DaysPerYear;

		/// <summary>
		/// Represents the number of days in 100 years. This field is constant.
		/// </summary>
		private const int DaysPer100Years = (DateTimeExtensions.DaysPer4Years * 25) - 1;

		/// <summary>
		/// Represents the number of days in 400 years. This field is constant.
		/// </summary>
		private const int DaysPer400Years = (DateTimeExtensions.DaysPer100Years * 4) + 1;

		/// <summary>
		/// Represents the number of days in 4 years. This field is constant.
		/// </summary>
		private const int DaysPer4Years = (DateTimeExtensions.DaysPerYear * 4) + 1;

		/// <summary>
		/// Represents the number of days a non-leap year. This field is constant.
		/// </summary>
		private const int DaysPerYear = 365;

		/// <summary>
		/// Represents the number of days from 1-Jan-0001 to 31-Dec-9999. This field is constant.
		/// </summary>
		private const int DaysTo10000 = (DateTimeExtensions.DaysPer400Years * 25) - 366;

		/// <summary>
		/// Represents the number of days from 1-Jan-0001 to 31-Dec-1969. This field is constant.
		/// </summary>
		private const int DaysTo1970 = (DateTimeExtensions.DaysPer400Years * 4) + (DateTimeExtensions.DaysPer100Years * 3) + (DateTimeExtensions.DaysPer4Years * 17) + DateTimeExtensions.DaysPerYear;

		/// <summary>
		/// Represents the maximum number of milliseconds. This field is constant.
		/// </summary>
		private const long MaxMilliseconds = (long)DateTimeExtensions.DaysTo10000 * DateTimeExtensions.MillisecondsPerDay;

		/// <summary>
		/// Represents the maximum number of ticks (100ns). This field is constant.
		/// </summary>
		private const long MaxTicks = (DateTimeExtensions.DaysTo10000 * DateTimeExtensions.TicksPerDay) - 1;

		/// <summary>
		/// Represents the minimum number of milliseconds. This field is constant.
		/// </summary>
		private const long MinMilliseconds = 0;

		/// <summary>
		/// Represents the minimum number of ticks (100ns). This field is constant.
		/// </summary>
		private const long MinTicks = 0;

		private static readonly int[] DaysToMonth365 = { 0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365 };

		private static readonly int[] DaysToMonth366 = { 0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366 };

		/// <summary>
		/// Returns the tick count that represent the date portion of the <see cref="System.DateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the date portion tick count.</param>
		/// <returns>
		/// The number of ticks that represent the date of <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long GetTicks(DateTime dateTime)
			=> dateTime.Ticks - (dateTime.Ticks % DateTimeExtensions.TicksPerDay);

		/// <summary>
		/// Returns the tick count that represent the date portion of the specified <paramref name="ticks" />.
		/// </summary>
		/// <param name="ticks">The tick count to which to get the number of ticks representing the date.</param>
		/// <returns>
		/// The number of ticks that represent the date of <paramref name="ticks" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long GetDateAsTicks(long ticks)
			=> ticks - (ticks % DateTimeExtensions.TicksPerDay);

		/// <summary>
		/// Returns the <see cref="System.DayOfWeek" /> for the specified date represented as ticks.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed since January 1, 0001 at 00:00:00.000 in
		/// the Gregorian calendar.
		/// </param>
		/// <returns>An enumerated constant that indicates the day of the week of the <paramref name="ticks" /> value.</returns>
		internal static DayOfWeek GetDayOfWeekFromTicks(long ticks)
			=> (DayOfWeek)(((ticks / DateTimeExtensions.TicksPerDay) + 1) % 7);

		/// <summary>
		/// Returns the tick count that represent a number of whole and fractional days.
		/// </summary>
		/// <param name="days">A number of whole and fractional days. The <paramref name="days" /> parameter can be negative or positive.</param>
		/// <returns>Returns the number of ticks that represent the number of whole and fractional days.</returns>
		internal static long GetDaysToTicks(double days)
			=> ((long)((days * (DateTimeExtensions.TicksPerDay / 10000)) + ((days >= 0.0) ? 0.5 : (-0.5)))) * 10000;

		/// <summary>
		/// Returns the year, month and day parts of the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the date parts of.</param>
		/// <param name="year">When this method returns, contains the year part of <paramref name="dateTime" />.</param>
		/// <param name="month">When this method returns, contains the month part of <paramref name="dateTime" />.</param>
		/// <param name="day">When this method returns, contains the day part of <paramref name="dateTime" />.</param>
		internal static void GetDateParts(DateTime dateTime, out int year, out int month, out int day)
		{
			long ticks = dateTime.Ticks;

			// n = number of days since 1/1/0001
			int n = (int)(ticks / TicksPerDay);

			// y400 = number of whole 400-year periods since 1/1/0001
			int y400 = n / DateTimeExtensions.DaysPer400Years;

			// n = day number within 400-year period
			n -= y400 * DateTimeExtensions.DaysPer400Years;

			// y100 = number of whole 100-year periods within 400-year period
			int y100 = n / DateTimeExtensions.DaysPer100Years;

			// Last 100-year period has an extra day, so decrement result if 4
			if (y100 == 4) y100 = 3;

			// n = day number within 100-year period
			n -= y100 * DateTimeExtensions.DaysPer100Years;

			// y4 = number of whole 4-year periods within 100-year period
			int y4 = n / DateTimeExtensions.DaysPer4Years;

			// n = day number within 4-year period
			n -= y4 * DateTimeExtensions.DaysPer4Years;

			// y1 = number of whole years within 4-year period
			int y1 = n / DateTimeExtensions.DaysPerYear;

			// Last year has an extra day, so decrement result if 4
			if (y1 == 4) y1 = 3;

			// compute year
			year = (y400 * 400) + (y100 * 100) + (y4 * 4) + y1 + 1;

			// n = day number within year
			n -= y1 * DateTimeExtensions.DaysPerYear;

			// dayOfYear = n + 1; Leap year calculation looks different from IsLeapYear since y1, y4, and y100 are relative to year 1, not
			// year 0
			bool leapYear = y1 == 3 && (y4 != 24 || y100 == 3);
			int[] days = leapYear ? DateTimeExtensions.DaysToMonth366 : DateTimeExtensions.DaysToMonth365;

			// All months have less than 32 days, so n >> 5 is a good conservative estimate for the month
			int m = (n >> 5) + 1;

			// m = 1-based month number
			while (n >= days[m]) m++;

			// compute month and day
			month = m;
			day = n - days[m - 1] + 1;
		}

		/// <summary>
		/// Returns the tick count between the <paramref name="dateTime" /> and next <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the next day of week.</param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the next day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified <paramref name="dateTime" /> and the
		/// specified next <paramref name="dayOfWeek" />. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long GetNextDayOfWeekAsTicks(DateTime dateTime, DayOfWeek dayOfWeek)
			=> DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7);

		/// <summary>
		/// Returns the tick count between the <paramref name="dateTime" /> and previous <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the previous day of week.</param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the previous day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified <paramref name="dateTime" /> and the
		/// specified previous <paramref name="dayOfWeek" />. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long GetPreviousDayOfWeekAsTicks(DateTime dateTime, DayOfWeek dayOfWeek)
			=> DateTimeExtensions.TicksPerDay * (((((int)dayOfWeek - (int)dateTime.DayOfWeek) - 7) % 7 - 7) % 7);

		/// <summary>
		/// Returns the tick count that represents the given year, month, and day.
		/// </summary>
		internal static long GetTicksForDate(int year, int month, int day)
		{
			if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
			{
				int[] days = DateTime.IsLeapYear(year) ? DateTimeExtensions.DaysToMonth366 : DateTimeExtensions.DaysToMonth365;
				if (day >= 1 && day <= days[month] - days[month - 1])
				{
					int y = year - 1;
					int n = (y * 365) + (y / 4) - (y / 100) + (y / 400) + days[month - 1] + day - 1;
					return n * TicksPerDay;
				}
			}

			throw new ArgumentOutOfRangeException(null, ResourceStrings.Arg_OutOfRange_BadYearMonthDay);
		}

		/// <summary>
		/// Returns the tick count that represent the first day of the month for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the first day of the month.</param>
		/// <returns>
		/// The number of ticks that represent the first day of the month of the <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetFirstDayOfMonthTicks(DateTime dateTime)
			=> DateTimeExtensions.GetTicksForDate(dateTime.Year, dateTime.Month, 1);

		/// <summary>
		/// Returns the tick count that represents the first day of week in the current month of the <see cref="System.DateTime" />.
		/// </summary>
		private static long GetFirstDayOfWeekInMonthTicks(DateTime dateTime, DayOfWeek dayOfWeek)
		{
			long ticks = DateTimeExtensions.GetFirstDayOfMonthTicks(dateTime);
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) + 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count that represents the first day of week in the current month of the <see cref="System.DateTime" />.
		/// </summary>
		private static long GetFirstDayOfWeekInMonthTicks(int year, int month, DayOfWeek dayOfWeek)
		{
			long ticks = DateTimeExtensions.GetTicksForDate(year, month, 1);
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) + 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count that represent the last day of the month for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the last day of the month.</param>
		/// <returns>
		/// The number of ticks that represent the last day of the month of the <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetLastDayOfMonthTicks(DateTime dateTime)
			=> DateTimeExtensions.GetTicksForDate(dateTime.Year, dateTime.Month, dateTime.DaysInMonth());

		/// <summary>
		/// Returns the tick count that represents the last day of week in the current month of the <see cref="System.DateTime" />.
		/// </summary>
		private static long GetLastDayOfWeekInMonthAsTicks(DateTime dateTime, DayOfWeek dayOfWeek)
		{
			long ticks = DateTimeExtensions.GetLastDayOfMonthTicks(dateTime);
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) - 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count that represents the last day of week in the current month of the <see cref="System.DateTime" />.
		/// </summary>
		private static long GetLastDayOfWeekInMonthAsTicks(int year, int month, DayOfWeek dayOfWeek)
		{
			long ticks = DateTimeExtensions.GetTicksForDate(year, month, DateTime.DaysInMonth(year, month));
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) - 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count between the date represented as <paramref name="ticks" /> and next <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed since January 1, 0001 at 00:00:00.000 in
		/// the Gregorian calendar.
		/// </param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the next day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified <paramref name="ticks" /> and the
		/// specified next <paramref name="dayOfWeek" />. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetNextDayOfWeekTicksFrom(long ticks, DayOfWeek dayOfWeek)
		{
			DayOfWeek day = DateTimeExtensions.GetDayOfWeekFromTicks(ticks);
			return DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)day + 7) % 7);
		}

		/// <summary>
		/// Returns the tick count between the date represented as <paramref name="ticks" /> and previous <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed since January 1, 0001 at 00:00:00.000 in
		/// the Gregorian calendar.
		/// </param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the previous day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified <paramref name="ticks" /> and the
		/// specified previous <paramref name="dayOfWeek" />. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetPreviousDayOfWeekTicksFrom(long ticks, DayOfWeek dayOfWeek)
		{
			DayOfWeek day = DateTimeExtensions.GetDayOfWeekFromTicks(ticks);
			return DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)day - 7) % 7);
		}

		/// <summary>
		/// Returns the tick count that represent the time portion of the <see cref="System.DateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the previous day of week.</param>
		/// <returns>
		/// The number of ticks that represent the time portion of the specified <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetTimeTicks(DateTime dateTime)
			=> dateTime.Ticks % DateTimeExtensions.TicksPerDay;

		/// <summary>
		/// Returns the tick count that represents the given hour, minute, second.
		/// </summary>
		/// <param name="hour">The hours (0 through 23).</param>
		/// <param name="minute">The minutes (0 through 59).</param>
		/// <param name="second">The seconds (0 through 59).</param>
		/// <returns>
		/// The number of ticks that represent the time for the specified <paramref name="hour" />, <paramref name="minute" /> and
		/// <paramref name="second" /> values. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetTicksForTime(int hour, int minute, int second)
		{
			if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60)
			{
				int t = (hour * 3600) + (minute * 60) + second;
				return t * DateTimeExtensions.TicksPerSecond;
			}

			throw new ArgumentOutOfRangeException(null, ResourceStrings.Arg_OutOfRange_BadHourMinuteSecond);
		}
	}
}
