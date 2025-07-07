﻿// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Quarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Gets the quarter number (1–4) of the year for the specified <see cref="DateTime" />, using the standard calendar quarter
		/// definition ( <see cref="CalendarQuarterDefinition.JanuaryToDecember" />).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>An integer between 1 and 4 representing the calendar quarter that contains <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// <para>This method uses the standard calendar alignment defined by <see cref="CalendarQuarterDefinition.JanuaryToDecember" />.</para>
		/// </remarks>
		public static int Quarter(this DateTime dateTime) =>
			GetQuarterForDate(dateTime, GetQuarterDefinition(CalendarQuarterDefinition.JanuaryToDecember));

		/// <summary>
		/// Gets the quarter number (1–4) for the specified <see cref="DateTime" />, using a predefined <see cref="CalendarQuarterDefinition" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="definition">The quarter definition that determines how the year is segmented into quarters.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that contains the given <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid enum value or is <see cref="CalendarQuarterDefinition.Custom" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method supports various standard calendar and financial quarter alignments by unpacking the encoded anchor month and day.
		/// </para>
		/// <para>For custom models, use <see cref="Quarter(DateTime, IQuarterDefinitionProvider)" /> instead.</para>
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
		/// Gets the quarter number (1–4) for the specified <see cref="DateTime" />, using a custom quarter definition provider.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="provider">An <see cref="IQuarterDefinitionProvider" /> that defines how quarters are structured.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that contains <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the quarter returned by the provider is not in the range 1–4.</exception>
		/// <remarks>
		/// <para>Use this overload to support complex fiscal models such as 4-4-5 retail calendars or non-month-aligned systems.</para>
		/// <para>This method calls <see cref="IQuarterDefinitionProvider.GetQuarter(DateTime)" /> to resolve the quarter.</para>
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
		/// Computes the tick value for the end of the specified quarter, based on a month-day anchor definition.
		/// </summary>
		/// <param name="year">The year in which the quarter ends.</param>
		/// <param name="quarter">The 1-based quarter number (1–4).</param>
		/// <param name="definition">
		/// A tuple representing the anchor month and day that define the start of Q1 (e.g., (4, 6) for April 6).
		/// </param>
		/// <returns>The number of ticks representing midnight on the last day of the quarter.</returns>
		/// <remarks>This is calculated by subtracting one day from the start of the next quarter.</remarks>
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
			return GetTicksForDate(endYear, (int)endMonth, (int)definition.defDay) - TicksPerDay;
		}

		/// <summary>
		/// Computes the tick value for the start of the specified quarter, based on a month-day anchor definition.
		/// </summary>
		/// <param name="year">The year in which the quarter begins.</param>
		/// <param name="quarter">The 1-based quarter number (1–4).</param>
		/// <param name="definition">
		/// A tuple representing the anchor month and day that define the start of Q1 (e.g., (4, 6) for April 6).
		/// </param>
		/// <returns>The number of ticks representing midnight on the first day of the quarter.</returns>
		/// <remarks>Accounts for wrap-around years when the anchor month begins in the latter part of the year.</remarks>
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
			return GetTicksForDate(startYear, (int)startMonth, (int)definition.defDay);
		}

		/// <summary>
		/// Determines the fiscal year and quarter that include the specified <paramref name="referenceDate" /> using the given quarter definition.
		/// </summary>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines quarter anchor points.</param>
		/// <param name="referenceDate">The date to evaluate.</param>
		/// <returns>A tuple containing the resolved year and quarter number (1–4).</returns>
		/// <remarks>
		/// If the calculated start of the resolved quarter is after <paramref name="referenceDate" />, the year is decremented to reflect
		/// the prior fiscal year.
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
			long startTicks = GetTicksForDate(startYear, (int)startMonth, (int)defMonthDay.defDay);

			// If the start date is after the reference date, back up to the previous fiscal year
			if (startTicks > referenceDate.Ticks)
				y--;

			return (y, q);
		}

		/// <summary>
		/// Extracts the anchor month and day components from a <see cref="CalendarQuarterDefinition" /> value.
		/// </summary>
		/// <param name="definition">A <see cref="CalendarQuarterDefinition" /> value encoded as MMDD (e.g., 406 for April 6).</param>
		/// <returns>A tuple <c>(defMonth, defDay)</c> representing the anchor month and day that define the start of Q1.</returns>
		/// <remarks>This method unpacks the encoded <see cref="CalendarQuarterDefinition" /> value for internal use in modular calculations.</remarks>
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
		/// Determines the quarter number (1–4) that includes the specified <see cref="DateTime" />, based on a month-day anchor definition.
		/// </summary>
		/// <param name="dateTime">The date to evaluate.</param>
		/// <param name="definition">A tuple representing the start of Q1, encoded as (month, day).</param>
		/// <returns>An integer between 1 and 4 representing the resolved quarter number.</returns>
		/// <remarks>
		/// If <paramref name="dateTime" /> falls before the anchor day in the quarter's first month, it is considered part of the previous quarter.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetQuarterForDate(this DateTime dateTime, (uint defMonth, uint defDay) definition)
		{
			// Compute quarter number using modular offset from anchor month
			int quarter = ((dateTime.Month + 12 - (int)definition.defMonth) % 12) / 3 + 1;

			// If anchor day is not the 1st, check if we are in the quarter's start month but still before the anchor day - in that case, we
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