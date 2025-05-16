// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfWeekInQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the quarter that contains the specified
		/// <see cref="DateTime" />, using the provided <see cref="CalendarQuarterDefinition" /> to determine the quarter boundaries.
		/// </summary>
		/// <param name="dateTime">
		/// The input <see cref="DateTime" /> used to identify the calendar quarter. The returned result preserves its <see cref="DateTime.Kind" />.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate within the quarter. For example, <see cref="DayOfWeek.Monday" /> returns the last
		/// Monday in the quarter.
		/// </param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines how quarters are segmented.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to 00:00:00 on the last occurrence of <paramref name="dayOfWeek" /> in the quarter that
		/// includes <paramref name="dateTime" />, preserving the input <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> or <paramref name="dayOfWeek" /> is not a valid enumeration value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method calculates the result by determining the last day of the quarter containing <paramref name="dateTime" /> as defined
		/// by <paramref name="definition" />, then stepping backward to the most recent occurrence of <paramref name="dayOfWeek" />.
		/// </para>
		/// <para>The returned value is normalized to midnight and guaranteed to fall within the same quarter as <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime LastDayOfWeekInQuarter(this DateTime dateTime, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			var ticks = ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - GetDayOfWeekFromTicks(ticks) + 7) % 7) * TicksPerDay;
			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the given calendar quarter and year, using the
		/// provided <see cref="CalendarQuarterDefinition" /> to determine quarter boundaries.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="quarter">
		/// The quarter number to evaluate. Must be an integer between 1 and 4, inclusive, where 1 represents the first quarter and 4
		/// represents the final quarter.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate. For example, specifying <see cref="DayOfWeek.Friday" /> will return the last
		/// Friday within the specified quarter.
		/// </param>
		/// <param name="definition">
		/// The <see cref="CalendarQuarterDefinition" /> value that defines how quarters are segmented within the year (e.g.,
		/// calendar-aligned or fiscal-aligned quarters).
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to 00:00:00 on the last occurrence of <paramref name="dayOfWeek" /> in the specified
		/// <paramref name="quarter" /> of <paramref name="year" />, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported <see cref="DateTime" /> year range, if <paramref name="quarter" />
		/// is not between 1 and 4, or if <paramref name="dayOfWeek" /> or <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The result is computed by determining the last day of the specified quarter using <paramref name="definition" />, then iterating
		/// backward to find the most recent occurrence of the specified <paramref name="dayOfWeek" />.
		/// </para>
		/// <para>The returned value is normalized to midnight (00:00:00) and uses <see cref="DateTimeKind.Unspecified" />.</para>
		/// </remarks>
		public static DateTime LastDayOfWeekInQuarter(int year, int quarter, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			var ticks = ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - GetDayOfWeekFromTicks(ticks) + 7) % 7) * TicksPerDay;
			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}