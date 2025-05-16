// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Midday.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing 12:00 PM (noon) on the same calendar day as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose date is used to compute the result.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to 12:00:00.000 (midday) on the same date as <paramref name="dateTime" />, preserving its <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method replaces the time component of the input with exactly 12:00 PM (midday), while retaining the original year, month,
		/// day, and <see cref="DateTime.Kind" />.
		/// </para>
		/// <para>The returned value always represents the midpoint of the day (noon), with no fractional seconds or milliseconds.</para>
		/// </remarks>
		public static DateTime Midday(this DateTime dateTime) =>
			new(dateTime.Date.Ticks + (TicksPerHour * 12), dateTime.Kind);
	}
}