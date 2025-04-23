// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.EndOfDay.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the last possible tick of the same day as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The original <see cref="DateTime" /> whose date is used to determine the end of the day.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to 23:59:59.9999999 on the same calendar day as <paramref name="dateTime" />, with the
		/// original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// The result represents the maximum possible time value for the day, just before midnight of the following day. The
		/// <see cref="DateTime.Kind" /> of the input is retained in the result.
		/// </remarks>
		public static DateTime EndOfDay(this DateTime dateTime)
			=> new DateTime(DateTimeExtensions.GetTicks(dateTime) + (DateTimeExtensions.TicksPerDay - 1), dateTime.Kind);
	}
}
