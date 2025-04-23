// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.IsLeapYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Determines whether the year of the specified <see cref="DateOnly" /> is a leap year.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns><see langword="true" /> if the year is a leap year; otherwise, <see langword="false" />.</returns>
		public static bool IsLeapYear(this DateOnly date)
			=> DateOnly.IsLeapYear(dateTime.Year);
	}
}