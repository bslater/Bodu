// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.WeekOfYear.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the ISO week number of the year that contains the specified date.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateOnly" /> to evaluate.</param>
		/// <returns>The week number (1�53) of the year.</returns>
		/// <remarks>This method uses the current culture's calendar and week rule.</remarks>
		public static int WeekOfYear(this DateOnly date)
			=> dateTime.WeekOfYear(null);

		/// <summary>
		/// Returns the ISO week number of the year that contains the specified date using the
		/// specified culture.
		/// </summary>
		/// <param name="this">The <see cref="DateOnly" /> to evaluate.</param>
		/// <param name="culture">
		/// The <see cref="CultureInfo" /> used to determine calendar rules. If null, the current
		/// culture is used.
		/// </param>
		/// <returns>The week number (1�53) of the year.</returns>
		/// <remarks>
		/// The result is based on the <see cref="CalendarWeekRule" /> and
		/// <see cref="DateOnlyFormatInfo.FirstDayOfWeek" /> of the culture.
		/// </remarks>
		public static int WeekOfYear(this DateOnly @this, CultureInfo culture)
		{
			DateOnlyFormatInfo info = (culture ?? Thread.CurrentThread.CurrentCulture).DateOnlyFormat;
			return culture.Calendar.GetWeekOfYear(@this, info.CalendarWeekRule, info.FirstDayOfWeek);
		}
	}
}