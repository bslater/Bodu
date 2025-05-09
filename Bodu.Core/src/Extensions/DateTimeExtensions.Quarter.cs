﻿// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.Quarter.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the quarter number (1–4) of the year for the specified <see cref="DateTime" />, using the standard calendar quarter
		/// definition ( <see cref="CalendarQuarterDefinition.JanuaryDecember" />).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns>An integer between 1 and 4 representing the calendar quarter that includes <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses the standard Gregorian calendar quarter structure aligned to the calendar year:
		/// <list type="bullet">
		/// <item>
		/// <term>Q1</term>
		/// <description>1 January – 31 March</description>
		/// </item>
		/// <item>
		/// <term>Q2</term>
		/// <description>1 April – 30 June</description>
		/// </item>
		/// <item>
		/// <term>Q3</term>
		/// <description>1 July – 30 September</description>
		/// </item>
		/// <item>
		/// <term>Q4</term>
		/// <description>1 October – 31 December</description>
		/// </item>
		/// </list>
		/// The quarter number returned is based on the month of <paramref name="dateTime" />, regardless of the day or time.
		/// </remarks>
		public static int Quarter(this DateTime dateTime)
			=> Quarter(dateTime, CalendarQuarterDefinition.JanuaryDecember);

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using the given
		/// <see cref="CalendarQuarterDefinition" /> to determine the fiscal calendar structure.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="definition">Specifies the quarter definition used to determine the quarter.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid value of the <see cref="CalendarQuarterDefinition" /> enum, or if it is
		/// <see cref="CalendarQuarterDefinition.Custom" />. Use the <see cref="Quarter(DateTime, IQuarterDefinitionProvider)" /> overload
		/// to support custom definitions.
		/// </exception>
		/// <remarks>
		/// This method supports predefined quarter structures aligned to calendar or financial years. The result is based on adjusting the
		/// input month by the definition offset and mapping the result to a 1-based quarter.
		/// </remarks>
		public static int Quarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			return GetQuarterForDate(dateTime, GetQuarterDefinition(definition));
		}

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using a custom
		/// <see cref="IQuarterDefinitionProvider" /> implementation.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="provider">
		/// An implementation of <see cref="IQuarterDefinitionProvider" /> that determines the quarter based on custom logic.
		/// </param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the quarter value returned by the provider is outside the valid range of 1 through 4.
		/// </exception>
		/// <remarks>
		/// Use this overload to support advanced or non-standard fiscal calendars, such as 4-4-5 financial periods, retail accounting
		/// calendars, or region-specific quarter models not covered by <see cref="CalendarQuarterDefinition" />.
		/// <para>
		/// This method delegates to <see cref="IQuarterDefinitionProvider.GetQuarter" /> and can be used in conjunction with
		/// <see cref="FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> and
		/// <see cref="LastDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> for complete custom quarter boundary resolution.
		/// </para>
		/// </remarks>
		public static int Quarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			int quarter = provider.GetQuarter(dateTime);

			if (quarter is < 1 or > 4)
				throw new ArgumentOutOfRangeException(nameof(provider), ResourceStrings.Arg_OutOfRange_InvalidQuarterNumber);

			return quarter;
		}

		/// <summary>
		/// Computes the tick value for the last day of the specified quarter based on a custom month-day anchor definition.
		/// </summary>
		/// <param name="year">The fiscal or calendar year in which the quarter ends.</param>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <param name="definition">
		/// A tuple representing the anchor month and day that define the start of Q1 (e.g., (4, 6) for April 6).
		/// </param>
		/// <returns>The number of ticks representing midnight at the start of the day *after* the last day of the quarter.</returns>
		/// <remarks>
		/// The returned value subtracts one day from the start of the next quarter to yield the final calendar day of the current quarter.
		/// This accounts for wrap-around years when the end of Q4 exceeds the anchor month.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long ComputeQuarterEndTicks(
			int year,
			int quarter,
			(uint defMonth, uint defDay) definition)
		{
			uint q = (uint)(quarter - 1);

			// Calculate the month where this quarter ends
			uint endMonth = (((q + 1U) * 3U + definition.defMonth - 1U) % 12U) + 1U;

			// If the end month is less than or equal to the anchor month, it also wraps
			int endYear = (endMonth <= definition.defMonth) ? year + 1 : year;

			// Return ticks
			return DateTimeExtensions.GetTicksForDate(endYear, (int)endMonth, (int)definition.defDay) - DateTimeExtensions.TicksPerDay;
		}

		/// <summary>
		/// Computes the tick value for the first day of the specified quarter based on a custom month-day anchor definition.
		/// </summary>
		/// <param name="year">The fiscal or calendar year in which the quarter starts.</param>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <param name="definition">
		/// A tuple representing the anchor month and day that define the start of Q1 (e.g., (4, 6) for April 6).
		/// </param>
		/// <returns>The number of ticks representing midnight on the first day of the specified quarter.</returns>
		/// <remarks>
		/// The method accounts for non-standard quarter anchors that may wrap into the following calendar year. For example, a fiscal year
		/// starting in October will have Q1 starting in October of the previous year.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static long ComputeQuarterStartTicks(
			int year,
			int quarter,
			(uint defMonth, uint defDay) definition)
		{
			uint q = (uint)(quarter - 1);

			// Calculate the month where this quarter starts
			uint startMonth = ((q * 3U + definition.defMonth - 1U) % 12U) + 1U;

			// If the start month is before the anchor month, it wraps into the *next calendar year*
			int startYear = (startMonth < definition.defMonth) ? year + 1 : year;

			// Return ticks
			return DateTimeExtensions.GetTicksForDate(startYear, (int)startMonth, (int)definition.defDay);
		}

		/// <summary>
		/// Determines the fiscal or calendar year and quarter number for a given <see cref="DateTime" /> based on a custom quarter definition.
		/// </summary>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines how quarters are anchored.</param>
		/// <param name="referenceDate">The date to evaluate.</param>
		/// <returns>A tuple containing the resolved <c>Year</c> and <c>Quarter</c> (1–4) that include the <paramref name="referenceDate" />.</returns>
		/// <remarks>
		/// This method supports both standard and non-standard quarter definitions by calculating the appropriate year and quarter number
		/// for the given <paramref name="referenceDate" />. If the computed start date of the quarter is after the reference date, the year
		/// is decremented to represent the previous fiscal year.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static (int Year, int Quarter) GetQuarterAndYearFromDate(CalendarQuarterDefinition definition, DateTime referenceDate)
		{
			var defMonthDay = GetQuarterDefinition(definition);

			// Determine the quarter number (1–4) for the provided reference date
			int q = GetQuarterForDate(referenceDate, defMonthDay);
			int y = referenceDate.Year;

			// Compute the actual calendar month when the resolved quarter starts
			uint startMonth = ((uint)(q - 1) * 3U + defMonthDay.defMonth - 1U) % 12U + 1U;

			// Adjust the start year if the start month falls in the next calendar year
			int startYear = (startMonth < defMonthDay.defMonth) ? y + 1 : y;

			// Calculate the tick count for the quarter start date
			long startTicks = DateTimeExtensions.GetTicksForDate(startYear, (int)startMonth, (int)defMonthDay.defDay);

			// If the start date is after the reference date, back up to the previous fiscal year
			if (startTicks > referenceDate.Ticks)
				y--;

			return (y, q);
		}

		/// <summary>
		/// Extracts the anchor month and day components from a <see cref="CalendarQuarterDefinition" /> value.
		/// </summary>
		/// <param name="definition">A <see cref="CalendarQuarterDefinition" /> enum value encoded as MMDD (e.g., 401 for 1 April).</param>
		/// <returns>A tuple <c>(defMonth, defDay)</c> representing the anchor month and day of Q1.</returns>
		/// <remarks>Used as a normalized form of the quarter definition for modular math calculations.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static (uint defMonth, uint defDay) GetQuarterDefinition(CalendarQuarterDefinition definition)
		{
			// Unpack MMDD value: e.g., 406 = 4/06
			uint def = (uint)definition;
			uint defMonth = def / 100U;
			uint defDay = def % 100U;

			return new(defMonth, defDay);
		}

		/// <summary>
		/// Determines the fiscal or calendar quarter number (1–4) that contains the specified <see cref="DateTime" />, based on the
		/// supplied quarter definition anchor.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="definition">
		/// A tuple representing the anchor month and day (e.g., (4, 1) for April 1) that defines the start of Q1.
		/// </param>
		/// <returns>An integer in the range [1, 4] representing the resolved quarter number.</returns>
		/// <remarks>If the date falls on the first month of a quarter but before the anchor day, it is assigned to the previous quarter.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetQuarterForDate(this DateTime dateTime, (uint defMonth, uint defDay) definition)
		{
			// Compute quarter number using modular offset from anchor month
			int quarter = ((dateTime.Month + 12 - (int)definition.defMonth) % 12) / 3 + 1;

			// If anchor day is not the 1st, check if we are in the quarter's start month but still before the anchor day — in that case, we
			// belong to the previous quarter
			if (definition.defDay != 1)
			{
				// Compute the actual start month for the resolved quarter
				uint quarterStartMonth = (((uint)(quarter - 1) * 3U + definition.defMonth - 1U) % 12U) + 1U;

				// If we're in the quarter's start month but before the anchor day, back up a quarter
				if ((uint)dateTime.Month == quarterStartMonth && (uint)dateTime.Day < definition.defDay)
					quarter = quarter == 1 ? 4 : quarter - 1;
			}

			return quarter;
		}
	}
}