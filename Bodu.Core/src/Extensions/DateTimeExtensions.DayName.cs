// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="DayName.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the full name of the day of the week for the specified <see cref="DateTime" />, using the formatting rules of the
		/// current culture.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose <see cref="DateTime.DayOfWeek" /> value is used to determine the name.</param>
		/// <returns>A <see cref="string" /> containing the localized full name of the day of the week, formatted using <see cref="CultureInfo.CurrentCulture" />.</returns>
		/// <remarks>
		/// <para>
		/// This method retrieves the day name from the <see cref="DateTimeFormatInfo.DayNames" /> property of the current culture’s <see cref="DateTimeFormatInfo" />.
		/// </para>
		/// <note>This method is functionally equivalent to calling <c>DayName(null)</c>.</note>
		/// </remarks>
		public static string DayName(this DateTime dateTime) =>
			dateTime.DayName(null!);

		/// <summary>
		/// Returns the full name of the day of the week for the specified <see cref="DateTime" />, using the formatting rules of the
		/// specified culture.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose <see cref="DateTime.DayOfWeek" /> value is used to determine the name.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to format the result. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="string" /> containing the localized full name of the day of the week for <paramref name="dateTime" />, formatted
		/// using the specified or current culture.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method retrieves the day name from the <see cref="DateTimeFormatInfo.DayNames" /> property of the
		/// <see cref="DateTimeFormatInfo" /> associated with the specified or current culture.
		/// </para>
		/// </remarks>
		public static string DayName(this DateTime dateTime, CultureInfo culture) =>
			(culture ?? CultureInfo.CurrentCulture).DateTimeFormat.GetDayName(dateTime.DayOfWeek);
	}
}