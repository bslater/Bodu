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
		/// Returns a new <see cref="DateTime" /> representing the previous occurrence of a recurring event based on a specified
		/// <paramref name="dateTime" /> time and fixed <paramref name="interval" />, occurring strictly before the given
		/// <paramref name="before" /> timestamp.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> representing the initial reference point for the recurring event.</param>
		/// <param name="interval">The fixed <see cref="TimeSpan" /> between occurrences. Must be greater than <see cref="TimeSpan.Zero" />.</param>
		/// <param name="before">
		/// A <see cref="DateTime" /> indicating the point in time before which the previous occurrence should be determined.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> representing the last occurrence of the event that falls strictly before
		/// <paramref name="before" />, based on the specified <paramref name="dateTime" /> and recurring <paramref name="interval" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="interval" /> is less than or equal to <see cref="TimeSpan.Zero" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <paramref name="before" /> is earlier than or equal to <paramref name="dateTime" />, the method returns the occurrence
		/// immediately prior to <paramref name="dateTime" />.
		/// </para>
		/// <para>
		/// Otherwise, the method computes the largest multiple of <paramref name="interval" /> added to <paramref name="dateTime" />
		/// that remains strictly less than <paramref name="before" />.
		/// </para>
		/// <para>
		/// The <see cref="DateTime.Kind" /> property of the returned instance matches that of the original <paramref name="dateTime" />.
		/// </para>
		/// <code language="csharp">
		///<![CDATA[
		/// var start = new DateTime(2025, 7, 7, 9, 0, 0);              // 09:00
		/// var interval = TimeSpan.FromHours(1);                       // Every hour
		/// var before = new DateTime(2025, 7, 7, 10, 45, 0);           // 10:45
		/// var previous = start.PreviousOccurrence(interval, before);  // 10:00
		///]]>
		/// </code>
		/// </remarks>
		public static DateTime PreviousOccurrence(this DateTime dateTime, TimeSpan interval, DateTime before)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(interval, TimeSpan.Zero);

			if (before <= dateTime)
				return dateTime.AddTicks(-interval.Ticks);

			double intervalsPassed = (double)(before - dateTime).Ticks / interval.Ticks;
			long previousIntervalCount = (long)Math.Floor(intervalsPassed);

			return dateTime.AddTicks(previousIntervalCount * interval.Ticks);
		}
	}
}