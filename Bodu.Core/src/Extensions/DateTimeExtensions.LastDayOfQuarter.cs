// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="LastDayOfQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the last day of the specified calendar quarter in the given <paramref name="year" />.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="quarter">
		/// The quarter number within the year. Must be an integer between 1 and 4, inclusive, where 1 represents the first quarter (January
		/// to March) and 4 represents the final quarter (October to December).
		/// </param>
		/// <returns>A <see cref="DateTime" /> set to 00:00:00 on the last day of the specified quarter, with <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is not between 1 and 4 (inclusive).</exception>
		/// <remarks>
		/// <para>
		/// This method uses the standard <see cref="CalendarQuarterDefinition.JanuaryDecember" /> structure for calendar quarters. For
		/// alternate fiscal or custom quarter definitions, use an overload that accepts a <see cref="CalendarQuarterDefinition" /> or an <see cref="IQuarterDefinitionProvider" />.
		/// </para>
		/// </remarks>
		public static DateTime GetLastDayOfQuarter(int year, int quarter) =>
			GetLastDayOfQuarter(year, quarter, CalendarQuarterDefinition.JanuaryDecember);

		/// <summary>
		/// Returns the last day of the specified <paramref name="quarter" /> in the given <paramref name="year" />, using the provided
		/// quarter definition.
		/// </summary>
		/// <param name="year">
		/// The calendar year used as the base year for the quarter. Must be between the <c>Year</c> property values of
		/// <see cref="DateTime.MinValue" /> and <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="quarter">
		/// The quarter number within the year. Must be an integer between 1 and 4, inclusive, where 1 represents the first quarter and 4
		/// represents the final quarter.
		/// </param>
		/// <param name="definition">
		/// The <see cref="CalendarQuarterDefinition" /> that determines how quarters are aligned within the year (e.g., calendar or fiscal alignment).
		/// </param>
		/// <returns>A <see cref="DateTime" /> set to midnight (00:00:00) on the last day of the specified quarter, with <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported range of <see cref="DateTime" />, if <paramref name="quarter" /> is
		/// not between 1 and 4, or if <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />. Use an overload that accepts an
		/// <see cref="IQuarterDefinitionProvider" /> instead.
		/// </exception>
		public static DateTime GetLastDayOfQuarter(int year, int quarter, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			return new(ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition)), DateTimeKind.Unspecified);
		}

		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the last day of the calendar quarter for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose quarter is evaluated.</param>
		/// <returns>A <see cref="DateTime" /> set to 00:00:00 on the last day of the quarter, preserving the original <see cref="DateTime.Kind" />.</returns>
		/// <remarks>
		/// <para>This method uses the standard calendar quarter structure defined by <see cref="CalendarQuarterDefinition.JanuaryDecember" />:</para>
		/// <list type="bullet">
		/// <item>
		/// <description>Q1: January 1 – March 31</description>
		/// </item>
		/// <item>
		/// <description>Q2: April 1 – June 30</description>
		/// </item>
		/// <item>
		/// <description>Q3: July 1 – September 30</description>
		/// </item>
		/// <item>
		/// <description>Q4: October 1 – December 31</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTime LastDayOfQuarter(this DateTime dateTime) =>
			LastDayOfQuarterInternal(dateTime, CalendarQuarterDefinition.JanuaryDecember);

		/// <summary>
		/// Returns the last day of the quarter that includes the specified <paramref name="dateTime" />, based on the given <paramref name="definition" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> used to determine quarter boundaries.</param>
		/// <returns>A <see cref="DateTime" /> set to midnight on the last day of the applicable quarter, preserving the original <see cref="DateTime.Kind" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="definition" /> is not a defined value.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />. Use the provider-based overload instead.
		/// </exception>
		/// <remarks>
		/// Supports both month- and day-aligned definitions. For fiscal quarters not aligned to calendar months, use the overload accepting <see cref="IQuarterDefinitionProvider" />.
		/// </remarks>
		public static DateTime LastDayOfQuarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			return LastDayOfQuarterInternal(dateTime, definition);
		}

		/// <summary>
		/// Returns the last day of the quarter that includes the specified <paramref name="dateTime" />, using a custom provider.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="provider">An implementation of <see cref="IQuarterDefinitionProvider" /> that defines quarter boundaries.</param>
		/// <returns>A <see cref="DateTime" /> set to midnight on the last day of the applicable custom quarter, preserving the <see cref="DateTime.Kind" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the provider returns an invalid quarter end date.</exception>
		public static DateTime LastDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);
			return provider.GetQuarterEnd(dateTime);
		}

		/// <summary>
		/// Returns the last day of the quarter that includes the specified <paramref name="dateTime" />, using the provided quarter definition.
		/// </summary>
		/// <param name="dateTime">The date used to evaluate the quarter.</param>
		/// <param name="definition">The quarter definition, assumed to be valid and non-custom.</param>
		/// <returns>A <see cref="DateTime" /> set to midnight on the last day of the applicable quarter, preserving the <see cref="DateTime.Kind" />.</returns>
		/// <remarks>This method skips validation and is intended for internal use in trusted contexts.</remarks>
		private static DateTime LastDayOfQuarterInternal(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			return new(ComputeQuarterEndTicks(year, quarter, GetQuarterDefinition(definition)), dateTime.Kind);
		}
	}
}