// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.DaysInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the number of days in the month of the specified <see cref="DateTime" />, using the Gregorian calendar.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose month and year are used to determine the number of days.</param>
		/// <returns>The total number of days in the month for the given <paramref name="dateTime" />, based on the <see cref="GregorianCalendar" />.</returns>
		/// <remarks>
		/// This method always evaluates the number of days using the proleptic Gregorian calendar, regardless of the current culture or
		/// calendar settings.
		/// </remarks>
		public static int DaysInMonth(this DateTime dateTime)
			=> DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
	}
}
