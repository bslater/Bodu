// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="StartOfDay.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the start of the calendar day for the specified <paramref name="dateTime" />,
		/// with the time component set to 00:00:00 (midnight).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose date is preserved while the time is reset.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the same day as <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// This method is functionally similar to accessing <c>dateTime.Date</c>, but it explicitly retains the
		/// <see cref="DateTime.Kind" /> of the original value, which <c>DateTime.Date</c> does not.
		/// </remarks>
		public static DateTime StartOfDay(this DateTime dateTime) =>
			new(GetDateTicks(dateTime), dateTime.Kind);
	}
}