// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IQuarterDefinitionProvider.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides custom quarter calculation logic for a <see cref="DateTime" />.
	/// </summary>
	/// <remarks>
	/// Implement this interface to support <see cref="CalendarQuarterDefinition.Custom" /> in
	/// <see cref="DateTimeExtensions.Quarter(DateTime, CalendarQuarterDefinition)" /> and related methods such as
	/// <see cref="DateTimeExtensions.FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> and
	/// <see cref="DateTimeExtensions.LastDayOfQuarter(DateTime, IQuarterDefinitionProvider)" />. This allows support for non-standard or
	/// domain-specific quarterly definitions such as 4-4-5 calendars, retail fiscal models, or academic quarters.
	/// </remarks>
	public interface IQuarterDefinitionProvider
	{
		/// <summary>
		/// Gets the quarter number (1–4) for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which the quarter is to be determined.</param>
		/// <returns>An integer in the range 1 to 4 representing the quarter that contains the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the input <paramref name="dateTime" /> cannot be mapped to a valid quarter.
		/// </exception>
		/// <remarks>
		/// The returned value must be in the range 1 to 4. Implementations should throw an exception for unsupported values or ambiguous ranges.
		/// </remarks>
		int GetQuarter(DateTime dateTime);

		/// <summary>
		/// Gets the start date of the quarter that includes the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> used to identify the containing quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the first day of the quarter. The returned value must have a time component of
		/// 00:00:00 and preserve the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the <paramref name="dateTime" /> does not fall within a known or supported quarter.
		/// </exception>
		DateTime GetStartDate(DateTime dateTime);

		/// <summary>
		/// Gets the end date of the quarter that includes the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> used to identify the containing quarter.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the final day of the quarter. The returned value must have a time component of
		/// 00:00:00 and preserve the <see cref="DateTime.Kind" /> of the input.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the <paramref name="dateTime" /> does not fall within a known or supported quarter.
		/// </exception>
		DateTime GetEndDate(DateTime dateTime);
	}
}