// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfTheYear.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last day of the same calendar year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose year is used to determine the result.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to December 31 of the same year as <paramref name="dateTime" />, with the time component set to
		/// midnight (00:00:00) and the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>This method uses the Gregorian calendar and normalizes the time component to 00:00:00 (midnight).</remarks>
		public static DateTime LastDayOfYear(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetTicksForDate(dateTime.Year, 12, 31), dateTime.Kind);
	}
}
