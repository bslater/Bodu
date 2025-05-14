// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateOnlyExtensions.Add.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Adds the specified number of years, months, and fractional days to the given <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The starting <see cref="DateOnly" /> value to which the offsets will be applied.</param>
		/// <param name="years">The number of calendar years to add. A negative value subtracts years.</param>
		/// <param name="months">The number of calendar months to add. A negative value subtracts months.</param>
		/// <param name="days">The number of days to add, including fractional values. A negative value subtracts days.</param>
		/// <returns>A new <see cref="DateOnly" /> adjusted by the specified number of years, months, and days.</returns>
		/// <remarks>
		/// <para>Applies adjustments in the following order: <b>years</b>, then <b>months</b>, then <b>days</b>.</para>
		/// <para>Clamps the resulting day to the last valid day of the calculated month if overflow occurs.</para>
		/// <para>Only full days are supported. Any fractional <paramref name="days" /> is rounded to the nearest whole number.</para>
		/// <para>Internally uses <see cref="DateOnly.DayNumber" /> for efficient tick-free date math.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the resulting date is before <see cref="DateOnly.MinValue" /> or after <see cref="DateOnly.MaxValue" />.
		/// </exception>
		public static DateOnly Add(this DateOnly date, int years, int months, int days)
		{
			date.GetDateParts(out int y, out int m, out int d);

			int totalMonths = m - 1 + (years * 12) + months;
			if (totalMonths >= 0)
			{
				y += totalMonths / 12;
				m = (totalMonths % 12) + 1;
			}
			else
			{
				y += (totalMonths - 11) / 12;
				m = 12 + ((totalMonths + 1) % 12);
			}

			// Compute leap year status without method call
			bool isLeap = (y % 4 == 0) && ((y % 100 != 0) || (y % 400 == 0));
			int[] daysToMonth = isLeap ? DateTimeExtensions.DaysToMonth366 : DateTimeExtensions.DaysToMonth365;

			// Clamp day manually using table lookup
			int maxDay = daysToMonth[m] - daysToMonth[m - 1];
			if (d > maxDay) d = maxDay;

			int y1 = y - 1;
			int dayNumber = y1 * 365 + y1 / 4 - y1 / 100 + y1 / 400 + daysToMonth[m - 1] + d - 1;

			// Add days if non-trivial
			if (days > DateTimeExtensions.Epsilon || days < -DateTimeExtensions.Epsilon)
				dayNumber = checked(dayNumber + days);

			if ((uint)dayNumber > DateOnly.MaxValue.DayNumber)
				throw new ArgumentOutOfRangeException(nameof(date),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateOnly)));

			return DateOnly.FromDayNumber(dayNumber);
		}
	}
}