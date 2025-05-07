// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.ToMidday.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing 12:00 PM (noon) on the same calendar day as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose date is used.</param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same year, month, and day as <paramref name="dateTime" />, and the time component set to
		/// 12:00:00.000 (midday). The <see cref="DateTime.Kind" /> is preserved.
		/// </returns>
		/// <remarks>
		/// This method sets the time-of-day component to exactly 12:00 PM (midday), while preserving the original date and <see cref="DateTime.Kind" />.
		/// </remarks>
		public static DateTime ToMidday(this DateTime dateTime)
			=> new(dateTime.Date.Ticks + (DateTimeExtensions.TicksPerHour * 12), dateTime.Kind);
	}
}