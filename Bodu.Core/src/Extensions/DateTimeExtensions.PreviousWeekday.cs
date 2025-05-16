// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="PreviousWeekday.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the previous calendar weekday before the specified
		/// <paramref name="dateTime" />, based on the given <paramref name="weekend" /> definition.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search backward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that defines which days are considered part of the weekend.</param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" /> that
		/// represents the previous weekday according to the specified <paramref name="weekend" /> pattern.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined value of the <see cref="CalendarWeekendDefinition" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The method iterates backward one day at a time from <paramref name="dateTime" /> until it finds a day not considered part of the
		/// weekend under the specified <paramref name="weekend" /> rule.
		/// </para>
		/// <para>The resulting <see cref="DateTime" /> preserves the original time-of-day and <see cref="DateTime.Kind" /> of the input.</para>
		/// </remarks>
		public static DateTime PreviousWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks -= TicksPerDay;
			}
			while (IsWeekend(GetDayOfWeekFromTicks(ticks), weekend));

			return new(ticks, dateTime.Kind);
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the previous calendar weekday before the specified
		/// <paramref name="dateTime" />, based on the given <paramref name="weekend" /> rule and optional <paramref name="provider" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateTime" /> from which to search backward.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> that specifies the default weekend rule to apply.</param>
		/// <param name="provider">
		/// An optional <see cref="IWeekendDefinitionProvider" /> that can override the default rule when <paramref name="weekend" /> is set
		/// to <see cref="CalendarWeekendDefinition.Custom" />.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> with the same time-of-day and <see cref="DateTime.Kind" /> as <paramref name="dateTime" /> that
		/// represents the previous weekday according to the specified or custom weekend logic.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is not a defined value of the <see cref="CalendarWeekendDefinition" /> enumeration.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This overload supports both predefined and custom weekend definitions. If a custom rule is used, <paramref name="provider" />
		/// must be non-null.
		/// </para>
		/// <para>The resulting <see cref="DateTime" /> retains the original time-of-day and <see cref="DateTime.Kind" /> of the input.</para>
		/// </remarks>
		public static DateTime PreviousWeekday(this DateTime dateTime, CalendarWeekendDefinition weekend, IWeekendDefinitionProvider? provider)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(weekend);

			long ticks = dateTime.Ticks;
			do
			{
				ticks -= TicksPerDay;
			}
			while (IsWeekend(GetDayOfWeekFromTicks(ticks), weekend, provider));

			return new(ticks, dateTime.Kind);
		}
	}
}