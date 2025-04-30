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
	/// <para>This provider must be explicitly instantiated and passed to quarter-related extension methods such as:</para>
	/// <code language="csharp">
	///<![CDATA[
	///var provider = new Fiscal544QuarterProvider(
	///fiscalYearStart: new DateTime(2025, 1, 26), // Sunday closest to Feb 1
	///firstDayOfWeek: DayOfWeek.Sunday
	///);
	///
	///int quarter = someDate.Quarter(provider);
	///DateTime start = someDate.FirstDayOfQuarter(provider);
	///]]>
	/// </code>
	/// <para>
	/// These methods use the logic in <see cref="IQuarterDefinitionProvider" /> to determine the quarter number and boundaries. This
	/// approach avoids modifying enum-based logic and is fully extensible.
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
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the specified <paramref name="dateTime" /> falls outside the bounds of the fiscal year defined by the
		/// <c>fiscalYearStart</c>. The date must be within the range of week 1 to week 52 (or 53) of the fiscal year. This provider
		/// instance only supports a single fiscal year beginning at the configured start date. Use a separate provider instance for dates
		/// in other fiscal years.
		/// </exception>
		public int GetQuarter(DateTime dateTime)
		{
			var alignedDate = AlignToStartOfWeek(dateTime.Date, firstDayOfWeek);
			var totalWeeks = (int)((alignedDate - fiscalYearStart).TotalDays / 7);

			if (totalWeeks < 0)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, this.fiscalYearStart));

			int maxWeeks = Is53WeekFiscalYear() ? 53 : 52;

			if (totalWeeks >= maxWeeks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, this.fiscalYearStart));

			return Math.Min((totalWeeks / 13) + 1, 4);
		}

		/// <inheritdoc />
		public int GetStartMonthFromQuarter(int quarter)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(quarter, 1, 4);

			var startOfQuarter = fiscalYearStart.AddDays((quarter - 1) * 13 * 7);
			return startOfQuarter.Month;
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