// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.DaysInYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the number of days in the year of the specified <see cref="DateOnly" /> using
		/// the current culture.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateOnly" /> to inspect.</param>
		/// <returns>The number of days in the year.</returns>
		/// <remarks>This method uses the <see cref="Calendar" /> of the current <see cref="CultureInfo" />.</remarks>
		public static int DaysInYear(this DateOnly date)
			=> dateTime.DaysInYear(null);

		/// <summary>
		/// Returns the number of days in the year of the specified <see cref="DateOnly" /> using
		/// the given calendar.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateOnly" /> to inspect.</param>
		/// <param name="calendar">
		/// The calendar to use. If null, the current culture's calendar is used.
		/// </param>
		/// <returns>The number of days in the year.</returns>
		public static int DaysInYear(this DateOnly date, Calendar calendar)
			=> (calendar ?? CultureInfo.CurrentCulture.Calendar).GetDaysInYear(dateTime.Year);
	}
}