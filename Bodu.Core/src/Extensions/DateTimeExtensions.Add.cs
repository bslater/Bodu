// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
		/// <param name="days">The number of days (including fractional values) to add. A negative value subtracts days.</param>
		/// <returns>
		/// A new <see cref="DateTime" /> adjusted by the specified number of years, months, and days, preserving the original time-of-day
		/// and <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// The adjustments are applied in the following order: years, then months, then days. This method respects calendar boundaries,
		/// leap years (e.g., Feb 29), and automatically normalizes the date when overflowing into the next month (e.g., adding 1 month to
		/// January 31 results in the last valid day of February).
		/// </remarks>
		public static DateTime Add(this DateTime dateTime, int years, int months, double days)
		{
			DateTimeExtensions.GetDateParts(dateTime, out int y, out int m, out int d);

			// compute the month and year
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

			// Clamp day to number of days in new month
			d = Math.Min(d, DateTime.DaysInMonth(y, m));

			// Reconstruct the base date with time component
			var baseDate = new DateTime(y, m, d, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, dateTime.Kind);

			// Only apply AddDays if the day fraction is materially non-zero
			return Math.Abs(days) > Epsilon ? baseDate.AddDays(days) : baseDate;
		}
	}
}
