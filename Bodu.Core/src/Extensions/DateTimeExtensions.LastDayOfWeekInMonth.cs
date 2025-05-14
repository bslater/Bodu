// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfWeekInMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
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
		/// The <see cref="DayOfWeek" /> value to locate. For example, <see cref="DayOfWeek.Friday" /> will return the last Friday of the month.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> set to midnight on the last occurrence of the specified <paramref name="dayOfWeek" /> in the same
		/// month and year as <paramref name="dateTime" />, with the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a defined value in the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// The search starts from the last day of the month and proceeds backward until the specified <paramref name="dayOfWeek" /> is
		/// found. The result is normalized to midnight (00:00:00), and the original <see cref="DateTime.Kind" /> is retained.
		/// </remarks>
		public static DateTime LastDayOfWeekInMonth(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateTimeExtensions.GetLastDayOfWeekInMonthAsTicks(dateTime, dayOfWeek);
			return new DateTime(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the specified <paramref name="year" /> and <paramref name="month" />.
		/// </summary>
		/// <param name="year">The year component of the month to search. Must be between <see cref="DateTime.MinValue" /> and <see cref="DateTime.MaxValue" />.</param>
		/// <param name="month">The month component of the year to search (1–12).</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> value to locate. For example, <see cref="DayOfWeek.Friday" /> will return the last Friday of the
		/// specified month.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> set to midnight on the last occurrence of the specified <paramref name="dayOfWeek" /> in the given
		/// <paramref name="year" /> and <paramref name="month" />, with <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> or <paramref name="month" /> is outside the valid range, or if <paramref name="dayOfWeek" />
		/// is not a defined value in the <see cref="DayOfWeek" /> enumeration.
		/// </exception>
		/// <remarks>
		/// The search begins from the last day of the specified month and proceeds backward until the specified
		/// <paramref name="dayOfWeek" /> is found. The resulting <see cref="DateTime" /> has its time component set to midnight and kind
		/// set to <see cref="DateTimeKind.Unspecified" />.
		/// </remarks>
		public static DateTime LastDayOfWeekInMonth(int year, int month, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTimeExtensions.MinYear, DateTimeExtensions.MaxYear);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateTimeExtensions.GetLastDayOfWeekInMonth(DateTimeExtensions.GetTicksForDate(year, month, DateTime.DaysInMonth(year, month)), dayOfWeek);
			return new DateTime(ticks, DateTimeKind.Unspecified);
		}
	}
}