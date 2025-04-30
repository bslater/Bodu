namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Specifies the method used to define the date of a notable event.
	/// </summary>
	/// <remarks>
	/// The <see cref="NotableDateDefinitionType" /> enumeration identifies how a notable date is calculated. Definitions may be fixed to a
	/// specific day, dynamically calculated based on algorithmic rules, rule-based conditions, or offsets from other notable dates.
	/// </remarks>
	public enum NotableDateDefinitionType
	{
		/// <summary>
		/// Defines a date that occurs on the same fixed day and month each year.
		/// </summary>
		Fixed,

		/// <summary>
		/// Defines a date that is dynamically calculated based on an algorithm, such as Easter Sunday.
		/// </summary>
		Dynamic,

		/// <summary>
		/// Defines a date that is determined by applying rule-based conditions, such as the first Monday of a month.
		/// </summary>
		Rule,

		/// <summary>
		/// Defines a date that is calculated by applying a relative offset from another notable date.
		/// </summary>
		OffsetFrom
	}
}