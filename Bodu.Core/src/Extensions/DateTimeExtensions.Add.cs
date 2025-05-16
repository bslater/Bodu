// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Add.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Adds the specified number of calendar years, months, and fractional days to the given <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> value to adjust.</param>
		/// <param name="years">The number of years to add. Negative values subtract years.</param>
		/// <param name="months">The number of months to add. Negative values subtract months.</param>
		/// <param name="days">The number of days to add, including fractional days. Negative values subtract days.</param>
		/// <returns>
		/// A new <see cref="DateTime" /> adjusted by the specified values. The original <see cref="DateTime.Kind" /> and time-of-day are
		/// preserved unless modified by the <paramref name="days" /> parameter.
		/// </returns>
		/// <remarks>
		/// <para>Adjustments are applied in the following order: <b>years</b>, then <b>months</b>, then <b>days</b>.</para>
		/// <para>
		/// Year and month adjustments honor calendar boundaries, including leap years. If the resulting day exceeds the last valid day of
		/// the target month (e.g., adding one month to January 31), it is clamped to the final day of that month.
		/// </para>
		/// <para>
		/// The original time-of-day is preserved unless the <paramref name="days" /> parameter includes a fractional component, in which
		/// case the time-of-day is adjusted accordingly.
		/// </para>
		/// <para>
		/// Fractional days are applied with tick-level precision. Values smaller than machine epsilon ( <c>1e-10</c>) are ignored to avoid
		/// unnecessary computation.
		/// </para>
		/// <para>
		/// This method performs all adjustments using tick arithmetic and does not rely on <see cref="DateTime.AddYears(int)" />,
		/// <see cref="DateTime.AddMonths(int)" />, or <see cref="DateTime.AddDays(double)" />, making it suitable for performance-critical paths.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the resulting date is earlier than <see cref="DateTime.MinValue" /> or later than <see cref="DateTime.MaxValue" />.
		/// </exception>
		public static DateTime Add(this DateTime dateTime, int years, int months, double days)
		{
			// Extract parts from original date
			dateTime.GetDateParts(out int y, out int m, out int d);

			// Adjust year/month
			int i = m + months - 1 + (years * 12);
			if (i >= 0)
			{
				m = (i % 12) + 1;
				y += i / 12;
			}
			else
			{
				m = 12 + ((i + 1) % 12);
				y += (i - 11) / 12;
			}

			// Clamp day
			int day = Math.Min(d, DateTime.DaysInMonth(y, m));
			long totalTicks = GetTicksForDate(y, m, day) + GetTimeTicks(dateTime);

			// Add fractional days (if any)
			if (Math.Abs(days) > Epsilon)
				totalTicks += GetDaysToTicks(days);

			if ((ulong)totalTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new(totalTicks, dateTime.Kind);
		}
	}
}