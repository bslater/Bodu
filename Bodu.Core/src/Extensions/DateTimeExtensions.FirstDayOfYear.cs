// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.FirstDayOfYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the same calendar year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose year is used to calculate the result.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to January 1 of the same year as <paramref name="dateTime" />, with the time component set to
		/// 00:00:00 and the <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// The method resets the date to January 1 of the year and normalizes the time to midnight (00:00:00). The result always falls
		/// within the same calendar year as the original <paramref name="dateTime" />.
		/// </remarks>
		public static DateTime FirstDayOfYear(this DateTime dateTime)
			=> new(DateTimeExtensions.GetTicksForDate(dateTime.Year, 1, 1), dateTime.Kind);
	}
}