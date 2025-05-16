// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="GetStartDateOfWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the <see cref="DateTime" /> corresponding to the first day of the specified week number in the given calendar year,
		/// based on the week rule and first day of the week defined by the specified or current <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="week">
		/// The 1-based week number to evaluate. Valid values range from 1 to 53, depending on the culture-specific
		/// <see cref="CalendarWeekRule" /> and starting <see cref="DayOfWeek" />.
		/// </param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to determine the <see cref="DateTimeFormatInfo.CalendarWeekRule" /> and
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> for calculating week boundaries. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00.000) on the first day of the specified week in the given year, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="week" /> does not correspond to a valid week number for the specified <paramref name="year" />, based
		/// on the <paramref name="culture" />'s <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method aligns with the culture-defined week numbering system. It calculates the anchor point by identifying the first
		/// occurrence of the culture's defined <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> on or before January 1 of the given year,
		/// then advances in 7-day intervals to reach the start of the specified week.
		/// </para>
		/// <para>
		/// The result is validated by recalculating the week number using
		/// <see cref="Calendar.GetWeekOfYear(DateTime, CalendarWeekRule, DayOfWeek)" /> and comparing it to <paramref name="week" />.
		/// </para>
		/// </remarks>
		public static DateTime GetStartDateOfWeek(int year, int week, CultureInfo? culture = null)
		{
			DateTimeFormatInfo dfi = (culture ?? CultureInfo.CurrentCulture).DateTimeFormat;

			// Compute ticks for the first day of the year
			long ticks = GetTicksForDate(year, 1, 1);

			// Compute the ticks from the first week start
			ticks += GetPreviousDayOfWeekTicksFrom(ticks, dfi.FirstDayOfWeek)
				+ ((week - 1) * 7L * TimeSpan.TicksPerDay);

			// Validate week number
			int resultWeek = GetWeekOfYear(ticks, dfi.CalendarWeekRule, dfi.FirstDayOfWeek);
			if (resultWeek != week)
				throw new ArgumentOutOfRangeException(nameof(week),
					string.Format(ResourceStrings.Arg_OutOfRange_WeekNotValidForYearAndCulture, week, year, (culture ?? CultureInfo.CurrentCulture).Name));

			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}