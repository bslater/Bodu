// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.Add.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Adds the specified number of years, months, and fractional days to the given <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> value to which the offsets will be applied.</param>
		/// <param name="years">The number of calendar years to add. A negative value subtracts years.</param>
		/// <param name="months">The number of calendar months to add. A negative value subtracts months.</param>
		/// <param name="days">The number of days to add, including fractional values. A negative value subtracts days.</param>
		/// <returns>
		/// A new <see cref="DateTime" /> that has been adjusted by the specified number of years, months, and days, preserving the original
		/// time-of-day and <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>The adjustments are applied in the following order: <b>years</b>, then <b>months</b>, then <b>days</b>.</para>
		/// <para>
		/// Month and year adjustments respect calendar boundaries, including leap years. If the resulting day exceeds the maximum valid day
		/// in the calculated month (e.g., adding one month to January 31), it is automatically clamped to the last valid day of the
		/// resulting month.
		/// </para>
		/// <para>
		/// Fractional days are applied with sub-millisecond precision using tick-based arithmetic. If the <paramref name="days" />
		/// parameter is less than machine epsilon (1e-10), it is ignored to avoid unnecessary computation.
		/// </para>
		/// <para>
		/// Internally, this method uses tick-level operations and does not rely on <see cref="DateTime.AddYears(int)" />,
		/// <see cref="DateTime.AddMonths(int)" />, or <see cref="DateTime.AddDays(double)" />, making it suitable for performance-sensitive scenarios.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the calculated date is earlier than <see cref="DateTime.MinValue" /> or later than <see cref="DateTime.MaxValue" />.
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

			return new DateTime(totalTicks, dateTime.Kind);
		}
	}
}