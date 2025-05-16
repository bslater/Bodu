// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="NextWeekday.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar weekday after the specified <paramref name="dateTime" />,
		/// based on the provided <paramref name="weekend" /> definition.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search forward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> used to determine which days are considered weekends.</param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next weekday according to the given <paramref name="weekend" /> rule.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined member of the <see cref="CalendarWeekendDefinition" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> falls on a weekday, the result will be the next calendar day that is also considered a weekday
		/// according to the specified <paramref name="weekend" /> rule.
		/// </para>
		/// <para>The returned value preserves the original time-of-day and <see cref="DateTime.Kind" /> of <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime NextWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks += TicksPerDay;
			}
			while (IsWeekend(GetDayOfWeekFromTicks(ticks), weekend));

			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the next calendar weekday after the specified <paramref name="dateTime" />,
		/// based on the given <paramref name="weekend" /> definition and optional custom <paramref name="provider" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search forward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that defines the default weekend pattern.</param>
		/// <param name="provider">
		/// An optional <see cref="IWeekendDefinitionProvider" /> used when <paramref name="weekend" /> is
		/// <see cref="CalendarWeekendDefinition.Custom" />. If <c>null</c>, the built-in logic is used.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" />,
		/// representing the next weekday based on the specified weekend rule and custom provider logic (if any).
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is <see cref="CalendarWeekendDefinition.Custom" /> and <paramref name="provider" /> is <c>null</c>.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="dateTime" /> falls on a weekday, the method returns the next calendar day that is also considered a weekday
		/// according to the <paramref name="weekend" /> pattern or the logic of the specified <paramref name="provider" />.
		/// </para>
		/// <para>The returned value preserves the original time-of-day and <see cref="DateTime.Kind" /> of <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime NextWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend, IWeekendDefinitionProvider? provider)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks += TicksPerDay;
			}
			while (IsWeekend(GetDayOfWeekFromTicks(ticks), weekend, provider));

			return new(ticks, dateTime.Kind);
		}
	}
}