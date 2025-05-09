// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.IsFirstDayOfMonth.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the current <see cref="DateTime" /> instance represents the first day of its month.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <returns><see langword="true" /> if the <paramref name="dateTime" /> is the first day of its month; otherwise, <see langword="false" />.</returns>
		/// <remarks>This method checks whether the day component of the <paramref name="dateTime" /> is equal to 1.</remarks>
		public static bool IsFirstDayOfMonth(this DateTime dateTime) =>
			dateTime.Day == 1;
	}
}