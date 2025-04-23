// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.MonthName.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the full name of the month for the specified <see cref="DateOnly" /> using the
		/// current culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A localized string representing the month name.</returns>
		/// <remarks>The result is culture-sensitive and uses <see cref="CultureInfo.CurrentCulture" />.</remarks>
		public static string MonthName(this DateOnly date)
			=> dateTime.MonthName(null);

		/// <summary>
		/// Returns the full name of the month for the specified <see cref="DateOnly" /> using the
		/// specified culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" />. If null, the current culture is used.
		/// </param>
		/// <returns>A localized string representing the month name.</returns>
		public static string MonthName(this DateOnly date, CultureInfo culture)
			=> (culture ?? System.Globalization.CultureInfo.CurrentCulture).DateOnlyFormat.GetMonthName(dateTime.Month);
	}
}