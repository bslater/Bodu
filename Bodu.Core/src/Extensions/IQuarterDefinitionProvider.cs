using System;

namespace Bodu.Extensions
{
	/// <summary>
	/// Defines a provider interface for custom quarter calculation logic based on <see cref="DateTime" /> or <see cref="DateOnly" /> values.
	/// </summary>
	/// <remarks>
	/// Implement this interface to support the <see cref="CalendarQuarterDefinition.Custom" /> option in
	/// <see cref="DateTimeExtensions.Quarter(DateTime, CalendarQuarterDefinition)" /> and
	/// <see cref="DateOnlyExtensions.Quarter(DateOnly, CalendarQuarterDefinition)" />, along with related methods such as
	/// <see cref="DateTimeExtensions.FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> and
	/// <see cref="DateOnlyExtensions.FirstDayOfQuarter(DateOnly, IQuarterDefinitionProvider)" />. This enables support for non-standard or
	/// domain-specific quarter systems, including 4–4–5 calendars, retail fiscal calendars, academic terms, or historical financial
	/// quarters that do not align with built-in <see cref="CalendarQuarterDefinition" /> values.
	/// </remarks>
	public interface IQuarterDefinitionProvider
	{
		/// <summary>
		/// Returns the quarter number (1–4) that contains the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the quarter.</param>
		/// <returns>An integer in the range 1 to 4 representing the quarter that includes <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateTime" /> does not fall within a valid or recognized quarter definition.
		/// </exception>
		int GetQuarter(DateTime dateTime);

		/// <summary>
		/// Returns the quarter number (1–4) that contains the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateOnly">The input <see cref="DateOnly" /> for which to determine the quarter.</param>
		/// <returns>An integer in the range 1 to 4 representing the quarter that includes <paramref name="dateOnly" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateOnly" /> does not fall within a valid or recognized quarter definition.
		/// </exception>
		int GetQuarter(DateOnly dateOnly);

		/// <summary>
		/// Returns the first day of the quarter that contains the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the start of the quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the quarter that includes <paramref name="dateTime" />.
		/// The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateTime" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateTime GetStartDate(DateTime dateTime);

		/// <summary>
		/// Returns the first day of the quarter that contains the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateOnly">The input <see cref="DateOnly" /> for which to determine the start of the quarter.</param>
		/// <returns>A <see cref="DateOnly" /> representing the first day of the quarter that includes <paramref name="dateOnly" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateOnly" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateOnly GetStartDate(DateOnly dateOnly);

		/// <summary>
		/// Returns the first day of the specified quarter number (1–4) within the configured fiscal year.
		/// </summary>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the specified quarter. The result preserves the
		/// intended <see cref="DateTime.Kind" /> and contextual interpretation.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="quarter" /> value is not between 1 and 4.</exception>
		DateTime GetStartDate(int quarter);

		/// <summary>
		/// Returns the first day of the specified quarter number (1–4) within the configured fiscal year.
		/// </summary>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <returns>A <see cref="DateOnly" /> representing the first day of the specified quarter.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="quarter" /> value is not between 1 and 4.</exception>
		DateOnly GetStartDateAsDateOnly(int quarter);

		/// <summary>
		/// Returns the last day of the quarter that contains the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the end of the quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the final day of the quarter that includes <paramref name="dateTime" />.
		/// The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateTime" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateTime GetEndDate(DateTime dateTime);

		/// <summary>
		/// Returns the last day of the quarter that contains the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateOnly">The input <see cref="DateOnly" /> for which to determine the end of the quarter.</param>
		/// <returns>A <see cref="DateOnly" /> representing the final day of the quarter that includes <paramref name="dateOnly" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateOnly" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateOnly GetEndDate(DateOnly dateOnly);

		/// <summary>
		/// Returns the last day of the specified quarter number (1–4) within the configured fiscal year.
		/// </summary>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the last day of the specified quarter. The result preserves the intended
		/// <see cref="DateTime.Kind" /> and contextual interpretation.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="quarter" /> value is not between 1 and 4.</exception>
		DateTime GetEndDate(int quarter);

		/// <summary>
		/// Returns the last day of the specified quarter number (1–4) within the configured fiscal year.
		/// </summary>
		/// <param name="quarter">The quarter number (1–4).</param>
		/// <returns>A <see cref="DateOnly" /> representing the final day of the specified quarter.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when the <paramref name="quarter" /> value is not between 1 and 4.</exception>
		DateOnly GetEndDateAsDateOnly(int quarter);
	}
}