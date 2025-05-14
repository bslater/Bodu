// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfWeekInQuarter.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
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
		/// The input <see cref="DateTime" /> used to identify the calendar quarter. Its <see cref="DateTime.Kind" /> is preserved in the result.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate within the quarter. For example, <see cref="DayOfWeek.Monday" /> returns the last
		/// Monday in the quarter.
		/// </param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines how quarters are segmented.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the last occurrence of <paramref name="dayOfWeek" /> in the quarter that includes
		/// <paramref name="dateTime" />. The result is set to midnight (00:00:00) and preserves the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> or <paramref name="definition" /> is not a defined enumeration value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The result is calculated by first determining the end date of the quarter containing <paramref name="dateTime" /> (as defined by
		/// <paramref name="definition" />), and then finding the last occurrence of <paramref name="dayOfWeek" /> on or before that date.
		/// </para>
		/// <para>
		/// The returned value is guaranteed to fall within the same quarter as <paramref name="dateTime" />, and the time is normalized to 00:00:00.
		/// </para>
		/// </remarks>
		public static DateTime LastDayOfWeekInQuarter(this DateTime dateTime, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			var ticks = ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) + 7) % 7) * DateTimeExtensions.TicksPerDay;
			return new DateTime(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the given calendar quarter and year, using the
		/// provided <see cref="CalendarQuarterDefinition" /> to determine quarter boundaries.
		/// </summary>
		/// <param name="year">The calendar year in which the quarter occurs.</param>
		/// <param name="quarter">The 1-based quarter number (1 through 4).</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate within the quarter. For example, <see cref="DayOfWeek.Friday" /> returns the last
		/// Friday in the quarter.
		/// </param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines how quarters are segmented.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the last occurrence of <paramref name="dayOfWeek" /> within the specified quarter
		/// and year, as defined by <paramref name="definition" />. The result is set to midnight (00:00:00) and uses <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="quarter" /> is not between 1 and 4 inclusive, or if <paramref name="definition" /> or
		/// <paramref name="dayOfWeek" /> is not a valid enumeration value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The calculation begins from the last day of the quarter, as defined by <paramref name="definition" />, and steps backward to
		/// find the last occurrence of the specified <paramref name="dayOfWeek" />.
		/// </para>
		/// <para>The returned value is normalized to 00:00:00 (midnight) and does not include any time component.</para>
		/// </remarks>
		public static DateTime LastDayOfWeekInQuarter(int year, int quarter, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			var ticks = ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - DateTimeExtensions.GetDayOfWeekFromTicks(ticks) + 7) % 7) * DateTimeExtensions.TicksPerDay;
			return new DateTime(ticks, DateTimeKind.Unspecified);
		}
	}
}