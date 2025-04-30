namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Defines the condition that triggers an adjustment action for a notable date.
	/// </summary>
	/// <remarks>
	/// The <see cref="NotableDateAdjustmentRuleType" /> enumeration specifies the rules under which a notable date should be adjusted.
	/// Adjustment rules are evaluated during notable date generation and can conditionally alter the date based on calendar properties,
	/// fixed dates, or user-defined logic.
	/// </remarks>
	public enum NotableDateAdjustmentRuleType
	{
		/// <summary>
		/// Always apply the associated adjustment action, regardless of any specific condition.
		/// </summary>
		Always,

		/// <summary>
		/// Apply the adjustment if the date falls on a specified day of the week (e.g., Monday, Friday).
		/// </summary>
		IfDayOfWeek,

		/// <summary>
		/// Apply the adjustment if the date falls on a weekend day, typically Saturday or Sunday based on the configured weekend definition.
		/// </summary>
		IfWeekend,

		/// <summary>
		/// Apply the adjustment if the date falls on a weekday, typically Monday through Friday.
		/// </summary>
		IfWeekday,

		/// <summary>
		/// Apply the adjustment if the date is classified as a non-working day, such as a weekend or public holiday.
		/// </summary>
		IfNonWorkingDay,

		/// <summary>
		/// Apply the adjustment if the date occurs before a specified fixed comparison date.
		/// </summary>
		IfBeforeFixedDate,

		/// <summary>
		/// Apply the adjustment if the date occurs after a specified fixed comparison date.
		/// </summary>
		IfAfterFixedDate,

		/// <summary>
		/// Apply the adjustment if the date falls within a leap year.
		/// </summary>
		IfLeapYear,

		/// <summary>
		/// Apply the adjustment if the date represents a specific ordinal occurrence of a weekday within a month (e.g., second Monday).
		/// </summary>
		IfNthOccurrenceInMonth,

		/// <summary>
		/// Apply a custom adjustment condition determined by a user-defined handler or external logic.
		/// </summary>
		Custom
	}
}