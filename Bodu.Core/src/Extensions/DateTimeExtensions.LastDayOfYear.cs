// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfYear.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last day of the same calendar year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">
		/// The input <see cref="DateTime" /> whose calendar year is used to calculate the result. The returned value preserves the original <see cref="DateTime.Kind" />.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to midnight (00:00:00) on December 31 of the same year as <paramref name="dateTime" />, with
		/// the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method uses the proleptic Gregorian calendar to determine the final day of the year, and normalizes the time component to
		/// midnight (00:00:00).
		/// </para>
		/// <para>The resulting date always falls within the same calendar year as <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime LastDayOfYear(this DateTime dateTime) =>
			new(GetTicksForDate(dateTime.Year, 12, 31), dateTime.Kind);
	}
}