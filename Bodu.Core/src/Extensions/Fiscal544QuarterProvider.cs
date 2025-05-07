// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fiscal544QuarterProvider.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
	public sealed class Fiscal544QuarterProvider : IQuarterDefinitionProvider
	{
		private readonly DayOfWeek firstDayOfWeek;
		private readonly long fiscalYearStartTicks;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fiscal544QuarterProvider" /> class.
		/// </summary>
		/// <param name="fiscalYearStart">
		/// The anchor date representing the start of the fiscal year (e.g., the Sunday nearest to February 1). This date will be aligned to
		/// the start of the fiscal week using <paramref name="firstDayOfWeek" />.
		/// </param>
		/// <param name="firstDayOfWeek">The day of the week on which the fiscal week begins (typically Sunday or Monday).</param>
		public Fiscal544QuarterProvider(DateTime fiscalYearStart, DayOfWeek firstDayOfWeek = DayOfWeek.Sunday)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(firstDayOfWeek);

			fiscalYearStartTicks = AlignToStartOfWeek(fiscalYearStart.Date, firstDayOfWeek).Ticks;
			this.firstDayOfWeek = firstDayOfWeek;
		}

		/// <inheritdoc />
		public DateTime GetEndDate(DateTime dateTime)
		{
			ValidateDateInFiscalYear(dateTime);
			long deltaWeeks = (dateTime.Ticks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStartTicks + (deltaWeeks / 13) * 13L * DateTimeExtensions.TicksPerWeek;
			long endTicks = startTicks + 13L * DateTimeExtensions.TicksPerWeek - DateTimeExtensions.TicksPerDay;

			endTicks = DateTimeExtensions.GetDateAsTicks(endTicks);
			return new DateTime(endTicks, dateTime.Kind);
		}

		/// <inheritdoc />
		public DateTime GetEndDate(int quarter)
		{
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);

			long startTicks = GetStartDate(quarter).Ticks;
			long endTicks = startTicks + 13L * DateTimeExtensions.TicksPerWeek - DateTimeExtensions.TicksPerDay;

			endTicks = DateTimeExtensions.GetDateAsTicks(endTicks);
			return new DateTime(endTicks, DateTimeKind.Unspecified);
		}

		/// <inheritdoc />
		public int GetQuarter(DateTime dateTime)
		{
			ValidateDateInFiscalYear(dateTime);
			int totalWeeks = (int)((dateTime.Ticks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerWeek);
			return Math.Min((totalWeeks / 13) + 1, 4);
		}

		/// <inheritdoc />
		public DateTime GetStartDate(DateTime dateTime)
		{
			ValidateDateInFiscalYear(dateTime);
			long deltaWeeks = (dateTime.Ticks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStartTicks + (deltaWeeks / 13) * 13L * DateTimeExtensions.TicksPerWeek;

			startTicks = DateTimeExtensions.GetDateAsTicks(startTicks);
			return new DateTime(startTicks, dateTime.Kind);
		}

		/// <inheritdoc />
		public DateTime GetStartDate(int quarter)
		{
			ThrowHelper.ThrowIfOutOfRange(quarter, 1, 4);

			long offsetTicks = (quarter - 1) * 13L * DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStartTicks + offsetTicks;

			startTicks = DateTimeExtensions.GetDateAsTicks(startTicks);
			return new DateTime(startTicks, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// Gets the total number of weeks in the fiscal year (52 or 53).
		/// </summary>
		public int GetWeeksInFiscalYear() => Is53WeekFiscalYear() ? 53 : 52;

		/// <summary>
		/// Determines whether the configured fiscal year has 53 weeks instead of the standard 52 weeks.
		/// </summary>
		public bool Is53WeekFiscalYear()
		{
			const int DaysIn52Weeks = 364;

			// Tentative next fiscal start (after 52 weeks)
			var unalignedNextYearTicks = fiscalYearStartTicks + (DateTimeExtensions.TicksPerDay * DaysIn52Weeks);

			// Align forward to the next fiscal week start
			var alignedNextYearTicks = unalignedNextYearTicks + DateTimeExtensions.GetNextDayOfWeekTicksFrom(unalignedNextYearTicks, firstDayOfWeek);

			var daysInYear = (alignedNextYearTicks - fiscalYearStartTicks) / DateTimeExtensions.TicksPerDay;
			return daysInYear > DaysIn52Weeks;
		}

		/// <summary>
		/// Validates that the specified date falls within the configured fiscal year range.
		/// </summary>
		private void ValidateDateInFiscalYear(DateTime dateTime)
		{
			var alignedDate = dateTime.Date.Ticks - DateTimeExtensions.GetPreviousDayOfWeekTicksFrom(dateTime.Date.Ticks, firstDayOfWeek);
			int fiscalYearLengthDays = Is53WeekFiscalYear() ? 371 : 364;

			long deltaDays = (alignedDate - fiscalYearStartTicks) / TimeSpan.TicksPerDay;

			if (deltaDays < 0 || deltaDays >= fiscalYearLengthDays)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, new DateTime(fiscalYearStartTicks)));
		}

		/// <summary>
		/// Aligns a date to the start of its week based on the configured start day.
		/// </summary>
		private static DateTime AlignToStartOfWeek(DateTime date, DayOfWeek weekStart)
		{
			long ticks = date.Date.Ticks - DateTimeExtensions.GetPreviousDayOfWeekTicksFrom(date.Date.Ticks, weekStart);
			return new DateTime(ticks, date.Kind);
		}
	}
}