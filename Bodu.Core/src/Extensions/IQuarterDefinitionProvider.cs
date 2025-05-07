// ---------------------------------------------------------------------------------------------------------------
// <copyright file="IQuarterDefinitionProvider.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	/// <summary>
	/// Defines a provider interface for custom quarter calculation logic based on <see cref="DateTime" /> values.
	/// </summary>
	/// <remarks>
	/// Implement this interface to support the <see cref="CalendarQuarterDefinition.Custom" /> option in
	/// <see cref="DateTimeExtensions.Quarter(DateTime, CalendarQuarterDefinition)" /> and related methods such as
	/// <see cref="DateTimeExtensions.FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> and
	/// <see cref="DateTimeExtensions.LastDayOfQuarter(DateTime, IQuarterDefinitionProvider)" />. This enables support for non-standard or
	/// domain-specific quarter systems, including 4–4–5 calendars, retail fiscal calendars, academic terms, or historical financial
	/// quarters that do not align with built-in <see cref="CalendarQuarterDefinition" /> values such as
	/// <see cref="CalendarQuarterDefinition.JanuaryDecember" /> or <see cref="CalendarQuarterDefinition.JulyToJune" />.
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
		/// <remarks>
		/// Used when <see cref="CalendarQuarterDefinition.Custom" /> is selected to support flexible fiscal structures. Implementations
		/// should throw for unsupported or ambiguous inputs.
		/// </remarks>
		int GetQuarter(DateTime dateTime);

		/// <summary>
		/// Returns the first day of the quarter that contains the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the start of the quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the first day of the quarter that includes <paramref name="dateTime" />.
		/// The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// This method applies a custom rule set as defined by the implementation of <see cref="IQuarterDefinitionProvider" />. It is used
		/// when <see cref="CalendarQuarterDefinition.Custom" /> is selected. The result is normalized to midnight.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateTime" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateTime GetStartDate(DateTime dateTime);

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
		/// Returns the last day of the quarter that contains the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which to determine the end of the quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to midnight (00:00:00) on the final day of the quarter that includes <paramref name="dateTime" />.
		/// The result preserves the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <remarks>
		/// This method applies the custom quarter logic associated with <see cref="CalendarQuarterDefinition.Custom" />. The result is
		/// normalized to midnight and excludes any time component.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when the <paramref name="dateTime" /> does not fall within a valid or recognized quarter.
		/// </exception>
		DateTime GetEndDate(DateTime dateTime);

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
	}
}