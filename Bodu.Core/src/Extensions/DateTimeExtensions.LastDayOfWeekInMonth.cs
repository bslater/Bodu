// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfWeekInMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the same month and year as the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose month and year define the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate. For example, <see cref="DayOfWeek.Friday" /> returns the last Friday of the month.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the last occurrence of the specified <paramref name="dayOfWeek" /> in
		/// the same month and year as <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid member of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The search begins at the last day of the month and moves backward to find the most recent occurrence of the specified
		/// <paramref name="dayOfWeek" />. The result is normalized to midnight (00:00:00), and the <see cref="DateTime.Kind" /> is preserved.
		/// </para>
		/// </remarks>
		public static DateTime LastDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = GetLastDayOfWeekInMonthAsTicks(dateTime, dayOfWeek);
			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the specified <paramref name="year" /> and <paramref name="month" />.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="month">
		/// The calendar month to evaluate. Must be an integer between 1 and 12, inclusive, where 1 represents January and 12 represents December.
		/// </param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, specifying <see cref="DayOfWeek.Monday" /> will return the last Monday in
		/// the specified month.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the last occurrence of the specified <paramref name="dayOfWeek" /> in
		/// the given <paramref name="year" /> and <paramref name="month" />, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is less than the <c>Year</c> of <see cref="DateTime.MinValue" /> or greater than the
		/// <c>Year</c> of <see cref="DateTime.MaxValue" />, if <paramref name="month" /> is not between 1 and 12, or if
		/// <paramref name="dayOfWeek" /> is not a defined value of the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method starts from the last calendar day of the specified month and iterates backward to find the most recent occurrence of
		/// the specified <paramref name="dayOfWeek" />. The returned <see cref="DateTime" /> is normalized to midnight (00:00:00) and has a
		/// <see cref="DateTimeKind" /> of <see cref="DateTimeKind.Unspecified" />.
		/// </para>
		/// </remarks>
		public static DateTime GetLastDayOfWeekInMonth(int year, int month, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfOutOfRange(year, MinYear, MaxYear);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = GetLastDayOfWeekInMonth(GetTicksForDate(year, month, DateTime.DaysInMonth(year, month)), dayOfWeek);
			return new(ticks, DateTimeKind.Unspecified);
		}
	}
}