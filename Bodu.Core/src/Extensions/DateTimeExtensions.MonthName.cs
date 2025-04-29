// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.MonthName.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the full name of the month for the specified <see cref="DateTime" />, using the current culture's formatting rules.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose month value is used to retrieve the name.</param>
		/// <returns>A localized <see cref="string" /> representing the full month name, based on <see cref="CultureInfo.CurrentCulture" />.</returns>
		/// <remarks>
		/// This method uses the <see cref="DateTimeFormatInfo.MonthNames" /> or the <see cref="CultureInfo.CurrentCulture" /> to retrieve
		/// the localized month name.
		/// </remarks>
		public static string MonthName(this DateTime dateTime)
			=> dateTime.MonthName(null!);

		/// <summary>
		/// Returns the full name of the month for the specified <see cref="DateTime" />, using the formatting rules of the given <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose month value is used to retrieve the name.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> that provides culture-specific formatting. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>A localized <see cref="string" /> representing the full month name based on the provided or current culture.</returns>
		/// <remarks>
		/// This method uses the <see cref="DateTimeFormatInfo.MonthNames" /> collection of the given or current culture to return the
		/// localized full name of the month represented by <paramref name="dateTime.Month" />.
		/// </remarks>
		public static string MonthName(this DateTime dateTime, CultureInfo culture)
			=> (culture ?? System.Globalization.CultureInfo.CurrentCulture).DateTimeFormat.GetMonthName(dateTime.Month);
	}
}
