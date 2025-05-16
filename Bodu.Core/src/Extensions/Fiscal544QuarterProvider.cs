// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Fiscal544QuarterProvider.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides quarter logic based on a 5-4-4 retail fiscal calendar with optional 53-week year support.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This provider implements a fiscal calendar where each quarter consists of exactly 13 weeks, divided into three "months" of 5, 4, and
	/// 4 weeks respectively. The fiscal year begins on a configured anchor date and proceeds in 13-week blocks, with quarters defined as:
	/// </para>
	/// <list type="bullet">
	/// <item>
	/// <term>Q1</term>
	/// <description>Weeks 1–13</description>
	/// </item>
	/// <item>
	/// <term>Q2</term>
	/// <description>Weeks 14–26</description>
	/// </item>
	/// <item>
	/// <term>Q3</term>
	/// <description>Weeks 27–39</description>
	/// </item>
	/// <item>
	/// <term>Q4</term>
	/// <description>Weeks 40–52 or 53</description>
	/// </item>
	/// </list>
	/// <para>
	/// The fiscal year may optionally contain a 53rd week if the year spans more than 364 days, depending on the alignment between the
	/// anchor date and the calendar year boundaries.
	/// </para>
	/// <para>
	/// The start of each week is controlled by the specified <see cref="DayOfWeek" /> (commonly Sunday). The anchor date will be
	/// automatically aligned to the beginning of its week based on this value.
	/// </para>
	/// </remarks>
	public sealed class Fiscal544QuarterProvider
		: IQuarterDefinitionProvider
	{
		private readonly DayOfWeek firstDayOfWeek;
		private readonly long fiscalYearStartTicks;
		private readonly bool is53WeekFiscalYear;

		///// <summary>
		///// Initializes a new instance of the <see cref="Fiscal544QuarterProvider" /> class using a <see cref="DateOnly" /> anchor date
		///// aligned to the configured first day of the fiscal week.
		///// </summary>
		///// <param name="fiscalYearStart">
		///// The anchor date representing the start of the fiscal year (e.g., the Sunday nearest to February 1). This will be aligned
		///// backward to the nearest <paramref name="firstDayOfWeek" />.
		///// </param>
		///// <param name="firstDayOfWeek">
		///// The day of the week on which the fiscal week begins. Common values include <see cref="DayOfWeek.Sunday" /> or
		///// <see cref="DayOfWeek.Monday" />. Defaults to <see cref="DayOfWeek.Sunday" />.
		///// </param>
		///// <exception cref="ArgumentOutOfRangeException">
		///// Thrown when <paramref name="firstDayOfWeek" /> is not a valid <see cref="DayOfWeek" /> value.
		///// </exception>
		//public Fiscal544QuarterProvider(DateOnly fiscalYearStart, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
		//	: this(fiscalYearStart.ToDateTime(TimeOnly.MinValue), firstDayOfWeek) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="Fiscal544QuarterProvider" /> class using a <see cref="DateTime" /> anchor date
		/// aligned to the configured first day of the fiscal week.
		/// </summary>
		/// <param name="year">
		/// The anchor date representing the start of the fiscal year. The date portion is extracted and aligned to the start of the week
		/// defined by <paramref name="dayOfWeek" />.
		/// </param>
		/// <param name="month">
		/// The anchor date representing the start of the fiscal year. The date portion is extracted and aligned to the start of the week
		/// defined by <paramref name="dayOfWeek" />.
		/// </param>
		/// <param name="dayOfWeek">
		/// The day of the week on which the fiscal week begins. Common values include <see cref="DayOfWeek.Sunday" /> or
		/// <see cref="DayOfWeek.Monday" />. Defaults to <see cref="DayOfWeek.Sunday" />.
		/// </param>
		/// <param name="isFiscalYearEnd">
		/// The day of the week on which the fiscal week begins. Common values include <see cref="DayOfWeek.Sunday" /> or
		/// <see cref="DayOfWeek.Monday" />. Defaults to <see cref="DayOfWeek.Sunday" />.
		/// </param>
		/// <remarks>
		/// The <see cref="DateOnly" /> representation of the provided <paramref name="fiscalYearStart" /> is used to ensure consistent
		/// behavior across time zones and <see cref="DateTimeKind" /> settings.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="dayOfWeek" /> is not a valid <see cref="DayOfWeek" /> value.
		/// </exception>
		public Fiscal544QuarterProvider(int year, int month, DayOfWeek dayOfWeek = DayOfWeek.Saturday, bool isFiscalYearEnd = true)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);
			firstDayOfWeek = isFiscalYearEnd ? (DayOfWeek)(((int)dayOfWeek + 1) % 7) : dayOfWeek;
			fiscalYearStartTicks = AlignToNearestDayOfWeek(
				DateTimeExtensions.GetTicksForDate(year, month + (isFiscalYearEnd ? 1 : 0), 1),
				dayOfWeek);
			is53WeekFiscalYear = Is53WeekFiscal(fiscalYearStartTicks, year, month + (isFiscalYearEnd ? 1 : 0), firstDayOfWeek);
		}

		/// <summary>
		/// Gets a value indicating whether [is53 week fiscal year].
		/// </summary>
		/// <value><c>true</c> if [is53 week fiscal year]; otherwise, <c>false</c>.</value>
		public bool Is53WeekFiscalYear => is53WeekFiscalYear;

		/// <summary>
		/// Gets the total number of weeks in the fiscal year (52 or 53).
		/// </summary>
		public int WeeksInFiscalYear => is53WeekFiscalYear ? 53 : 52;

		/// <inheritdoc />
		public int GetQuarter(DateTime dateTime)
		{
			ValidateDateInFiscalYear(dateTime);
			int weeksFromStart = (int)((dateTime.Ticks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerWeek);
			int quarter = (weeksFromStart / 13) + 1;

			// Clamp to Q4 if 53-week year and within final week
			if (quarter > 4) quarter = 4;
			return quarter;
		}

		/// <inheritdoc />
		public int GetQuarter(DateOnly dateOnly) =>
			GetQuarter(dateOnly.ToDateTime(TimeOnly.MinValue));

		/// <inheritdoc />
		public DateTime GetQuarterEnd(DateTime dateTime)
		{
			int quarter = GetQuarter(dateTime);
			return GetQuarterEnd(quarter);
		}

		/// <inheritdoc />
		public DateTime GetQuarterEnd(int quarter)
		{
			DateTime start = GetQuarterStart(quarter);
			int weeks = (quarter < 4 || !is53WeekFiscalYear) ? 13 : 14;
			long endTicks = start.Ticks + (weeks * DateTimeExtensions.TicksPerWeek) - DateTimeExtensions.TicksPerDay;
			return new DateTime(DateTimeExtensions.GetDateAsTicks(endTicks), DateTimeKind.Unspecified);
		}

		/// <inheritdoc />
		public DateOnly GetQuarterEndDate(DateOnly dateOnly) =>
			GetQuarterEnd(dateOnly.ToDateTime(TimeOnly.MinValue)).ToDateOnly();

		/// <inheritdoc />
		public DateOnly GetQuarterEndDate(int quarter) =>
			GetQuarterEnd(quarter).ToDateOnly();

		/// <inheritdoc />
		public DateTime GetQuarterStart(DateTime dateTime)
		{
			ValidateDateInFiscalYear(dateTime);
			int quarter = GetQuarter(dateTime);
			return GetQuarterStart(quarter);
		}

		/// <inheritdoc />
		public DateTime GetQuarterStart(int quarter)
		{
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);
			long offsetWeeks = (quarter - 1) * 13L;
			long startTicks = fiscalYearStartTicks + offsetWeeks * DateTimeExtensions.TicksPerWeek;
			return new DateTime(DateTimeExtensions.GetDateAsTicks(startTicks), DateTimeKind.Unspecified);
		}

		/// <inheritdoc />
		public DateOnly GetQuarterStartDate(DateOnly dateOnly) =>
			GetQuarterStart(dateOnly.ToDateTime(TimeOnly.MinValue)).ToDateOnly();

		/// <inheritdoc />
		public DateOnly GetQuarterStartDate(int quarter) =>
			GetQuarterStart(quarter).ToDateOnly();

		/// <summary>
		/// Aligns a date to the start of the week.
		/// </summary>
		private static long AlignToNearestDayOfWeek(long ticks, DayOfWeek weekStart)
			=> DateTimeExtensions.GetNearestDayOfWeek(ticks, weekStart);

		/// <summary>
		/// Determines whether the configured fiscal year has 53 weeks.
		/// </summary>
		private static bool Is53WeekFiscal(long fiscalYearStartTicks, int year, int month, DayOfWeek firstDayOfWeek)
		{
			const int DaysIn52Weeks = 364;

			long unalignedNextYear = DateTimeExtensions.GetTicksForDate(year + 1, month, 1);

			// Align forward to the start of the next fiscal year week
			long alignedNextYear = AlignToNearestDayOfWeek(unalignedNextYear, firstDayOfWeek);

			// Total days in fiscal year
			long daysInYear = (alignedNextYear - fiscalYearStartTicks) / DateTimeExtensions.TicksPerDay;

			return daysInYear > DaysIn52Weeks;
		}

		/// <summary>
		/// Validates that the specified date is within the fiscal year range.
		/// </summary>
		private void ValidateDateInFiscalYear(DateTime dateTime)
		{
			long alignedTicks = dateTime.Ticks - DateTimeExtensions.GetPreviousDayOfWeekTicksFrom(dateTime.Ticks, firstDayOfWeek);
			long deltaDays = (alignedTicks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerDay;
			int fiscalYearLengthDays = is53WeekFiscalYear ? 371 : 364;

			if (deltaDays < 0 || deltaDays >= fiscalYearLengthDays)
			{
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, new DateTime(fiscalYearStartTicks, DateTimeKind.Unspecified)));
			}
		}
	}
}