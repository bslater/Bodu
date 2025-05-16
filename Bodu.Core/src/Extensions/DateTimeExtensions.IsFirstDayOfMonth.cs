// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IsFirstDayOfMonth.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Indicates whether the specified <see cref="DateTime" /> falls on the first day of its month.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns><see langword="true" /> if <paramref name="dateTime" /> represents the first day of its month; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method compares the <see cref="DateTime.Day" /> component to <c>1</c> to determine if the date is the first day of the month.
		/// </remarks>
		public static bool IsFirstDayOfMonth(this DateTime dateTime) =>
			dateTime.Day == 1;
	}
}