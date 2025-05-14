// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.WeekOfMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the current culture's calendar
		/// week rule and first day of the week.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <remarks>
		/// This method uses the <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> defined by
		/// <see cref="CultureInfo.CurrentCulture" />. The result is based on the difference in week-of-year values between the input date
		/// and the first day of its month, plus one.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime)
		{
			var culture = CultureInfo.CurrentCulture;
			return dateTime.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the week rule and first day of
		/// week defined by the given <paramref name="culture" />.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="culture">
		/// The culture providing the calendar rule and week start. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <remarks>
		/// The week number is determined by comparing the week of year for the input date and the first day of the same month, based on the
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> in the specified culture.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime, CultureInfo? culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			return dateTime.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the specified
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> start day.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="weekRule">The calendar rule that defines how the first week of the year is determined.</param>
		/// <param name="weekStart">The day considered the start of the week.</param>
		/// <returns>The 1-based week number of the month for the specified date.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekRule" /> or <paramref name="weekStart" /> is not a defined enum value.
		/// </exception>
		/// <remarks>
		/// The method compares the week of year for the input date and the first day of its month to determine the week-of-month value. The
		/// result is always 1-based and sensitive to the chosen calendar rule and week start.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime, CalendarWeekRule weekRule, DayOfWeek weekStart)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekRule);
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekStart);

			return GetWeekOfYear(dateTime.Ticks, weekRule, weekStart)
				- GetWeekOfYear(GetTicksForDate(dateTime.Year, dateTime.Month, 1), weekRule, weekStart)
				+ 1;
		}
	}
}