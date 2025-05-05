// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfWeek.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using Bodu.Globalization.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateTime" />, using the current culture's calendar settings.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose week context is evaluated.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the culturally defined last day of the week, with the time component set to midnight
		/// (00:00:00) and the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// This method uses <see cref="CultureInfo.CurrentCulture" /> to determine the last day of the week. The result is computed by
		/// finding the next occurrence of that day from <paramref name="dateTime" /> (or the same day if already matched). If the resulting
		/// date exceeds the valid range for <see cref="DateTime" />, an <see cref="ArgumentOutOfRangeException" /> is thrown.
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime)
					=> dateTime.LastDayOfWeek(null!);

		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateTime" />, using the specified <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> from which the last day of the week is calculated.</param>
		/// <param name="culture">
		/// The <see cref="CultureInfo" /> that defines the first day of the week via its <see cref="DateTimeFormatInfo" />. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value set to midnight on the culturally determined last day of the week relative to
		/// <paramref name="dateTime" />, preserving its <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the resulting <see cref="DateTime" /> exceeds the supported <see cref="DateTime" /> range.
		/// </exception>
		/// <remarks>
		/// The last day of the week is determined by calling <see cref="DateTimeFormatInfo.FirstDayOfWeek" /> from the provided or current
		/// culture. If <paramref name="dateTime" /> already falls on the last day, it is returned at midnight. Otherwise, the next
		/// occurrence is calculated.
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek lastDayOfWeek = culture.DateTimeFormat.LastDayOfWeek();

			long baseTicks = DateTimeExtensions.GetDateTicks(dateTime);
			long offsetTicks = dateTime.DayOfWeek == lastDayOfWeek
				? 0
				: DateTimeExtensions.GetNextDayOfWeekAsTicks(dateTime, lastDayOfWeek);

			long ticks = baseTicks + offsetTicks;

			if (ticks < DateTime.MinValue.Ticks || ticks > DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime), ResourceStrings.Arg_OutOfRange_ResultingDateTimeOutOfRange);

			return new DateTime(ticks, dateTime.Kind);
		}
	}
}
