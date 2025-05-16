// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfWeekInQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> within the quarter that includes the given
		/// <see cref="DateTime" />, based on the specified <see cref="CalendarQuarterDefinition" />.
		/// </summary>
		/// <param name="dateTime">
		/// The reference <see cref="DateTime" /> used to identify the containing quarter. The <see cref="DateTime.Kind" /> is preserved in
		/// the result.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate within the quarter. For example, <see cref="DayOfWeek.Monday" /> returns the first Monday
		/// in the quarter.
		/// </param>
		/// <param name="definition">
		/// The <see cref="CalendarQuarterDefinition" /> used to determine the quarter boundaries (e.g., Calendar Year, Fiscal Year).
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing midnight (00:00:00) on the first occurrence of <paramref name="dayOfWeek" /> within the
		/// quarter containing <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The method calculates the start date of the quarter based on <paramref name="definition" /> and advances forward to the first
		/// matching <paramref name="dayOfWeek" />. The result is guaranteed to fall within the same quarter as <paramref name="dateTime" />.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> or <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		public static DateTime FirstDayOfWeekInQuarter(this DateTime dateTime, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			var ticks = ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - GetDayOfWeekFromTicks(ticks) + 7) % 7) * TicksPerDay;
			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the first occurrence of the specified <see cref="DayOfWeek" /> within the given quarter and year, using the specified
		/// <see cref="CalendarQuarterDefinition" /> to determine the quarter boundaries.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="quarter">
		/// The quarter number within the year. Must be an integer between 1 and 4, inclusive, where 1 represents the first quarter and 4
		/// represents the final quarter.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate within the quarter. For example, specifying <see cref="DayOfWeek.Friday" /> will return
		/// the first Friday on or after the start of the quarter.
		/// </param>
		/// <param name="definition">
		/// The <see cref="CalendarQuarterDefinition" /> that defines how the quarters are segmented within the year (e.g.,
		/// <see cref="CalendarQuarterDefinition.JanuaryDecember" /> or <see cref="CalendarQuarterDefinition.AprilToMarch" />).
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing midnight (00:00:00) on the first occurrence of <paramref name="dayOfWeek" /> within the
		/// specified quarter, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The calculation begins from the first day of the quarter as defined by <paramref name="definition" /> and advances forward to
		/// the first occurrence of the specified <paramref name="dayOfWeek" />.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported <see cref="DateTime" /> year range, if <paramref name="quarter" />
		/// is not between 1 and 4, or if <paramref name="dayOfWeek" /> or <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		public static DateTime FirstDayOfWeekInQuarter(int year, int quarter, DayOfWeek dayOfWeek, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			var ticks = ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition));
			ticks += ((dayOfWeek - GetDayOfWeekFromTicks(ticks) + 7) % 7) * TicksPerDay;
			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}