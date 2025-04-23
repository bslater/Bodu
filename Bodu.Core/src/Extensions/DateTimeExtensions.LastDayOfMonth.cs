// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.LastDayOfMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last day of the same month and year as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose month and year are used to determine the last day.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the last day of the month, with the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// This method uses the rules of the Gregorian calendar to determine the number of days in the month, including leap year behavior
		/// for February. The time component is normalized to midnight (00:00:00).
		/// </remarks>
		public static DateTime LastDayOfMonth(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetLastDayOfMonthTicks(dateTime), dateTime.Kind);
	}
}
