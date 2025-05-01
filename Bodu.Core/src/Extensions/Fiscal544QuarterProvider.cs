// ---------------------------------------------------------------------------------------------------------------
// <copyright>
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
		private readonly DateTime fiscalYearStart;

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
		/// This method is used by <see cref="DateTimeExtensions.LastDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> to compute the
		/// final calendar date of the quarter containing the specified date.
		/// <para>The end date is calculated as 13 weeks (91 days) after the start of the fiscal quarter, minus 1 day to ensure inclusivity.</para>
		/// </remarks>
		/// <param name="dateTime">A <see cref="DateTime" /> within the fiscal year to resolve to a quarter end date.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the final day of the quarter, preserving the <paramref name="dateTime" /> kind.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="dateTime" /> is outside the configured fiscal year.</exception>
		public DateTime GetEndDate(DateTime dateTime)
		{
			var alignedDate = ValidateDateInFiscalYear(dateTime);
			long deltaWeeks = (alignedDate.Ticks - fiscalYearStart.Ticks) / DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStart.Ticks + ((deltaWeeks / 13) * 13L * DateTimeExtensions.TicksPerWeek);
			long endTicks = startTicks + (13L * DateTimeExtensions.TicksPerWeek) - DateTimeExtensions.TicksPerDay;

			endTicks = DateTimeExtensions.GetDateAsTicks(endTicks);
			return new DateTime(endTicks, dateTime.Kind);
		}

		/// <inheritdoc />
		/// <remarks>Calculates the final day of the specified quarter by adding 13 weeks (91 days) to the quarter's start date, inclusive.</remarks>
		/// <param name="quarter">The fiscal quarter number (1–4).</param>
		/// <returns>A <see cref="DateTime" /> representing the last day of the quarter, normalized to 00:00:00 and <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is less than 1 or greater than 4.</exception>
		public DateTime GetEndDate(int quarter)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(quarter, 1, 4);

			long startTicks = GetStartDate(quarter).Ticks;
			long endTicks = startTicks + (13L * DateTimeExtensions.TicksPerWeek) - DateTimeExtensions.TicksPerDay;

			endTicks = DateTimeExtensions.GetDateAsTicks(endTicks);

			return new DateTime(endTicks, DateTimeKind.Unspecified);
		}

		/// <inheritdoc />
		/// <remarks>
		/// This method is used by <see cref="DateTimeExtensions.Quarter(DateTime, IQuarterDefinitionProvider)" /> to determine the 1-based
		/// fiscal quarter for the specified date.
		/// <para>
		/// Each quarter spans 13 weeks, beginning from the aligned fiscal year anchor. The method calculates the number of full weeks
		/// between the input date and the fiscal year start and returns the corresponding quarter number (1–4).
		/// </para>
		/// <para>Throws <see cref="ArgumentOutOfRangeException" /> if the date falls outside the supported fiscal year.</para>
		/// </remarks>
		public int GetQuarter(DateTime dateTime)
		{
			var alignedDate = ValidateDateInFiscalYear(dateTime);
			int totalWeeks = (int)((alignedDate.Ticks - fiscalYearStart.Ticks) / DateTimeExtensions.TicksPerWeek);
			return Math.Min((totalWeeks / 13) + 1, 4);
		}

		/// <inheritdoc />
		/// <remarks>
		/// This method is used by <see cref="DateTimeExtensions.FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> to compute the
		/// starting calendar date of the quarter containing the specified date.
		/// <para>
		/// The start date is calculated by aligning the input date to the beginning of its fiscal week and determining the corresponding
		/// fiscal quarter (1–4), then offsetting from the fiscal anchor date by a multiple of 13 weeks (91 days).
		/// </para>
		/// </remarks>
		/// <param name="dateTime">A <see cref="DateTime" /> within the fiscal year to resolve to a quarter start date.</param>
		/// <returns>A <see cref="DateTime" /> representing the start of the quarter, preserving the <paramref name="dateTime" /> kind.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="dateTime" /> is outside the configured fiscal year.</exception>
		public DateTime GetStartDate(DateTime dateTime)
		{
			var alignedDate = ValidateDateInFiscalYear(dateTime);
			long deltaWeeks = (alignedDate.Ticks - fiscalYearStart.Ticks) / DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStart.Ticks + ((deltaWeeks / 13) * 13L * DateTimeExtensions.TicksPerWeek);

			startTicks = DateTimeExtensions.GetDateAsTicks(startTicks);
			return new DateTime(startTicks, dateTime.Kind);
		}

		/// <inheritdoc />
		/// <remarks>
		/// Calculates the start date of the specified quarter by offsetting the aligned fiscal year start date by a multiple of 13 weeks.
		/// </remarks>
		/// <param name="quarter">The fiscal quarter number (1–4).</param>
		/// <returns>A <see cref="DateTime" /> representing the first day of the quarter, normalized to 00:00:00 and <see cref="DateTimeKind.Unspecified" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is less than 1 or greater than 4.</exception>
		public DateTime GetStartDate(int quarter)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(quarter, 1, 4);

			long offsetTicks = (quarter - 1) * 13L * DateTimeExtensions.TicksPerWeek;
			long startTicks = fiscalYearStart.Ticks + offsetTicks;

			startTicks = DateTimeExtensions.GetDateAsTicks(startTicks);

			return new DateTime(startTicks, DateTimeKind.Unspecified);
		}

		/// <summary>
		/// Gets the total number of weeks in the fiscal year (52 or 53).
		/// </summary>
		/// <returns>The number of weeks in the fiscal year.</returns>
		public int GetWeeksInFiscalYear() => Is53WeekFiscalYear() ? 53 : 52;

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
		/// Aligns a date to the start of its week based on the configured start day.
		/// </summary>
		/// <param name="date">The date to align.</param>
		/// <param name="weekStart">The starting day of the week.</param>
		/// <returns>A <see cref="DateTime" /> aligned to the beginning of the week.</returns>
		private static DateTime AlignToStartOfWeek(DateTime date, DayOfWeek weekStart)
		{
			long ticks = date.Date.Ticks - DateTimeExtensions.GetPreviousDayOfWeekTicksFrom(date.Date.Ticks, weekStart);
			return new DateTime(ticks, date.Kind);
		}

		/// <summary>
		/// Validates that the specified date falls within the configured fiscal year range.
		/// </summary>
		/// <param name="dateTime">The date to validate.</param>
		/// <returns>A <see cref="DateTime" /> aligned to the beginning of the fiscal week that contains <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dateTime" /> is earlier than the fiscal year start, or more than 52 or 53 weeks (depending on
		/// configuration) from it.
		/// </exception>
		/// <remarks>
		/// This method is used internally by all methods that compute fiscal quarters based on dates. It ensures the input is within the
		/// valid range of the configured fiscal year and normalizes the date to its week's start boundary.
		/// </remarks>
		private DateTime ValidateDateInFiscalYear(DateTime dateTime)
		{
			var alignedDate = AlignToStartOfWeek(dateTime.Date, firstDayOfWeek);
			long deltaWeeks = (alignedDate.Ticks - fiscalYearStart.Ticks) / DateTimeExtensions.TicksPerWeek;

			int maxWeeks = Is53WeekFiscalYear() ? 53 : 52;

			if (deltaWeeks < 0 || deltaWeeks >= maxWeeks)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_DateOutsideFiscalYear, dateTime, fiscalYearStart));

			return alignedDate;
		}
	}
}