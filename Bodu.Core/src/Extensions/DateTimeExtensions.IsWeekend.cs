// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.IsWeekend.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> falls on a weekend using the default
		/// <see cref="CalendarWeekendDefinition.SaturdaySunday" /> rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns><see langword="true" /> if the <paramref name="dateTime" /> occurs on Saturday or Sunday; otherwise, <see langword="false" />.</returns>
		/// <remarks>This method assumes the standard Western weekend definition, where weekends are Saturday and Sunday.</remarks>
		public static bool IsWeekend(this DateTime dateTime) =>
			IsWeekend(dateTime.DayOfWeek, CalendarWeekendDefinition.SaturdaySunday, null);

		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> falls on a weekend, based on the provided
		/// <see cref="CalendarWeekendDefinition" /> rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="weekend">
		/// The <see cref="CalendarWeekendDefinition" /> value that defines which days are considered part of the weekend.
		/// </param>
		/// <param name="provider">
		/// An optional custom <see cref="IWeekendDefinitionProvider" /> used when <paramref name="weekend" /> is <see cref="CalendarWeekendDefinition.Custom" />.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the <paramref name="dateTime" /> occurs on a weekend day according to the specified rule or custom
		/// provider; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is <see cref="CalendarWeekendDefinition.Custom" /> but no <paramref name="provider" /> is supplied.
		/// </exception>
		/// <remarks>
		/// Use this method to evaluate weekend status under different global or cultural definitions, such as Friday/Saturday in the Middle
		/// East or Sunday-only in some business contexts.
		/// </remarks>
		public static bool IsWeekend(this DateTime dateTime, CalendarWeekendDefinition weekend, IWeekendDefinitionProvider? provider = null) =>
			IsWeekend(dateTime.DayOfWeek, weekend, provider);

		/// <summary>
		/// Evaluates whether the specified <see cref="DayOfWeek" /> is considered part of the weekend using the given rule or provider.
		/// </summary>
		/// <param name="dayOfWeek">The <see cref="DayOfWeek" /> value to check.</param>
		/// <param name="weekend">The <see cref="CalendarWeekendDefinition" /> rule to apply.</param>
		/// <param name="provider">A custom <see cref="IWeekendDefinitionProvider" /> used when the rule is <see cref="CalendarWeekendDefinition.Custom" />.</param>
		/// <returns><see langword="true" /> if the <paramref name="dayOfWeek" /> falls on a weekend day; otherwise, <see langword="false" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is <see cref="CalendarWeekendDefinition.Custom" /> and <paramref name="provider" /> is null.
		/// </exception>
		public static bool IsWeekend(DayOfWeek dayOfWeek, CalendarWeekendDefinition weekend, IWeekendDefinitionProvider? provider = null)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return weekend switch
			{
				CalendarWeekendDefinition.SaturdaySunday => dayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday,
				CalendarWeekendDefinition.FridaySaturday => dayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday,
				CalendarWeekendDefinition.ThursdayFriday => dayOfWeek is DayOfWeek.Thursday or DayOfWeek.Friday,
				CalendarWeekendDefinition.SundayOnly => dayOfWeek == DayOfWeek.Sunday,
				CalendarWeekendDefinition.FridayOnly => dayOfWeek == DayOfWeek.Friday,
				CalendarWeekendDefinition.Custom when provider is not null => provider.IsWeekend(dayOfWeek),
				_ => throw new ArgumentOutOfRangeException(
					nameof(weekend),
					string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(CalendarWeekendDefinition).Name))
			};
		}
	}
}