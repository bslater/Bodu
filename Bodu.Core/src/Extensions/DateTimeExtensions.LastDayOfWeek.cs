// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfWeek.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
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
		/// A <see cref="DateTime" /> representing the culturally defined last day of the week, set to midnight (00:00:00) and preserving
		/// the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method uses <see cref="CultureInfo.CurrentCulture" /> to determine the first day of the week and infers the last day by
		/// advancing six days. The result is calculated from <paramref name="dateTime" />, normalized to midnight, and is guaranteed to
		/// fall within the same week context.
		/// </para>
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime) =>
			dateTime.LastDayOfWeek(null!);

		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateTime" />, using the specified <see cref="CultureInfo" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> from which the last day of the week is calculated.</param>
		/// <param name="culture">
		/// The <see cref="CultureInfo" /> that defines the week's starting day via <see cref="DateTimeFormatInfo.FirstDayOfWeek" />. If
		/// <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight on the culture-specific last day of the week that includes
		/// <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the calculated result exceeds <see cref="DateTime.MaxValue" />.</exception>
		/// <remarks>
		/// <para>
		/// This method determines the last day of the week by using the first day of the week defined in the culture and offsetting by 6
		/// days. If <paramref name="dateTime" /> already falls on the last day, it is returned as-is with its time component reset to 00:00:00.
		/// </para>
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek lastDayOfWeek = culture.DateTimeFormat.LastDayOfWeek();

			long baseTicks = GetDateTicks(dateTime);
			long offsetTicks = dateTime.DayOfWeek == lastDayOfWeek
				? 0
				: GetNextDayOfWeekAsTicks(dateTime, lastDayOfWeek);

			long dateTicks = baseTicks + offsetTicks;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new(dateTicks, dateTime.Kind);
		}

		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateTime" />, based on the inferred start of the week
		/// from the given <see cref="CalendarWeekendDefinition" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="weekend">
		/// A <see cref="CalendarWeekendDefinition" /> that defines which days are treated as weekends, and is used to infer the week's start.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight on the last day of the inferred week, preserving the <see cref="DateTime.Kind" /> of
		/// the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="weekend" /> is not a defined value of <see cref="CalendarWeekendDefinition" />.</exception>
		/// <remarks>
		/// <para>
		/// The method infers the start of the week from <paramref name="weekend" />, then calculates the last day as 6 days later. The
		/// result is normalized to 00:00:00 and falls on or after <paramref name="dateTime" /> within the same inferred week context.
		/// </para>
		/// <para>If <paramref name="weekend" /> is <see cref="CalendarWeekendDefinition.None" />, the week is assumed to start on <see cref="DayOfWeek.Monday" />.</para>
		/// </remarks>
		public static DateTime LastDayOfWeek(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);
			DayOfWeek startOfWeek = GetWeekStartDay(weekend);
			DayOfWeek endOfWeek = (DayOfWeek)(((int)startOfWeek + 6) % 7);

			int offsetDays = ((int)endOfWeek - (int)dateTime.DayOfWeek + 7) % 7;
			long dateTicks = dateTime.Ticks + offsetDays * TicksPerDay;

			if ((ulong)dateTicks > (ulong)DateTime.MaxValue.Ticks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateTime)));

			return new(dateTicks, dateTime.Kind);
		}
	}
}