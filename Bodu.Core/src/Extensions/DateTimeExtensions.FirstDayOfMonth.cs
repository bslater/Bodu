// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.FirstDayOfMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the same month and year as the input.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose month and year are used.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to the first day of the month at midnight (00:00:00), with the same <see cref="DateTime.Kind" />
		/// as the input.
		/// </returns>
		/// <remarks>The time component is normalized to midnight (00:00:00), and the <see cref="DateTime.Kind" /> is preserved.</remarks>
		public static DateTime FirstDayOfMonth(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetFirstDayOfMonthTicks(dateTime), dateTime.Kind);
	}
}
