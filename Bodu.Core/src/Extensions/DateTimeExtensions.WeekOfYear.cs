// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.WeekOfYear.cs" company="PlaceholderCompany">
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
		/// Returns the week number (1–53) of the year that contains the specified <see cref="DateTime" />, using the current culture's
		/// calendar and week rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>The culture-specific week number of the year that contains <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses the <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> defined in
		/// <see cref="CultureInfo.CurrentCulture" />. Week numbering may vary across cultures (e.g., U.S. vs ISO 8601).
		/// </remarks>
		public static int WeekOfYear(this DateTime dateTime)
			=> dateTime.WeekOfYear(null);

		/// <summary>
		/// Returns the week number (1–53) of the year that contains the specified <see cref="DateTime" />, using the given <paramref name="culture" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="culture">
		/// The <see cref="CultureInfo" /> used to determine the calendar, week rule, and first day of the week. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>The culture-specific week number of the year that contains <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses <see cref="DateTimeFormatInfo.CalendarWeekRule" /> and <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> from
		/// the specified culture to compute the result.
		/// </remarks>
		public static int WeekOfYear(this DateTime dateTime, CultureInfo? culture)
		{
			var info = (culture ?? Thread.CurrentThread.CurrentCulture).DateTimeFormat;
			return GetWeekOfYear(dateTime.Ticks, info.CalendarWeekRule, info.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the week number (1–53) of the year that contains the specified <see cref="DateTime" />, using the specified
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> start day.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="weekRule">The calendar rule that defines how the first week of the year is determined.</param>
		/// <param name="weekStart">The day considered the start of the week.</param>
		/// <returns>A 1-based integer representing the week number of the year that contains the specified date.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekRule" /> or <paramref name="weekStart" /> is not a defined enumeration value.
		/// </exception>
		/// <remarks>
		/// The result may vary depending on the rule and starting day. For example, ISO 8601 uses <c>FirstFourDayWeek</c> and <c>Monday</c>.
		/// </remarks>
		public static int WeekOfYear(this DateTime dateTime, CalendarWeekRule weekRule, DayOfWeek weekStart)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekRule);
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekStart);

			return GetWeekOfYear(dateTime.Ticks, weekRule, weekStart);
		}
	}
}