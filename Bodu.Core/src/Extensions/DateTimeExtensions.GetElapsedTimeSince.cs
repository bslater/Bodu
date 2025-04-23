// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.GetElapsedTimeSince.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the amount of time that has elapsed between the current system time and the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to compare against the current time.</param>
		/// <returns>
		/// A <see cref="TimeSpan" /> representing the duration from <paramref name="dateTime" /> to <see cref="DateTime.Now" />. The value
		/// is positive if the input is in the past, and negative if it is in the future.
		/// </returns>
		/// <remarks>
		/// This method subtracts the specified <paramref name="dateTime" /> from <see cref="DateTime.Now" />, producing a positive result
		/// for past values and a negative result for future values. The comparison uses the system's current local time at the moment of evaluation.
		/// </remarks>
		public static TimeSpan GetElapsedTimeSince(this DateTime dateTime)
			=> DateTime.Now - dateTime;
	}
}
