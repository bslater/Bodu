// ---------------------------------------------------------------------------------------------------------------
// <copyright>
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
		private readonly DateTime fiscalYearStart;
		private readonly DayOfWeek firstDayOfWeek;

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

			this.fiscalYearStart = AlignToStartOfWeek(fiscalYearStart.Date, firstDayOfWeek);
			this.firstDayOfWeek = firstDayOfWeek;
		}

		/// <inheritdoc />
		/// <remarks>
		/// Each quarter spans 13 weeks. The method computes the number of weeks elapsed since the start of the fiscal year and maps it to a
		/// 1-based quarter. Only dates within the configured fiscal year are supported.
		/// </remarks>
		public int GetQuarter(DateTime dateTime)
		{
			var alignedDate = AlignToStartOfWeek(dateTime.Date, firstDayOfWeek);
			var totalWeeks = (int)((alignedDate - fiscalYearStart).TotalDays / 7);

			if (totalWeeks < 0)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, fiscalYearStart));

			int maxWeeks = Is53WeekFiscalYear() ? 53 : 52;

			if (totalWeeks >= maxWeeks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, fiscalYearStart));

			return Math.Min((totalWeeks / 13) + 1, 4);
		}

		/// <inheritdoc />
		/// <remarks>Calculates the start date of the quarter by offsetting from the fiscal year anchor by 13-week intervals.</remarks>
		public DateTime GetStartDate(DateTime dateTime)
		{
			int quarter = GetQuarter(dateTime);
			var start = fiscalYearStart.AddDays((quarter - 1) * 13 * 7);
			return new DateTime(start.Year, start.Month, start.Day, 0, 0, 0, dateTime.Kind);
		}

		/// <inheritdoc />
		/// <remarks>Returns the final calendar day of the quarter, which is always 91 days (13 weeks) after the quarter's start date.</remarks>
		public DateTime GetEndDate(DateTime dateTime)
		{
			var start = GetStartDate(dateTime);
			var end = start.AddDays((13 * 7) - 1);
			return new DateTime(end.Year, end.Month, end.Day, 0, 0, 0, dateTime.Kind);
		}

		/// <summary>
		/// Determines whether the configured fiscal year has 53 weeks instead of the standard 52 weeks.
		/// </summary>
		/// <returns><see langword="true" /> if the fiscal year includes 53 weeks; otherwise, <see langword="false" />.</returns>
		public bool Is53WeekFiscalYear()
		{
			var nextFiscalStart = fiscalYearStart.AddYears(1);
			var alignedNextFiscalStart = AlignToStartOfWeek(nextFiscalStart, firstDayOfWeek);
			var daysInYear = (alignedNextFiscalStart - fiscalYearStart).TotalDays;

			return daysInYear > 364; // 364 days = 52 weeks
		}

		/// <summary>
		/// Gets the total number of weeks in the fiscal year (52 or 53).
		/// </summary>
		/// <returns>The number of weeks in the fiscal year.</returns>
		public int GetWeeksInFiscalYear() => Is53WeekFiscalYear() ? 53 : 52;

		/// <summary>
		/// Aligns a date to the start of its week based on the configured start day.
		/// </summary>
		/// <param name="date">The date to align.</param>
		/// <param name="weekStart">The starting day of the week.</param>
		/// <returns>A <see cref="DateTime" /> aligned to the beginning of the week.</returns>
		private static DateTime AlignToStartOfWeek(DateTime date, DayOfWeek weekStart)
		{
			int diff = (7 + (date.DayOfWeek - weekStart)) % 7;
			return date.AddDays(-diff);
		}
	}
}