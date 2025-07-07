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
		/// Returns a new <see cref="DateTime" /> representing the first day of the specified week number in the given calendar year, based
		/// on the week rule and first day of the week defined by the specified or current <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="year">The calendar year to evaluate. Must be between 1 and 9999, inclusive.</param>
		/// <param name="week">
		/// The culture-defined week number to evaluate, starting at 1. The maximum valid value depends on the
		/// <see cref="CalendarWeekRule" /> and <see cref="DayOfWeek" /> used by the specified <paramref name="culture" />.
		/// </param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to determine the <see cref="CalendarWeekRule" /> and starting
		/// <see cref="DayOfWeek" />. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first date of the specified week in the specified year.</returns>
		/// <remarks>
		/// <para>
		/// This method uses the culture-defined week numbering system. It begins by identifying the first occurrence of the culture�s
		/// defined <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> on or before January 1 of the specified year, then advances in 7-day
		/// intervals to calculate the start of the specified week.
		/// </para>
		/// <para>
		/// The result is validated by recalculating the week number for the computed date using
		/// <see cref="Calendar.GetWeekOfYear(DateTime, CalendarWeekRule, DayOfWeek)" /> and comparing it to <paramref name="week" />.
		/// </para>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance is <see cref="DateTimeKind.Unspecified" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="week" /> does not correspond to a valid week number for <paramref name="year" /> under the rules of
		/// the specified or current <paramref name="culture" />.
		/// </exception>
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