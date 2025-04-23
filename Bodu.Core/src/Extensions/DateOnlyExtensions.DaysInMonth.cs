// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.DaysInMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the number of days in the current month of the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateOnly" /> to inspect.</param>
		/// <returns>The number of days in the month of the given date.</returns>
		/// <remarks>This method always interprets the month and year using the Gregorian calendar.</remarks>
		public static int DaysInMonth(this DateOnly date)
			=> DateOnly.DaysInMonth(dateTime.Year, dateTime.Month);
	}
}