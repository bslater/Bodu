namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Specifies the type of action to apply when adjusting a notable date based on a matching adjustment rule.
	/// </summary>
	/// <remarks>
	/// The <see cref="NotableDateAdjustmentActionType" /> enumeration defines how a notable date should be modified if an associated
	/// <see cref="NotableDateAdjustmentRule" /> condition is met. Actions may involve moving the date, replacing it with a different named
	/// date, or applying a custom adjustment.
	/// </remarks>
	public enum NotableDateAdjustmentActionType
	{
		/// <summary>
		/// No adjustment is made; the date remains unchanged even if the adjustment condition is satisfied.
		/// </summary>
		None,

		/// <summary>
		/// Moves the date forward to the next weekday (typically Monday to Friday).
		/// </summary>
		MoveToNextWeekday,

		/// <summary>
		/// Moves the date forward to the next non-working day based on the configured weekend or non-working day definitions.
		/// </summary>
		MoveToNextNonWorkingDay,

		/// <summary>
		/// Moves the date backward to the previous weekday (typically Monday to Friday).
		/// </summary>
		MoveToPreviousWeekday,

		/// <summary>
		/// Adds a specified number of calendar days to the original date.
		/// </summary>
		AddDays,

		/// <summary>
		/// Replaces the date with a different named notable date, based on a reference definition.
		/// </summary>
		ReplaceWithNamedDate,

		/// <summary>
		/// Applies a custom adjustment action, typically implemented through external logic.
		/// </summary>
		Custom
	}
}