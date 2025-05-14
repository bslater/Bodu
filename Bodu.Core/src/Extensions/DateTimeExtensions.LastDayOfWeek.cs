// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastDayOfWeek.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using Bodu.Globalization.Extensions;
using System;
using System.Globalization;
using System.Threading;

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

			long dateTicks = baseTicks + offsetTicks;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(dateTicks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateTime" />, using the inferred start-of-week day from
		/// the provided <see cref="CalendarWeekendDefinition" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose week context is evaluated.</param>
		/// <param name="weekend">
		/// A <see cref="CalendarWeekendDefinition" /> value used to infer the start of the week and determine the week's structure.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the last day of the week that contains <paramref name="dateTime" />. The returned
		/// value has a time component of 00:00:00 and preserves the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined <see cref="CalendarWeekendDefinition" /> value.
		/// </exception>
		/// <remarks>
		/// This method uses the specified <paramref name="weekend" /> to infer the first day of the week, then calculates the last day as 6
		/// days after the inferred start.
		/// <para>
		/// If <paramref name="weekend" /> is set to <see cref="CalendarWeekendDefinition.None" />, the method defaults to using
		/// <see cref="DayOfWeek.Monday" /> as the start of the week.
		/// </para>
		/// The result is offset forward up to 6 days from the input date, and is normalized to midnight.
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);
			DayOfWeek startOfWeek = GetWeekStartDay(weekend);
			DayOfWeek endOfWeek = (DayOfWeek)(((int)startOfWeek + 6) % 7);

			int offsetDays = ((int)endOfWeek - (int)dateTime.DayOfWeek + 7) % 7;
			long dateTicks = dateTime.Ticks + offsetDays * DateTimeExtensions.TicksPerDay;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(dateTicks, dateTime.Kind);
		}
	}
}