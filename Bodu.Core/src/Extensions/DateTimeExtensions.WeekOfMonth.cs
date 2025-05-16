// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="WeekOfMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the current culture's
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> settings.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <returns>An integer representing the week of the month in which <paramref name="dateTime" /> falls, starting at 1.</returns>
		/// <remarks>
		/// The calculation is based on the difference between the week of year for <paramref name="dateTime" /> and the week of year for
		/// the first day of that month, plus one. Week calculation respects <see cref="CultureInfo.CurrentCulture" />.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime)
		{
			var culture = CultureInfo.CurrentCulture;
			return GetWeekOfMonth(dateTime, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the calendar settings from the
		/// specified <paramref name="culture" />.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="culture">
		/// A <see cref="CultureInfo" /> that provides the calendar rule and week start definition. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>An integer representing the week of the month in which <paramref name="dateTime" /> falls, starting at 1.</returns>
		/// <remarks>
		/// This method uses the culture’s <see cref="CalendarWeekRule" /> and <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> to calculate
		/// the week of the month.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime, CultureInfo? culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			return GetWeekOfMonth(dateTime, culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
		}

		/// <summary>
		/// Returns the 1-based week number of the month for the specified <see cref="DateTime" />, using the given
		/// <paramref name="weekRule" /> and <paramref name="weekStart" /> values.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="weekRule">The rule used to determine the first week of the year.</param>
		/// <param name="weekStart">The first day of the week.</param>
		/// <returns>An integer representing the week of the month in which <paramref name="dateTime" /> falls, starting at 1.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekRule" /> or <paramref name="weekStart" /> is not a valid enum value.
		/// </exception>
		/// <remarks>
		/// This method compares the week of year for <paramref name="dateTime" /> and the first day of its month, using the provided
		/// calendar rule and starting day of the week.
		/// </remarks>
		public static int WeekOfMonth(this DateTime dateTime, CalendarWeekRule weekRule, DayOfWeek weekStart)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekRule);
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekStart);

			return GetWeekOfMonth(dateTime, weekRule, weekStart);
		}

		/// <summary>
		/// Calculates the 1-based week-of-month using raw tick data and specified calendar rules.
		/// </summary>
		private static int GetWeekOfMonth(DateTime dateTime, CalendarWeekRule weekRule, DayOfWeek weekStart) =>
			GetWeekOfYear(dateTime.Ticks, weekRule, weekStart)
				- GetWeekOfYear(GetTicksForDate(dateTime.Year, dateTime.Month, 1), weekRule, weekStart)
				+ 1;
	}
}