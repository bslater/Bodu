namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Represents a notable date including its calculated occurrence, cultural applicability, and adjustment status.
	/// </summary>
	public sealed record class NotableDate
	{
#if NET7_0_OR_GREATER

		/// <summary>
		/// The final calculated date for the notable event.
		/// </summary>
		public required DateTime Date { get; init; }

		/// <summary>
		/// The name or label identifying the notable date (e.g., "Christmas Day", "ANZAC Day").
		/// </summary>
		public required string Name { get; init; }

		/// <summary>
		/// The kind of notable date (e.g., Holiday, Observance, Public Event).
		/// </summary>
		public required NotableDateKind Kind { get; init; }
#else

		/// <summary>
		/// The final calculated date for the notable event.
		/// </summary>
		public DateTime Date { get; init; }

		/// <summary>
		/// The name or label identifying the notable date (e.g., "Christmas Day", "ANZAC Day").
		/// </summary>
		public string Name { get; init; }

		/// <summary>
		/// The kind of notable date (e.g., Holiday, Observance, Public Event).
		/// </summary>
		public NotableDateKind Kind { get; init; }
#endif

		/// <summary>
		/// The original date before any adjustments were applied, if applicable.
		/// </summary>
		/// <remarks>
		/// This field is populated only if adjustment rules modified the base date. If no adjustments were made, this value is <c>null</c>.
		/// </remarks>
		public DateTime? OriginalDate { get; init; }

		/// <summary>
		/// Indicates whether the notable date is considered a non-working day.
		/// </summary>
		/// <remarks>
		/// Defaults to <c>false</c> if unspecified. A value of <c>true</c> indicates that the date is officially recognized as a
		/// non-working day (e.g., public holiday, mandated observance, or organizational closure).
		/// </remarks>
		public bool NonWorking { get; init; } = false;

		/// <summary>
		/// Indicates whether the final <see cref="Date" /> was adjusted from its original value.
		/// </summary>
		/// <remarks>Returns <c>true</c> if an <see cref="OriginalDate" /> exists and differs from the final <see cref="Date" />.</remarks>
		public bool WasAdjusted => OriginalDate.HasValue && OriginalDate.Value != Date;

		/// <summary>
		/// The calendar system associated with this notable date (e.g., Gregorian, Hebrew, Islamic).
		/// </summary>
		/// <remarks>If <c>null</c>, the date is assumed to use the default Gregorian calendar.</remarks>
		public Type? CalendarType { get; init; }

		/// <summary>
		/// Optional ISO 3166-1 alpha-2 country code (e.g., "US", "AU") where this notable date applies.
		/// </summary>
		/// <remarks>If <c>null</c>, the date is assumed to be globally applicable or not country-specific.</remarks>
		public string? TerritoryCode { get; init; }

		/// <summary>
		/// Optional comment or additional notes associated with the notable date.
		/// </summary>
		public string? Comment { get; init; }

		/// <summary>
		/// Gets the localized or context-specific display name for the notable date, including territory and/or calendar qualifiers if applicable.
		/// </summary>
		/// <remarks>
		/// If a territory code and/or calendar type is present, they are appended to the base name for clarity (e.g., "Labour Day
		/// (AU-NSW)", "Easter (AU, Hebrew)").
		/// </remarks>
		public string LocalizedName =>
			TerritoryCode is null && CalendarType is null
				? Name
				: $"{Name} ({string.Join(", ",
					new[] { TerritoryCode, CalendarType?.Name.Replace("Calendar", "", StringComparison.OrdinalIgnoreCase) }
						.Where(s => !string.IsNullOrEmpty(s))
				)})";
	}
}