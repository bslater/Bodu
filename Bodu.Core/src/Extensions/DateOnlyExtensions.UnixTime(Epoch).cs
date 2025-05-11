// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensions.UnixTime(Epoch).cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Converts a Unix timestamp expressed in milliseconds since 1970-01-01T00:00:00Z to a <see cref="DateOnly" />.
		/// </summary>
		/// <param name="timestamp">The number of milliseconds since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateOnly" /> representing the calendar date in UTC.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="timestamp" /> falls outside the supported Unix timestamp range.
		/// </exception>
		public static DateOnly FromUnixTimeMilliseconds(this long timestamp)
		{
			ThrowHelper.ThrowIfOutOfRange(timestamp, DateTimeExtensions.MinEpochMilliseconds, DateTimeExtensions.MaxEpochMilliseconds);
			return DateOnly.FromDayNumber((int)((DateTimeExtensions.UnixEpochTicks + timestamp * DateTimeExtensions.TicksPerMillisecond) / DateTimeExtensions.TicksPerDay));
		}

		/// <summary>
		/// Converts a Unix timestamp expressed in seconds since 1970-01-01T00:00:00Z to a <see cref="DateOnly" />.
		/// </summary>
		/// <param name="timestamp">The number of seconds since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateOnly" /> representing the calendar date in UTC.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="timestamp" /> falls outside the supported Unix timestamp range.
		/// </exception>
		public static DateOnly FromUnixTimeSeconds(this long timestamp)
		{
			ThrowHelper.ThrowIfOutOfRange(timestamp, DateTimeExtensions.MinEpochSeconds, DateTimeExtensions.MaxEpochSeconds);
			return DateOnly.FromDayNumber((int)((DateTimeExtensions.UnixEpochTicks + timestamp * DateTimeExtensions.TicksPerSecond) / DateTimeExtensions.TicksPerDay));
		}

		/// <summary>
		/// Converts the specified <see cref="DateOnly" /> to the number of milliseconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		/// <param name="date">The <see cref="DateOnly" /> to convert. The date is interpreted as UTC.</param>
		/// <returns>The number of milliseconds since the Unix epoch.</returns>
		public static long ToUnixTimeMilliseconds(this DateOnly date) =>
			(date.DayNumber * DateTimeExtensions.TicksPerDay - DateTimeExtensions.UnixEpochTicks) / DateTimeExtensions.TicksPerMillisecond;

		/// <summary>
		/// Converts the specified <see cref="DateOnly" /> to the number of seconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		/// <param name="date">The <see cref="DateOnly" /> to convert. The date is interpreted as UTC.</param>
		/// <returns>The number of seconds since the Unix epoch.</returns>
		public static long ToUnixTimeSeconds(this DateOnly date) =>
			(date.DayNumber * DateTimeExtensions.TicksPerDay - DateTimeExtensions.UnixEpochTicks) / DateTimeExtensions.TicksPerSecond;
	}
}