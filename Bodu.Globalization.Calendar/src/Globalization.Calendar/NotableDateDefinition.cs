using Bodu.Extensions;
using System.Collections.Immutable;
using SysGlobal = System.Globalization;

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Represents the definition of a notable calendar date, such as a public holiday, cultural observance, or seasonal event. This
	/// metadata defines how the date is calculated, categorized, and adjusted within a calendar system.
	/// </summary>
	/// <remarks>
	/// A <see cref="NotableDateDefinition" /> specifies the rules and properties used to determine when and how a notable date should
	/// occur, supporting fixed, dynamic, rule-based, and offset-from-base-date configurations.
	/// </remarks>
	public sealed record class NotableDateDefinition
	{
		// Basic required fields
#if NET7_0_OR_GREATER

		/// <summary>
		/// Gets the unique name or identifier of the notable date.
		/// </summary>
		public required string Name { get; init; }

		/// <summary>
		/// Gets the definition type that determines how this date is calculated (e.g., Fixed, Rule-based, Dynamic).
		/// </summary>
		public required NotableDateDefinitionType DefinitionType { get; init; }

		/// <summary>
		/// Gets the category or classification of the notable date (e.g., Holiday, Observance, Religious).
		/// </summary>
		public required NotableDateKind NotableDateKind { get; init; }
#else

		/// <summary>
		/// Gets the unique name or identifier of the notable date.
		/// </summary>
		public string Name { get; init; } = string.Empty;

		/// <summary>
		/// Gets the definition type that determines how this date is calculated (e.g., Fixed, Rule-based, Dynamic).
		/// </summary>
		public NotableDateDefinitionType DefinitionType { get; init; }

		/// <summary>
		/// Gets the category or classification of the notable date (e.g., Holiday, Observance, Religious).
		/// </summary>
		public NotableDateKind NotableDateKind { get; init; }
#endif

		// Metadata constraints

		/// <summary>
		/// Gets the first year the notable date is applicable. If <c>null</c>, the date applies to all years.
		/// </summary>
		public int? FirstYear { get; init; }

		/// <summary>
		/// Gets the last year the notable date is applicable. If <c>null</c>, the date applies indefinitely.
		/// </summary>
		public int? LastYear { get; init; }

		/// <summary>
		/// Gets the calendar system type associated with this notable date (e.g., <see cref="SysGlobal.GregorianCalendar" />).
		/// </summary>
		public Type? CalendarType { get; init; }

		/// <summary>
		/// Gets the ISO territory or country code (e.g., "US", "AU") where the notable date is applicable.
		/// </summary>
		public string? TerritoryCode { get; init; }

		/// <summary>
		/// Gets a value indicating whether the notable date is classified as a non-working day.
		/// </summary>
		public bool? NonWorking { get; init; }

		/// <summary>
		/// Gets the recurrence interval in years, if the date repeats periodically (e.g., every 4 years).
		/// </summary>
		public int? OccurrenceYears { get; init; }

		// Fixed date fields

		/// <summary>
		/// Gets the day of the month when the notable date occurs. Used primarily for fixed or rule-based definitions.
		/// </summary>
		public int? Day { get; init; }

		/// <summary>
		/// Gets the month of the year when the notable date occurs. Used primarily for fixed or rule-based definitions.
		/// </summary>
		public int? Month { get; init; }

		// Rule-based date fields

		/// <summary>
		/// Gets the day of the week that the notable date falls on, used for rule-based definitions (e.g., first Monday of October).
		/// </summary>
		public DayOfWeek? DayOfWeek { get; init; }

		/// <summary>
		/// Gets the ordinal position of the weekday within the month (e.g., second Monday). Used for rule-based definitions.
		/// </summary>
		public WeekOfMonthOrdinal? WeekOrdinal { get; init; }

		// Offset-from-based fields

		/// <summary>
		/// Gets the name of another notable date from which this date is calculated by an offset. Used for offset-from notable date types.
		/// </summary>
		public string? BaseNotableDateName { get; init; }

		/// <summary>
		/// Gets the number of days to offset from the base notable date. Positive values move forward; negative values move backward.
		/// </summary>
		public int? OffsetDays { get; init; }

		// Dynamic provider-based fields

		/// <summary>
		/// Gets the type of the dynamic date calculator used to compute the date algorithmically. Required for dynamic definition types.
		/// </summary>
		public Type? NotableDateCalculatorType { get; init; }

		// Adjustment rules

		/// <summary>
		/// Gets a collection of adjustment rules that modify the notable date under specific conditions.
		/// </summary>
		public ImmutableArray<NotableDateAdjustmentRule> AdjustmentRules { get; init; } = ImmutableArray<NotableDateAdjustmentRule>.Empty;

		// Miscellaneous

		/// <summary>
		/// Gets an optional description, comment, or documentation note associated with the notable date.
		/// </summary>
		public string? Comment { get; init; }
	}
}