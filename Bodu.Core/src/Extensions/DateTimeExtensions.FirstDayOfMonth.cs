// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the first day of the same month and year as the specified value.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose year and month components are used.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the month, preserving the <see cref="DateTime.Kind" />
		/// of the input.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The time component of the returned value is normalized to midnight (00:00:00). The <see cref="DateTime.Kind" /> is copied from
		/// the input.
		/// </para>
		/// </remarks>
		public static DateTime FirstDayOfMonth(this DateTime dateTime) =>
			new(GetFirstDayOfMonthTicks(dateTime), dateTime.Kind);

		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the first day of the specified month and year.
		/// </summary>
		/// <param name="year">
		/// The calendar year component of the date. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="month">
		/// The month component of the date. Must be an integer between 1 and 12, inclusive, where 1 represents January and 12 represents December.
		/// </param>
		/// <returns>A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the specified month and year, with <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <remarks>
		/// <para>
		/// The time component of the returned value is set to midnight (00:00:00), and the <see cref="DateTime.Kind" /> is explicitly <see cref="DateTimeKind.Unspecified" />.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is less than the <c>Year</c> of <see cref="DateTime.MinValue" /> or greater than the
		/// <c>Year</c> of <see cref="DateTime.MaxValue" />, or if <paramref name="month" /> is outside the valid range of 1 to 12.
		/// </exception>
		public static DateTime GetFirstDayOfMonth(int year, int month)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);

			return new(GetTicksForDate(year, month, 1), DateTimeKind.Unspecified);
		}
	}
}