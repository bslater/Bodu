// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.IsLastDayOfMonth.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the current <see cref="DateTime" /> instance represents the last day of its month.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <returns><see langword="true" /> if the <paramref name="dateTime" /> is the last day of its month; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method compares the day component of the <paramref name="dateTime" /> with the total number of days in its month,
		/// accounting for variations such as leap years.
		/// </remarks>
		public static bool IsLastDayOfMonth(this DateTime dateTime) =>
			dateTime.Day == DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
	}
}