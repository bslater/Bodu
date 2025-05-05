// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="DateTimeExtensions.FirstDayOfMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the same month and year as the input.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose month and year are used.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to the first day of the month at midnight (00:00:00), with the same <see cref="DateTime.Kind" />
		/// as the input.
		/// </returns>
		/// <remarks>The time component is normalized to midnight (00:00:00), and the <see cref="DateTime.Kind" /> is preserved.</remarks>
		public static DateTime FirstDayOfMonth(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetFirstDayOfMonthTicks(dateTime), dateTime.Kind);

		/// <summary>
		/// Returns a <see cref="DateTime" /> representing midnight on the first day of the specified year and month.
		/// </summary>
		/// <param name="year">The year component of the date. Must be between <see cref="DateTime.MinValue.Year" /> and <see cref="DateTime.MaxValue.Year" />.</param>
		/// <param name="month">The month component of the date (1–12).</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to 00:00:00 on the first day of the specified month and year, with
		/// <see cref="DateTimeKind.Unspecified" /> as its kind.
		/// </returns>
		/// <remarks>The returned <see cref="DateTime" /> has its time component set to midnight and the kind set to <see cref="DateTimeKind.Unspecified" />.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the valid range, or if <paramref name="month" /> is not between 1 and 12.
		/// </exception>
		public static DateTime FirstDayOfMonth(int year, int month)
		{
			ThrowHelper.ThrowIfOutOfRange(year, DateTime.MinValue.Year, DateTime.MaxValue.Year);
			ThrowHelper.ThrowIfOutOfRange(month, 1, 12);

			return new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Unspecified);
		}
	}
}