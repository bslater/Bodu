﻿// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="FirstDayOfQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the calendar quarter that contains the specified instance,
		/// using the standard calendar quarter definition.
		/// </summary>
		/// <param name="dateTime">The date and time value used to determine the calendar quarter.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the quarter containing <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// <para>This method uses the standard calendar alignment defined by <see cref="CalendarQuarterDefinition.JanuaryToDecember" />.</para>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance matches that of the original <paramref name="dateTime" />.</para>
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime) =>
			FirstDayOfQuarterInternal(dateTime, CalendarQuarterDefinition.JanuaryToDecember);

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the quarter that contains the specified instance, using the
		/// specified calendar quarter definition.
		/// </summary>
		/// <param name="dateTime">The date and time value used to determine the quarter.</param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> used to determine quarter boundaries.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the corresponding quarter.</returns>
		/// <remarks>
		/// <para>
		/// The <paramref name="definition" /> controls whether quarters are aligned by month (e.g., Jan–Mar) or anchored to custom
		/// day-based boundaries.
		/// </para>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance matches that of the original <paramref name="dateTime" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="definition" /> is not a valid enumeration value.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />. Use the provider-based overload instead.
		/// </exception>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			return FirstDayOfQuarterInternal(dateTime, definition);
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the quarter that contains the specified instance, using a
		/// custom <see cref="IQuarterDefinitionProvider" />.
		/// </summary>
		/// <param name="dateTime">The date and time value used to determine the quarter.</param>
		/// <param name="provider">An implementation of <see cref="IQuarterDefinitionProvider" /> defining custom quarter boundaries.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the quarter containing <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance matches that of the original <paramref name="dateTime" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the provider returns a date outside the range of <see cref="DateTime.MinValue" /> and <see cref="DateTime.MaxValue" />.
		/// </exception>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			return provider.GetQuarterStart(dateTime);
		}

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the specified calendar quarter in the given year, using the
		/// standard calendar quarter definition.
		/// </summary>
		/// <param name="year">The calendar year to evaluate. Must be between 1 and 9999, inclusive.</param>
		/// <param name="quarter">The quarter number, from 1 (Jan–Mar) to 4 (Oct–Dec).</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the specified quarter and year.</returns>
		/// <remarks>
		/// <para>This method uses the standard calendar alignment defined by <see cref="CalendarQuarterDefinition.JanuaryToDecember" />.</para>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance is <see cref="DateTimeKind.Unspecified" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported range,
		/// -or- if <paramref name="quarter" /> is less than 1 or greater than 4.
		/// </exception>
		public static DateTime GetFirstDayOfQuarter(int year, int quarter) =>
			GetFirstDayOfQuarter(year, quarter, CalendarQuarterDefinition.JanuaryToDecember);

		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the first day of the specified quarter and year, using the provided quarter definition.
		/// </summary>
		/// <param name="year">The calendar year to evaluate. Must be between 1 and 9999, inclusive.</param>
		/// <param name="quarter">The quarter number, from 1 to 4.</param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines how quarters are aligned.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the specified quarter.</returns>
		/// <remarks>
		/// <para>The <see cref="DateTime.Kind" /> property of the returned instance is <see cref="DateTimeKind.Unspecified" />.</para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported range,
		/// -or- if <paramref name="quarter" /> is less than 1 or greater than 4,
		/// -or- if <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />. Use a provider-based overload instead.
		/// </exception>
		public static DateTime GetFirstDayOfQuarter(int year, int quarter, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			return new(ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition)), DateTimeKind.Unspecified);
		}

		/// <summary>
		/// Returns the first day of the quarter that contains the specified <see cref="DateTime" />, using a prevalidated quarter definition.
		/// </summary>
		/// <param name="dateTime">The reference date.</param>
		/// <param name="definition">A valid <see cref="CalendarQuarterDefinition" /> that is not <see cref="CalendarQuarterDefinition.Custom" />.</param>
		/// <returns>An object whose value is set to midnight (00:00:00) on the first day of the quarter.</returns>
		/// <remarks>This method performs no validation and should be used only when the input definition is known to be valid.</remarks>
		private static DateTime FirstDayOfQuarterInternal(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			return new(ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition)), dateTime.Kind);
		}
	}
}