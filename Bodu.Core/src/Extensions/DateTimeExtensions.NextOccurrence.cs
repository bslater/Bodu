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
		/// Calculates the next occurrence of a recurring event based on a specified <paramref name="start" /> time and fixed
		/// <paramref name="interval" />, occurring strictly after the given <paramref name="after" /> timestamp.
		/// </summary>
		/// <param name="start">The <see cref="DateTime" /> representing the initial reference point for the recurring event.</param>
		/// <param name="interval">The <see cref="TimeSpan" /> interval between occurrences. Must be greater than <see cref="TimeSpan.Zero" />.</param>
		/// <param name="after">
		/// A <see cref="DateTime" /> value representing the point in time after which the next occurrence should be determined.
		/// </param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first occurrence of the event strictly after <paramref name="after" />, based on the
		/// given <paramref name="start" /> and recurring <paramref name="interval" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="interval" /> is less than or equal to <see cref="TimeSpan.Zero" />.</exception>
		/// <remarks>
		/// <para>If <paramref name="after" /> is earlier than or equal to <paramref name="start" />, the method returns <paramref name="start" />.</para>
		/// <para>
		/// Otherwise, the method calculates the smallest multiple of <paramref name="interval" /> added to <paramref name="start" /> that
		/// occurs after <paramref name="after" />. For example, if <paramref name="start" /> is 09:00 and <paramref name="interval" /> is 1
		/// hour, calling this method with an <paramref name="after" /> of 10:45 returns 11:00.
		/// </para>
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