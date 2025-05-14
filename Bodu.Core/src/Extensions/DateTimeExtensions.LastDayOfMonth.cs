// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last day of the same month and year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose month and year are used to determine the last day.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the last day of the month, with the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// This method uses the rules of the Gregorian calendar to determine the number of days in the month, including leap year behavior
		/// for February. The time component is normalized to midnight (00:00:00).
		/// </remarks>
		public static DateTime LastDayOfMonth(this DateTime dateTime)
			=> new(DateTimeExtensions.GetLastDayOfMonthTicks(dateTime), dateTime.Kind);

		/// <summary>
		/// Returns a <see cref="DateTime" /> representing midnight on the last day of the specified year and month.
		/// </summary>
		/// <param name="year">The year component of the date. Must be between 1 and 9999, inclusive.</param>
		/// <param name="month">The month component of the date. Must be between 1 and 12, inclusive.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to 00:00:00 on the last day of the specified month and year, with
		/// <see cref="DateTimeKind.Unspecified" /> as its kind.
		/// </returns>
		/// <remarks>
		/// The method calculates the last day of the month using the Gregorian calendar rules, accounting for leap years. The returned
		/// <see cref="DateTime" /> always has the time component set to midnight and the kind set to <see cref="DateTimeKind.Unspecified" />.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the valid range, or if <paramref name="month" /> is not between 1 and 12.
		/// </exception>
		public static DateTime LastDayOfMonth(int year, int month)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);

			return new DateTime(year, month, DateTime.DaysInMonth(year, month), 0, 0, 0, DateTimeKind.Unspecified);
		}
	}
}