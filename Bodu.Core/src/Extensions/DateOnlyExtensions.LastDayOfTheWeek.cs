// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.LastDayOfTheWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Globalization.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last day of the week for the given <see cref="DateOnly" /> using the current culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <returns>A new <see cref="DateOnly" /> set to the last day of the week.</returns>
		/// <remarks>The last day is determined by <see cref="CultureInfo.CurrentCulture" />.</remarks>
		public static DateOnly LastDayOfTheWeek(this DateOnly date)
			=> dateTime.LastDayOfTheWeek(null);

		/// <summary>
		/// Returns the last day of the week for the given <see cref="DateOnly" /> using the
		/// specified culture.
		/// </summary>
		/// <param name="date">The input <see cref="DateOnly" />.</param>
		/// <param name="culture">
		/// The culture used to determine the end of the week. If null, the current culture is used.
		/// </param>
		/// <returns>A new <see cref="DateOnly" /> set to the last day of the week.</returns>
		/// <remarks>
		/// Preserves the <see cref="DateOnly.Kind" /> and adjusts based on cultural norms.
		/// </remarks>
		public static DateOnly LastDayOfTheWeek(this DateOnly date, CultureInfo culture)
		{
			DayOfWeek lastDayOfWeek = (culture ?? Thread.CurrentThread.CurrentCulture).DateOnlyFormat.LastDayOfWeek();
			return dateTime.DayOfWeek == lastDayOfWeek
				? dateTime.StartOfTheDay()
				: new DateOnly(DateOnlyExtensions.DateTicks(dateTime) + DateOnlyExtensions.NextDayOfWeekTicks(dateTime, lastDayOfWeek), dateTime.Kind);
		}
	}
}