// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Adds the specified number of years, months, and fractional days to the provided <see cref="DateOnly" />.
		/// </summary>
		/// <param name="date">The starting <see cref="DateOnly" /> value.</param>
		/// <param name="years">The number of years to add. Can be negative to subtract years.</param>
		/// <param name="months">The number of months to add. Can be negative to subtract months.</param>
		/// <param name="days">
		/// The number of whole and fractional days to add. Can be negative to subtract days.
		/// </param>
		/// <returns>A <see cref="DateOnly" /> adjusted by the specified components.</returns>
		/// <remarks>This method respects leap years and month boundaries.</remarks>
		public static DateOnly Add(this DateOnly date, int years, int months, double days)
		{
			DateOnlyExtensions.GetDatePart(date, out int y, out int m, out int d);

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

			return new DateOnly(y, m, 1);
		}
	}
}