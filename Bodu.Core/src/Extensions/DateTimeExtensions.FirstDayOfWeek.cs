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

			if ((ulong)ticks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using the inferred start-of-week day from
		/// the provided <see cref="CalendarWeekendDefinition" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> whose week context is evaluated.</param>
		/// <param name="weekend">A <see cref="CalendarWeekendDefinition" /> value used to infer the start of the week.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the first day of the week that contains <paramref name="dateTime" />. The returned
		/// value has a time component of 00:00:00 and preserves the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined <see cref="CalendarWeekendDefinition" /> value.
		/// </exception>
		/// <remarks>
		/// This method uses the specified <paramref name="weekend" /> to infer the first day of the week. For example, if
		/// <see cref="CalendarWeekendDefinition.SaturdaySunday" /> is provided, the start of the week is Monday.
		/// <para>
		/// If <paramref name="weekend" /> is set to <see cref="CalendarWeekendDefinition.None" />, the method defaults to using
		/// <see cref="DayOfWeek.Monday" /> as the start of the week.
		/// </para>
		/// The returned date is normalized to midnight and may fall before or on the specified <paramref name="dateTime" />.
		/// </remarks>
		public static DateTime FirstDayOfWeek(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);
			DayOfWeek startOfWeek = GetWeekStartDay(weekend);

			int offsetDays = ((7 + (dateTime.DayOfWeek - startOfWeek)) % 7);
			long dateTicks = dateTime.Ticks - offsetDays * DateTimeExtensions.TicksPerDay;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new DateTime(dateTicks, dateTime.Kind);
		}
	}
}