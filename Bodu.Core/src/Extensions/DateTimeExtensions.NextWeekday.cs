// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.NextDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar weekday after the given <paramref name="dateTime" />, based
		/// on the specified <paramref name="weekend" /> definition.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search forward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that defines which days are considered weekends.</param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next weekday according to the given <paramref name="weekend" /> definition.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a valid <see cref="CalendarWeekendDefinition" /> value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> falls on a weekday, the result will be the next calendar day that is also a weekday according to
		/// the specified <paramref name="weekend" /> pattern.
		/// </para>
		/// <para>The returned value preserves the original time-of-day and <see cref="DateTime.Kind" /> values of <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime NextWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks += DateTimeExtensions.TicksPerDay;
			}
			while (DateTimeExtensions.IsWeekend(DateTimeExtensions.GetDayOfWeekFromTicks(ticks), weekend));

			return new DateTime(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar weekday after the given <paramref name="dateTime" />, based
		/// on the specified <paramref name="weekend" /> definition and optional <paramref name="provider" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search forward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that defines the default pattern of weekend days.</param>
		/// <param name="provider">
		/// An optional <see cref="ICalendarWeekendProvider" /> that can override the weekend definition with custom logic. If <c>null</c>,
		/// the default behavior based on <paramref name="weekend" /> is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next weekday according to the given <paramref name="weekend" /> definition and optional <paramref name="provider" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a valid <see cref="CalendarWeekendDefinition" /> value.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> falls on a weekday, the result will be the next calendar day that is also a weekday according to
		/// the specified <paramref name="weekend" /> pattern or the custom rules provided by <paramref name="provider" />.
		/// </para>
		/// <para>The returned value preserves the original time-of-day and <see cref="DateTime.Kind" /> values of <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime NextWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend, ICalendarWeekendProvider? provider)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks += DateTimeExtensions.TicksPerDay;
			}
			while (DateTimeExtensions.IsWeekend(DateTimeExtensions.GetDayOfWeekFromTicks(ticks), weekend, provider));

			return new DateTime(ticks, dateTime.Kind);
		}
	}
}
