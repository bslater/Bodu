namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Represents a conditional rule that adjusts a notable date based on contextual factors such as day of week, region, or calendar system.
	/// </summary>
	public sealed record NotableDateAdjustmentRule
	{
		/// <summary>
		/// The type of condition that triggers this adjustment (e.g. if the date falls on a weekend).
		/// </summary>
		public NotableDateAdjustmentRuleType AdjustmentRule { get; init; }

		/// <summary>
		/// The adjustment action to apply if the rule is triggered.
		/// </summary>
		public NotableDateAdjustmentActionType Action { get; init; }

		/// <summary>
		/// The number of days to offset the date, used when the action is a relative adjustment.
		/// </summary>
		public DayOfWeek? DayOfWeek { get; init; }

		/// <summary>
		/// Indicates whether the adjusted notable date is considered a non-working day.
		/// </summary>
		public bool? NonWorking { get; init; }

		/// <summary>
		/// The number of days to offset the date, used when the action is a relative adjustment.
		/// </summary>
		public int OffsetDays { get; init; } = 0;

		/// <summary>
		/// Optional ISO country code (e.g., "AU", "US") that scopes the adjustment to a specific country.
		/// </summary>
		public string? TerritoryCode { get; init; }

		/// <summary>
		/// The calendar system or provider name this rule is associated with (e.g., "Gregorian").
		/// </summary>
		public Type? CalendarType { get; init; }

		/// <summary>
		/// Optional minimum year for which the rule is effective.
		/// </summary>
		public int? EffectiveFromYear { get; init; }

		/// <summary>
		/// Optional maximum year for which the rule remains effective.
		/// </summary>
		public int? EffectiveToYear { get; init; }

		/// <summary>
		/// Optional friendly name of another notable date this rule may reference (used by ReplaceWithNamedDate).
		/// </summary>
		public string? TargetNotableDateName { get; init; }

		/// <summary>
		/// Determines the order of rule evaluation; lower values have higher precedence.
		/// </summary>
		public int Priority { get; init; } = 100;

		/// <summary>
		/// Optional name of the custom adjustment provider to use.
		/// </summary>
		public string? CustomHandler { get; init; }

		/// <summary>
		/// Optional parameters for the custom logic.
		/// </summary>
		public Dictionary<string, string>? CustomHandlerParameters { get; init; }
	}
}