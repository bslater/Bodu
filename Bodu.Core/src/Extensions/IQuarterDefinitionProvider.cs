using System;

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides custom quarter calculation logic for a <see cref="DateTime" />.
	/// </summary>
	/// <remarks>
	/// Implement this interface to support <see cref="QuarterDefinition.Custom" /> in
	/// <see cref="DateTimeExtensions.Quarter(DateTime, QuarterDefinition)" /> and related methods like
	/// <see cref="DateTimeExtensions.FirstDayOfQuarter(DateTime, IQuarterProvider)" />. This allows support for non-standard or
	/// domain-specific quarterly definitions such as 4-4-5 calendars or region-specific fiscal models.
	/// </remarks>
	public interface IQuarterProvider
	{
		/// <summary>
		/// Gets the quarter number (1–4) for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> for which the quarter is to be determined.</param>
		/// <returns>An integer in the range 1 to 4 representing the quarter that contains the given dateTime.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the input dateTime cannot be mapped to a valid quarter.</exception>
		/// <remarks>
		/// The returned value must be in the range 1 to 4. Implementations should throw an exception for unsupported values or ambiguous ranges.
		/// </remarks>
		int GetQuarter(DateTime dateTime);

		/// <summary>
		/// Gets the starting month number (1–12) corresponding to the specified <paramref name="quarter" />.
		/// </summary>
		/// <param name="quarter">The quarter number for which to retrieve the first month (must be 1–4).</param>
		/// <returns>
		/// An integer representing the first month of the specified quarter. For example, a return value of 4 corresponds to April.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="quarter" /> is not between 1 and 4.</exception>
		/// <remarks>This method is used to compute the start of a quarter and must always return a valid month in the range 1–12.</remarks>
		int GetStartMonthFromQuarter(int quarter);
	}
}
