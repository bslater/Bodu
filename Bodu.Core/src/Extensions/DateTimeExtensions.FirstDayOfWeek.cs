// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using the first day of the week defined
		/// by the current culture.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" />.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing midnight (00:00:00) on the first day of the week that includes
		/// <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>This overload uses <see cref="CultureInfo.CurrentCulture" /> to determine the first day of the week, based on <see cref="DateTimeFormatInfo.FirstDayOfWeek" />.</remarks>
		public static DateTime FirstDayOfWeek(this DateTime dateTime) =>
			dateTime.FirstDayOfWeek(null!);

		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using the first day of the week defined
		/// by the specified or current culture.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" />.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to determine the start of the week. If <c>null</c>,
		/// <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing midnight (00:00:00) on the first day of the week that includes
		/// <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method computes the number of days between <paramref name="dateTime" /> and the culturally defined first day of the week,
		/// then subtracts that offset and resets the time to midnight.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the computed date falls outside the supported range of <see cref="DateTime" />.</exception>
		public static DateTime FirstDayOfWeek(this DateTime dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

			long baseTicks = GetDateTicks(dateTime);
			long offsetTicks = ((7 + (dateTime.DayOfWeek - firstDayOfWeek)) % 7) * TicksPerDay;

			long ticks = baseTicks - offsetTicks;

			if ((ulong)ticks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateTime" />, using a start-of-week inferred from the
		/// specified <see cref="CalendarWeekendDefinition" />.
		/// </summary>
		/// <param name="dateTime">The reference <see cref="DateTime" />.</param>
		/// <param name="weekend">
		/// A <see cref="CalendarWeekendDefinition" /> used to infer the first day of the week. For example,
		/// <see cref="CalendarWeekendDefinition.SaturdaySunday" /> implies a Monday start.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing midnight (00:00:00) on the first day of the week that includes
		/// <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The method infers the start of the week based on the <paramref name="weekend" /> value. If
		/// <see cref="CalendarWeekendDefinition.None" /> is specified, the method defaults to using <see cref="DayOfWeek.Monday" /> as the
		/// start of the week.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a valid <see cref="CalendarWeekendDefinition" /> value, or if the resulting date is
		/// outside the supported range of <see cref="DateTime" />.
		/// </exception>
		public static DateTime FirstDayOfWeek(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);
			DayOfWeek startOfWeek = GetWeekStartDay(weekend);

			int offsetDays = ((7 + (dateTime.DayOfWeek - startOfWeek)) % 7);
			long dateTicks = dateTime.Ticks - offsetDays * TicksPerDay;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new(dateTicks, dateTime.Kind);
		}
	}
}