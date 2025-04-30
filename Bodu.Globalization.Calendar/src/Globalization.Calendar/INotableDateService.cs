using SysGlobal = System.Globalization;

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Defines a service for managing, computing, and querying notable dates based on preloaded notable date definitions.
	/// </summary>
	/// <remarks>
	/// An <see cref="INotableDateService" /> implementation is responsible for providing notable date evaluations, including determining
	/// weekends, non-working days, and retrieving notable events for specific years, date ranges, or individual days. The behavior may vary
	/// based on territory-specific holidays, cultural calendars, or custom calendar systems.
	/// </remarks>
	public interface INotableDateService
	{
		/// <summary>
		/// Determines whether the specified date falls on a weekend based on the configured weekend definition.
		/// </summary>
		/// <param name="date">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns><c>true</c> if the specified date is considered a weekend day; otherwise, <c>false</c>.</returns>
		/// <remarks>Weekend days are typically Saturday and Sunday but may vary depending on regional or cultural definitions.</remarks>
		bool IsWeekend(DateTime date);

		/// <summary>
		/// Determines whether the specified date is considered a non-working day, such as a weekend or a notable holiday.
		/// </summary>
		/// <param name="date">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="territoryCode">
		/// Optional. The territory or country code used to apply localized non-working rules. If <c>null</c>, default rules apply.
		/// </param>
		/// <param name="calendarType">
		/// Optional. The calendar system type to use for evaluating dates, such as <see cref="SysGlobal.GregorianCalendar" /> or a custom calendar.
		/// </param>
		/// <returns><c>true</c> if the specified date is a non-working day; otherwise, <c>false</c>.</returns>
		/// <remarks>
		/// Non-working days may include weekends, public holidays, observances, or other defined notable days depending on the configuration.
		/// </remarks>
		bool IsNonWorkingDay(DateTime date, string? territoryCode = null, Type? calendarType = null);

		/// <summary>
		/// Retrieves all notable dates for the specified calendar year.
		/// </summary>
		/// <param name="year">The calendar year to retrieve notable dates for.</param>
		/// <param name="territoryCode">
		/// Optional. The territory or country code used to filter notable dates. If <c>null</c>, default dates are returned.
		/// </param>
		/// <param name="calendarType">Optional. The calendar system type to use when evaluating the year, such as <see cref="SysGlobal.GregorianCalendar" />.</param>
		/// <returns>A read-only list of <see cref="NotableDate" /> instances that occur within the specified year.</returns>
		/// <remarks>
		/// This method includes all notable dates matching the provided territory and calendar context, including fixed and dynamically
		/// calculated events.
		/// </remarks>
		IReadOnlyList<NotableDate> GetNotableDates(int year, string? territoryCode = null, Type? calendarType = null);

		/// <summary>
		/// Retrieves all notable dates that fall within a specified inclusive date range.
		/// </summary>
		/// <param name="startDate">The inclusive start date of the range.</param>
		/// <param name="endDate">The inclusive end date of the range.</param>
		/// <param name="territoryCode">
		/// Optional. The territory or country code used to filter notable dates. If <c>null</c>, default dates are returned.
		/// </param>
		/// <param name="calendarType">Optional. The calendar system type to use when evaluating dates, such as <see cref="SysGlobal.GregorianCalendar" />.</param>
		/// <returns>A read-only list of <see cref="NotableDate" /> instances occurring within the specified range.</returns>
		/// <remarks>
		/// This method is useful for retrieving notable dates relevant to a project phase, report period, or other time-based window.
		/// </remarks>
		IReadOnlyList<NotableDate> GetNotableDates(DateTime startDate, DateTime endDate, string? territoryCode = null, Type? calendarType = null);

		/// <summary>
		/// Retrieves all notable dates that occur on a specific day.
		/// </summary>
		/// <param name="date">The specific <see cref="DateTime" /> value to check for notable events.</param>
		/// <param name="territoryCode">
		/// Optional. The territory or country code used to filter notable dates. If <c>null</c>, default dates are evaluated.
		/// </param>
		/// <param name="calendarType">Optional. The calendar system type to use when evaluating the date, such as <see cref="SysGlobal.GregorianCalendar" />.</param>
		/// <returns>A read-only list of <see cref="NotableDate" /> instances that match the given date.</returns>
		/// <remarks>Multiple notable dates may occur on the same day, such as overlapping holidays or observances.</remarks>
		IReadOnlyList<NotableDate> GetNotableDates(DateTime date, string? territoryCode = null, Type? calendarType = null);
	}
}