// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.UnixTime(Epoch).cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Represents the maximum number of milliseconds since the Unix epoch that can be converted to <see cref="DateTime" />.
		/// </summary>
		internal const long MaxEpochMilliseconds = (MaxTicks / TicksPerMillisecond) - UnixEpochMilliseconds;

		/// <summary>
		/// Represents the maximum number of seconds since the Unix epoch that can be converted to <see cref="DateTime" />.
		/// </summary>
		internal const long MaxEpochSeconds = (MaxTicks / TicksPerSecond) - UnixEpochSeconds;

		/// <summary>
		/// Represents the minimum number of milliseconds since the Unix epoch that can be converted to <see cref="DateTime" />.
		/// </summary>
		internal const long MinEpochMilliseconds = (MinTicks / TicksPerMillisecond) - UnixEpochMilliseconds;

		/// <summary>
		/// Represents the minimum number of seconds since the Unix epoch that can be converted to <see cref="DateTime" />.
		/// </summary>
		internal const long MinEpochSeconds = (MinTicks / TicksPerSecond) - UnixEpochSeconds;

		/// <summary>
		/// Represents the number of milliseconds between <see cref="DateTime.MinValue" /> and the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		internal const long UnixEpochMilliseconds = UnixEpochTicks / TicksPerMillisecond;

		/// <summary>
		/// Represents the number of seconds between <see cref="DateTime.MinValue" /> and the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		internal const long UnixEpochSeconds = UnixEpochTicks / TicksPerSecond;

		/// <summary>
		/// Represents the number of ticks (100 nanoseconds) between <see cref="DateTime.MinValue" /> and the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		internal const long UnixEpochTicks = TicksPerDay * DaysTo1970;

		/// <summary>
		/// Converts a Unix timestamp expressed in milliseconds since 1970-01-01T00:00:00Z to a <see cref="DateTime" />.
		/// </summary>
		/// <param name="timestamp">The number of milliseconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateTime" /> in UTC representing the specified Unix timestamp.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="timestamp" /> falls outside the supported Unix timestamp range.
		/// </exception>
		/// <remarks>The returned <see cref="DateTime" /> has <see cref="DateTimeKind.Utc" />.</remarks>
		/// <seealso cref="ToUnixTimeMilliseconds(DateTime)" />
		public static DateTime FromUnixTimeMilliseconds(long timestamp)
		{
			ThrowHelper.ThrowIfOutOfRange(timestamp, MinEpochMilliseconds, MaxEpochMilliseconds);
			return new DateTime(UnixEpochTicks + (timestamp * TicksPerMillisecond), DateTimeKind.Utc);
		}

		/// <summary>
		/// Converts a Unix timestamp expressed in seconds since 1970-01-01T00:00:00Z to a <see cref="DateTime" />.
		/// </summary>
		/// <param name="timestamp">The number of seconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateTime" /> in UTC representing the specified Unix timestamp.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="timestamp" /> falls outside the supported Unix timestamp range.
		/// </exception>
		/// <remarks>The returned <see cref="DateTime" /> has <see cref="DateTimeKind.Utc" />.</remarks>
		/// <seealso cref="ToUnixTimeSeconds(DateTime)" />
		public static DateTime FromUnixTimeSeconds(long timestamp)
		{
			ThrowHelper.ThrowIfOutOfRange(timestamp, MinEpochSeconds, MaxEpochSeconds);
			return new DateTime(UnixEpochTicks + (timestamp * TicksPerSecond), DateTimeKind.Utc);
		}

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to the number of milliseconds that have elapsed since the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert. It is converted to UTC before calculation.</param>
		/// <returns>The number of milliseconds since the Unix epoch.</returns>
		/// <remarks>
		/// This method converts <paramref name="dateTime" /> to UTC via <see cref="DateTime.ToUniversalTime()" /> before computing the
		/// elapsed time.
		/// </remarks>
		/// <seealso cref="FromUnixTimeMilliseconds(long)" />
		public static long ToUnixTimeMilliseconds(this DateTime dateTime)
			=> (dateTime.ToUniversalTime().Ticks / TicksPerMillisecond) - UnixEpochMilliseconds;

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to the number of seconds that have elapsed since the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert. It is converted to UTC before calculation.</param>
		/// <returns>The number of seconds since the Unix epoch.</returns>
		/// <remarks>
		/// This method converts <paramref name="dateTime" /> to UTC via <see cref="DateTime.ToUniversalTime()" /> before computing the
		/// elapsed time.
		/// </remarks>
		/// <seealso cref="FromUnixTimeSeconds(long)" />
		public static long ToUnixTimeSeconds(this DateTime dateTime)
			=> (dateTime.ToUniversalTime().Ticks / TicksPerSecond) - UnixEpochSeconds;
	}
}