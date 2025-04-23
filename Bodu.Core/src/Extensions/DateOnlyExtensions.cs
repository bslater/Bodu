// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides a set of <see langword="static" /> ( <see langword="Shared" /> in Visual Basic)
	/// methods that extend the <see cref="System.DateOnly" /> class.
	/// </summary>
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the tick count that represent the date portion of the <see cref="System.DateOnly" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the date portion tick count.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the date of <paramref name="dateTime" />. The value
		/// is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long DateTicks(DateOnly dateTime)
			=> dateTime.Ticks - (dateTime.Ticks % DateTimeExtensions.TicksPerDay);

		/// <summary>
		/// Returns the tick count that represent the date portion of the specified <paramref name="ticks" />.
		/// </summary>
		/// <param name="ticks">
		/// The tick count to which to get the number of ticks representing the date.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the date of <paramref name="ticks" />. The value is
		/// between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long DateTicks(long ticks)
			=> ticks - (ticks % DateTimeExtensions.TicksPerDay);

		/// <summary>
		/// Returns the <see cref="System.DayOfWeek" /> for the specified date represented as ticks.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed
		/// since January 1, 0001 at 00:00:00.000 in the Gregorian calendar.
		/// </param>
		/// <returns>
		/// An enumerated constant that indicates the day of the week of the
		/// <paramref name="ticks" /> value.
		/// </returns>
		internal static DayOfWeek DayOfWeekFromTicks(long ticks)
			=> (DayOfWeek)(((ticks / DateTimeExtensions.TicksPerDay) + 1) % 7);

		/// <summary>
		/// Returns the tick count that represent a number of whole and fractional days.
		/// </summary>
		/// <param name="days">
		/// A number of whole and fractional days. The <paramref name="days" /> parameter can be
		/// negative or positive.
		/// </param>
		/// <returns>
		/// Returns the number of ticks that represent the number of whole and fractional days.
		/// </returns>
		internal static long DaysToTicks(double days)
			=> ((long)((days * (DateTimeExtensions.TicksPerDay / 10000)) + ((days >= 0.0) ? 0.5 : (-0.5)))) * 10000;

		/// <summary>
		/// Returns the year, month and day parts of the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the date parts of.
		/// </param>
		/// <param name="year">When this method returns, contains the year part of <paramref name="dateTime" />.</param>
		/// <param name="month">When this method returns, contains the month part of <paramref name="dateTime" />.</param>
		/// <param name="day">When this method returns, contains the day part of <paramref name="dateTime" />.</param>
		internal static void GetDatePart(DateOnly dateTime, out int year, out int month, out int day)
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

			// dayOfYear = n + 1; Leap year calculation looks different from IsLeapYear since y1,
			// y4, and y100 are relative to year 1, not year 0
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
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the next day of week.
		/// </param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the next day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified
		/// <paramref name="dateTime" /> and the specified next <paramref name="dayOfWeek" />. The
		/// value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long NextDayOfWeekTicks(DateOnly dateTime, DayOfWeek dayOfWeek)
			=> DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7);

		/// <summary>
		/// Returns the tick count between the <paramref name="dateTime" /> and previous <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the previous day of week.
		/// </param>
		/// <param name="dayOfWeek">
		/// An enumerated constant that indicates the previous day of the week.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified
		/// <paramref name="dateTime" /> and the specified previous <paramref name="dayOfWeek" />.
		/// The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		internal static long PreviousDayOfWeekTicks(DateOnly dateTime, DayOfWeek dayOfWeek)
			=> DateTimeExtensions.TicksPerDay * (((((int)dayOfWeek - (int)dateTime.DayOfWeek) - 7) % 7 + 7) % 7);

		/// <summary>
		/// Returns the tick count that represents the given year, month, and day.
		/// </summary>
		private static long DateToTicks(int year, int month, int day)
		{
			if (year >= 1 && year <= 9999 && month >= 1 && month <= 12)
			{
				int[] days = DateOnly.IsLeapYear(year) ? DateTimeExtensions.DaysToMonth366 : DateTimeExtensions.DaysToMonth365;
				if (day >= 1 && day <= days[month] - days[month - 1])
				{
					int y = year - 1;
					int n = (y * 365) + (y / 4) - (y / 100) + (y / 400) + days[month - 1] + day - 1;
					return n * TicksPerDay;
				}
			}

			throw new ArgumentOutOfRangeException(null, SR.Arg_OutOfRange_BadYearMonthDay);
		}

		/// <summary>
		/// Returns the tick count that represent the first day of the month for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the first day of the month.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the first day of the month of the
		/// <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long FirstDayOfTheMonthTicks(DateOnly dateTime)
			=> DateOnlyExtensions.DateToTicks(dateTime.Year, dateTime.Month, 1);

		/// <summary>
		/// Returns the tick count that represents the first day of week in the current month of the <see cref="System.DateOnly" />.
		/// </summary>
		private static long FirstDayOfWeekInTheMonthTicks(DateOnly dateTime, DayOfWeek dayOfWeek)
		{
			long ticks = DateOnlyExtensions.FirstDayOfTheMonthTicks(dateTime);
			ticks += ((dayOfWeek - DateOnlyExtensions.DayOfWeekFromTicks(ticks) + 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count that represent the last day of the month for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the last day of the month.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the last day of the month of the
		/// <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long LastDayOfTheMonthTicks(DateOnly dateTime)
			=> DateOnlyExtensions.DateToTicks(dateTime.Year, dateTime.Month, dateTime.DaysInMonth());

		/// <summary>
		/// Returns the tick count that represents the last day of week in the current month of the <see cref="System.DateOnly" />.
		/// </summary>
		private static long LastDayOfWeekInTheMonthTicks(DateOnly dateTime, DayOfWeek dayOfWeek)
		{
			long ticks = DateOnlyExtensions.LastDayOfTheMonthTicks(dateTime);
			ticks += ((dayOfWeek - DateOnlyExtensions.DayOfWeekFromTicks(ticks) - 7) % 7) * DateTimeExtensions.TicksPerDay;
			return ticks;
		}

		/// <summary>
		/// Returns the tick count between the date represented as <paramref name="ticks" /> and
		/// next <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed
		/// since January 1, 0001 at 00:00:00.000 in the Gregorian calendar.
		/// </param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the next day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified
		/// <paramref name="ticks" /> and the specified next <paramref name="dayOfWeek" />. The
		/// value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long NextDayOfWeekTicks(long ticks, DayOfWeek dayOfWeek)
		{
			DayOfWeek day = DateOnlyExtensions.DayOfWeekFromTicks(ticks);
			return DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)day + 7) % 7);
		}

		/// <summary>
		/// Returns the tick count between the date represented as <paramref name="ticks" /> and
		/// previous <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="ticks">
		/// A date and time expressed in the number of 100-nanosecond intervals that have elapsed
		/// since January 1, 0001 at 00:00:00.000 in the Gregorian calendar.
		/// </param>
		/// <param name="dayOfWeek">
		/// An enumerated constant that indicates the previous day of the week.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified
		/// <paramref name="ticks" /> and the specified previous <paramref name="dayOfWeek" />. The
		/// value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long PreviousDayOfWeekTicks(long ticks, DayOfWeek dayOfWeek)
		{
			DayOfWeek day = DateOnlyExtensions.DayOfWeekFromTicks(ticks);
			return DateTimeExtensions.TicksPerDay * (((int)dayOfWeek - (int)day - 7) % 7);
		}

		/// <summary>
		/// Returns the tick count that represent the time portion of the <see cref="System.DateOnly" />.
		/// </summary>
		/// <param name="dateTime">
		/// The <see cref="System.DateOnly" /> to which to get the previous day of week.
		/// </param>
		/// <returns>
		/// The number of ticks that represent the time portion of the specified
		/// <paramref name="dateTime" />. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long GetTimeTicks(DateOnly dateTime)
			=> dateTime.Ticks % DateTimeExtensions.TicksPerDay;

		/// <summary>
		/// Returns the tick count that represents the given hour, minute, second.
		/// </summary>
		/// <param name="hour">The hours (0 through 23).</param>
		/// <param name="minute">The minutes (0 through 59).</param>
		/// <param name="second">The seconds (0 through 59).</param>
		/// <returns>
		/// The number of ticks that represent the time for the specified <paramref name="hour" />,
		/// <paramref name="minute" /> and <paramref name="second" /> values. The value is between
		/// <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		private static long TimeToTicks(int hour, int minute, int second)
		{
			if (hour >= 0 && hour < 24 && minute >= 0 && minute < 60 && second >= 0 && second < 60)
			{
				int t = (hour * 3600) + (minute * 60) + second;
				return t * DateTimeExtensions.TicksPerSecond;
			}

			throw new ArgumentOutOfRangeException(null, SR.Arg_OutOfRange_BadHourMinuteSecond);
		}
	}
}