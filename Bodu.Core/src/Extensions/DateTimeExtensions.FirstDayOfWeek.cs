// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.FirstDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using the current culture's settings.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the start of the week.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the culturally defined first day of the week that includes
		/// <paramref name="dateTime" />. The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// This method uses <see cref="CultureInfo.CurrentCulture" /> to determine the start of the week (via
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" />). The result is normalized to midnight and excludes any time component.
		/// </remarks>
		public static DateTime FirstDayOfWeek(this DateTime dateTime)
			=> dateTime.FirstDayOfWeek(null!);

		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using the provided or current culture.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the start of the week.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> that defines the starting day of the week via
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" />. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the week that contains <paramref name="dateTime" />.
		/// The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// This method computes the difference between the current day and the culture-defined first day of the week, then subtracts that
		/// number of days from <paramref name="dateTime" /> and resets the time to midnight.
		/// </remarks>
		public static DateTime FirstDayOfWeek(this DateTime dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

			long baseTicks = DateTimeExtensions.GetDateTicks(dateTime);
			long offsetTicks = ((7 + (dateTime.DayOfWeek - firstDayOfWeek)) % 7) * DateTimeExtensions.TicksPerDay;

			long ticks = baseTicks - offsetTicks;

			if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(ticks, dateTime.Kind);
		}
	}
}