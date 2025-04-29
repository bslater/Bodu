// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.ToMidnight.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing midnight (00:00:00) at the start of the same calendar day as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose date is preserved.</param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same year, month, and day as <paramref name="dateTime" />, with the time component set to
		/// 00:00:00 (midnight). The original <see cref="DateTime.Kind" /> is preserved.
		/// </returns>
		/// <remarks>
		/// This method is functionally equivalent to accessing <c>dateTime.Date</c>, but it explicitly retains the original <see cref="DateTime.Kind" />.
		/// </remarks>
		public static DateTime ToMidnight(this DateTime dateTime)
			=> dateTime.Date;
	}
}
