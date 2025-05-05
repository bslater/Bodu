// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.StartOfDay.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the beginning of the same calendar day as the specified
		/// <paramref name="dateTime" />, with the time set to 00:00:00.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose date is preserved while the time component is reset.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to midnight (00:00:00) on the same day as <paramref name="dateTime" />, preserving the
		/// original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>This method is functionally similar to accessing <c>dateTime.Date</c>, but explicitly retains the original <see cref="DateTime.Kind" />.</remarks>
		public static DateTime ToStartOfDay(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetDateTicks(dateTime), dateTime.Kind);
	}
}
