// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.FirstDateOfIsoWeek.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first date of the specified ISO 8601 week and year.
		/// </summary>
		/// <param name="isoYear">The ISO 8601 year (e.g., 2024).</param>
		/// <param name="isoWeek">The ISO 8601 week number (1–53).</param>
		/// <returns>A <see cref="DateTime" /> value representing the Monday that begins the given ISO 8601 week.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="isoWeek" /> or <paramref name="isoYear" /> are not valid ISO 8601 values.
		/// </exception>
		public static DateTime FirstDateOfIsoWeek(int isoYear, int isoWeek)
			=> ISOWeek.ToDateTime(isoYear, isoWeek, DayOfWeek.Monday);
	}
}
