// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.NextOccurrence.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Calculates the next occurrence of a periodic event based on a given start time and interval, relative to a specified point in time.
		/// </summary>
		/// <param name="start">The <see cref="DateTime" /> representing the initial start time of the recurring event.</param>
		/// <param name="interval">A <see cref="TimeSpan" /> that defines how often the event recurs. Must be greater than zero.</param>
		/// <param name="after">A <see cref="DateTime" /> after which the next occurrence should be found.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the next occurrence of the event after the specified <paramref name="after" /> time.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="interval" /> is zero or negative.</exception>
		/// <remarks>
		/// The returned <see cref="DateTime" /> is aligned with the start time and repeated using the defined interval. For example, with a
		/// start of 9:00 AM and a 1-hour interval, the next occurrence after 10:45 AM would be 11:00 AM.
		/// </remarks>
		public static DateTime NextOccurrence(this DateTime start, TimeSpan interval, DateTime after)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

			if (after <= start)
				return start;

			// Calculate how many full intervals fit between start and after
			double intervalsPassed = (double)(after - start).Ticks / interval.Ticks;
			long nextIntervalCount = (long)Math.Ceiling(intervalsPassed);

			return start.AddTicks(nextIntervalCount * interval.Ticks);
		}
	}
}
