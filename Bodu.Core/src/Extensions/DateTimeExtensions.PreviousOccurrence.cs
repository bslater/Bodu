// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="NextOccurrence.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Calculates the most recent occurrence of a recurring event based on a specified <paramref name="start" /> time and fixed
		/// <paramref name="interval" />, occurring strictly before the given <paramref name="before" /> timestamp.
		/// </summary>
		/// <param name="start">The <see cref="DateTime" /> representing the initial reference point for the recurring event.</param>
		/// <param name="interval">The <see cref="TimeSpan" /> interval between occurrences. Must be greater than <see cref="TimeSpan.Zero" />.</param>
		/// <param name="before">
		/// A <see cref="DateTime" /> value representing the point in time before which the previous occurrence should be determined.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the last occurrence of the event strictly before <paramref name="before" />, based on the
		/// given <paramref name="start" /> and recurring <paramref name="interval" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="interval" /> is less than or equal to <see cref="TimeSpan.Zero" />.</exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="before" /> is less than or equal to <paramref name="start" />, the method returns <paramref name="start" />
		/// minus one interval.
		/// </para>
		/// <para>
		/// Otherwise, the method calculates the largest multiple of <paramref name="interval" /> added to <paramref name="start" /> that
		/// occurs strictly before <paramref name="before" />. For example, if <paramref name="start" /> is 09:00 and
		/// <paramref name="interval" /> is 1 hour, calling this method with a <paramref name="before" /> of 10:45 returns 10:00.
		/// </para>
		/// </remarks>
		public static DateTime PreviousOccurrence(this DateTime start, TimeSpan interval, DateTime before)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

			if (before <= start)
				return start.AddTicks(-interval.Ticks);

			double intervalsPassed = (double)(before - start).Ticks / interval.Ticks;
			long previousIntervalCount = (long)Math.Floor(intervalsPassed);

			return start.AddTicks(previousIntervalCount * interval.Ticks);
		}
	}
}