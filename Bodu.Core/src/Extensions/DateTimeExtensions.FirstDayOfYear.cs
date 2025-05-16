// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfYear.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the first day of the same calendar year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" /> whose <c>Year</c> value is used to calculate the result.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on January 1 of the same year as <paramref name="dateTime" />, preserving
		/// the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method resets the date to January 1 of the same calendar year and normalizes the time component to midnight. The
		/// <see cref="DateTime.Kind" /> of the original input is retained in the result.
		/// </para>
		/// </remarks>
		public static DateTime FirstDayOfYear(this DateTime dateTime)
			=> new(GetTicksForDate(dateTime.Year, 1, 1), dateTime.Kind);
	}
}