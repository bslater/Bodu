// // --------------------------------------------------------------------------------------------------------------- //
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
		/// Returns the first day of the calendar quarter that contains the specified <see cref="DateTime" />, using the standard calendar
		/// quarter definition.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the quarter, with the time set to midnight (00:00:00) and the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// This method uses the standard calendar quarter system:
		/// <list type="bullet">
		/// <item>
		/// <description>Q1: January–March</description>
		/// </item>
		/// <item>
		/// <description>Q2: April–June</description>
		/// </item>
		/// <item>
		/// <description>Q3: July–September</description>
		/// </item>
		/// <item>
		/// <description>Q4: October–December</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime) =>
			FirstDayOfQuarterInternal(dateTime, CalendarQuarterDefinition.JanuaryDecember);

		/// <summary>
		/// Returns the first day of the quarter that contains the specified <see cref="DateTime" />, using the specified quarter definition.
		/// </summary>
		/// <param name="dateTime">The reference date.</param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> to use for determining quarter boundaries.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the quarter, with the time set to midnight (00:00:00) and the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// <para>Supports both month- and day-aligned quarter definitions. For custom systems, use the provider-based overload.</para>
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
		/// Returns the first day of the quarter containing the specified <see cref="DateTime" />, using a custom <see cref="IQuarterDefinitionProvider" />.
		/// </summary>
		/// <param name="dateTime">The reference date.</param>
		/// <param name="provider">An implementation of <see cref="IQuarterDefinitionProvider" /> that defines custom quarter logic.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the quarter, with the time set to midnight (00:00:00) and the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>This method is used for fiscal calendars such as 4-4-5 or other region-specific quarter systems.</remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the provider returns an invalid quarter boundary.</exception>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			return provider.GetQuarterStart(dateTime);
		}

		/// <summary>
		/// Returns the first day of the specified calendar quarter in the given year, using the standard calendar quarter definition.
		/// </summary>
		/// <param name="year">
		/// The calendar year to evaluate. Must be between the <c>Year</c> property values of <see cref="DateTime.MinValue" /> and
		/// <see cref="DateTime.MaxValue" />, inclusive.
		/// </param>
		/// <param name="quarter">
		/// The quarter number within the year. Must be an integer between 1 and 4, inclusive, where 1 represents the first quarter (January
		/// to March), and 4 represents the last quarter (October to December).
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the specified quarter, with the time set to midnight (00:00:00) and <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <remarks>See <see cref="CalendarQuarterDefinition.JanuaryDecember" /> for standard quarter alignment.</remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is less than 1 or greater than 4.</exception>
		public static DateTime GetFirstDayOfQuarter(int year, int quarter) =>
			GetFirstDayOfQuarter(year, quarter, CalendarQuarterDefinition.JanuaryDecember);

		/// <summary>
		/// Returns the first day of the specified quarter and year, using the provided quarter definition.
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
		/// The <see cref="CalendarQuarterDefinition" /> value that determines how quarters are aligned within the year (e.g.,
		/// calendar-aligned or fiscal-aligned).
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the quarter, with the time set to midnight (00:00:00) and <see cref="DateTimeKind.Unspecified" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// Month-aligned definitions (e.g., January, April, etc.) start each quarter on the 1st of the month. Day-aligned definitions
		/// (e.g., MMDD-style) support anchored day-of-month starts for each quarter.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="year" /> is outside the supported <see cref="DateTime" /> year range, if <paramref name="quarter" />
		/// is not between 1 and 4, or if <paramref name="definition" /> is not a valid enumeration value.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />, which requires an external <see cref="IQuarterDefinitionProvider" />.
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
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the quarter, with the time set to midnight (00:00:00) and the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>This method performs no validation and should be used only when the input definition is known to be valid.</remarks>
		private static DateTime FirstDayOfQuarterInternal(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			return new(ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition)), dateTime.Kind);
		}
	}
}