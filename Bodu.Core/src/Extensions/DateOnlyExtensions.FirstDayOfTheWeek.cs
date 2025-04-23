// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.FirstDayOfTheWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the first day of the week for the given <see cref="DateOnly" /> using the
		/// current culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> at the beginning of the week.</returns>
		/// <remarks>The start of the week is determined by <see cref="CultureInfo.CurrentCulture" />.</remarks>
		public static DateOnly FirstDayOfTheWeek(this DateOnly date)
			=> dateTime.FirstDayOfTheWeek(null);

		/// <summary>
		/// Returns the first day of the week for the given <see cref="DateOnly" /> using the
		/// specified culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <param name="culture">
		/// Optional <see cref="CultureInfo" /> to determine the first day of the week. Uses current
		/// culture if null.
		/// </param>
		/// <returns>A <see cref="DateOnly" /> at the start of the week.</returns>
		public static DateOnly FirstDayOfTheWeek(this DateOnly date, CultureInfo culture)
		{
			DayOfWeek firstDayOfWeek = (culture ?? System.Globalization.CultureInfo.CurrentCulture).DateOnlyFormat.FirstDayOfWeek;
			return dateTime.DayOfWeek == firstDayOfWeek
				? dateTime.Date
				: new DateOnly(DateOnlyExtensions.DateTicks(dateTime) + DateOnlyExtensions.PreviousDayOfWeekTicks(dateTime, firstDayOfWeek), dateTime.Kind);
		}
	}
}