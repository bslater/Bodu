using Bodu.Extensions;

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Applies adjustment rules to notable dates.
	/// </summary>
	internal sealed class NotableDateAdjuster
	{
		private readonly Func<DateTime, bool> _isWeekend;
		private readonly Func<DateTime, string?, Type?, bool> _isNonWorkingDay;
		private readonly CalendarWeekendDefinition _weekendDefinition;
		private readonly IWeekendDefinitionProvider? _weekendProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="NotableDateAdjuster" /> class.
		/// </summary>
		/// <param name="isWeekend">A function to determine if a date is a weekend.</param>
		/// <param name="isNonWorkingDay">A function to determine if a date is a non-working day with culture-awareness.</param>
		/// <param name="weekendDefinition">The weekend definition used for adjustments.</param>
		/// <param name="weekendProvider">An optional custom weekend provider.</param>
		public NotableDateAdjuster(
			Func<DateTime, bool> isWeekend,
			Func<DateTime, string?, Type?, bool> isNonWorkingDay,
			CalendarWeekendDefinition weekendDefinition,
			IWeekendDefinitionProvider? weekendProvider)
		{
			_isWeekend = isWeekend ?? throw new ArgumentNullException(nameof(isWeekend));
			_isNonWorkingDay = isNonWorkingDay ?? throw new ArgumentNullException(nameof(isNonWorkingDay));
			_weekendDefinition = weekendDefinition;
			_weekendProvider = weekendProvider;
		}

		/// <summary>
		/// Applies an adjustment rule to a specified date based on defined conditions and actions.
		/// </summary>
		/// <param name="rule">The adjustment rule that defines the condition and action to apply.</param>
		/// <param name="original">The original <see cref="DateTime" /> value to evaluate and potentially adjust.</param>
		/// <param name="territoryCode">
		/// Optional. A country or territory code used to determine non-working days when evaluating rules such as <see cref="NotableDateAdjustmentRuleType.IfNonWorkingDay" />.
		/// </param>
		/// <param name="calendarType">
		/// Optional. A calendar system type used when evaluating non-working days, typically derived from a custom calendar system.
		/// </param>
		/// <returns>
		/// A tuple consisting of:
		/// <list type="bullet">
		/// <item>
		/// <description><c>Success</c>: <c>true</c> if the adjustment condition was met and the action was applied; otherwise, <c>false</c>.</description>
		/// </item>
		/// <item>
		/// <description>
		/// <c>AdjustedDate</c>: The resulting <see cref="DateTime" /> after applying the action if the condition was met, or the original
		/// date if not.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// <para>
		/// The method first evaluates the condition specified in <paramref name="rule" />. If the condition is satisfied, it applies the
		/// associated action to adjust the original date.
		/// </para>
		/// <para>
		/// Supported conditions include fixed rules such as <see cref="NotableDateAdjustmentRuleType.IfWeekend" />, custom conditions like
		/// <see cref="NotableDateAdjustmentRuleType.IfDayOfWeek" />, or external calendar-based conditions such as <see cref="NotableDateAdjustmentRuleType.IfNonWorkingDay" />.
		/// </para>
		/// <para>
		/// If no action is applicable or the condition is not met, the method returns the original date without modification and sets
		/// <c>Success</c> to <c>false</c>.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="rule" /> is <c>null</c>.</exception>
		public (bool Success, DateTime AdjustedDate) Apply(NotableDateAdjustmentRule rule, DateTime original, string? territoryCode = null, Type? calendarType = null)
		{
			ThrowHelper.ThrowIfNull(rule);

			DateTime adjusted = original;

			bool conditionMet = rule.AdjustmentRule switch
			{
				NotableDateAdjustmentRuleType.Always => true,
				NotableDateAdjustmentRuleType.IfWeekend => _isWeekend(original),
				NotableDateAdjustmentRuleType.IfWeekday => !_isWeekend(original),
				NotableDateAdjustmentRuleType.IfNonWorkingDay => _isNonWorkingDay(original, territoryCode, calendarType),
				NotableDateAdjustmentRuleType.IfLeapYear => DateTime.IsLeapYear(original.Year),
				NotableDateAdjustmentRuleType.IfDayOfWeek when rule.DayOfWeek.HasValue => original.DayOfWeek == rule.DayOfWeek.Value,
				_ => false,
			};

			if (!conditionMet)
				return (false, original);

			adjusted = rule.Action switch
			{
				NotableDateAdjustmentActionType.None => adjusted,
				NotableDateAdjustmentActionType.AddDays => adjusted.AddDays(rule.OffsetDays),
				NotableDateAdjustmentActionType.MoveToNextWeekday => adjusted.NextWeekday(_weekendDefinition, _weekendProvider),
				NotableDateAdjustmentActionType.MoveToPreviousWeekday => adjusted.PreviousWeekday(_weekendDefinition, _weekendProvider),
				_ => adjusted
			};

			return (true, adjusted);
		}
	}
}