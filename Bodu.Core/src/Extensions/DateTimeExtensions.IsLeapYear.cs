// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.IsLeapYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the year of the specified <see cref="DateTime" /> is a leap year in the Gregorian calendar.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose year is evaluated.</param>
		/// <returns><see langword="true" /> if the year is a leap year (i.e., contains February 29); otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method follows the rules of the Gregorian calendar, where a leap year occurs every 4 years, except for years that are
		/// divisible by 100 but not by 400 (e.g., 2000 was a leap year, but 1900 was not).
		/// </remarks>
		public static bool IsLeapYear(this DateTime dateTime)
			=> DateTime.IsLeapYear(dateTime.Year);
	}
}
