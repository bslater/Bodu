// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.UnixTime(Epoch).cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Represents the maximum number of Epoch milliseconds. This field is constant.
		/// </summary>
		internal const long MaxEpochMilliseconds = (DateTimeExtensions.MaxTicks / TimeSpan.TicksPerMillisecond) - DateTimeExtensions.UnixEpochMilliseconds;

		/// <summary>
		/// Represents the maximum number of Epoch seconds. This field is constant.
		/// </summary>
		internal const long MaxEpochSeconds = (DateTimeExtensions.MaxTicks / TimeSpan.TicksPerSecond) - DateTimeExtensions.UnixEpochSeconds;

		/// <summary>
		/// Represents the minimum number of Epoch milliseconds. This field is constant.
		/// </summary>
		internal const long MinEpochMilliseconds = (DateTimeExtensions.MinTicks / TimeSpan.TicksPerMillisecond) - DateTimeExtensions.UnixEpochMilliseconds;

		/// <summary>
		/// Represents the minimum number of Epoch seconds. This field is constant.
		/// </summary>
		internal const long MinEpochSeconds = (DateTimeExtensions.MinTicks / TimeSpan.TicksPerSecond) - DateTimeExtensions.UnixEpochSeconds;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 Epoch millisecond. This field is constant.
		/// </summary>
		internal const long UnixEpochMilliseconds = DateTimeExtensions.UnixEpochTicks / TimeSpan.TicksPerMillisecond;

		/// <summary>
		/// Represents the number of ticks (100ns) in 1 Epoch second. This field is constant.
		/// </summary>
		internal const long UnixEpochSeconds = DateTimeExtensions.UnixEpochTicks / TimeSpan.TicksPerSecond;

		/// <summary>
		/// Represents the number of ticks (100ns) to 1970. This field is constant.
		/// </summary>
		internal const long UnixEpochTicks = DateTimeExtensions.TicksPerDay * DateTimeExtensions.DaysTo1970;

		/// <summary>
		/// Converts a Unix timestamp expressed in milliseconds since 1970-01-01T00:00:00Z to a <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The number of milliseconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateTime" /> in UTC that represents the specified Unix timestamp.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dateTime" /> falls outside the supported Unix timestamp range.
		/// </exception>
		/// <remarks>The returned <see cref="DateTime" /> has <see cref="DateTimeKind.Utc" />.</remarks>
		public static DateTime FromUnixTimeMilliseconds(this long dateTime)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(dateTime, DateTimeExtensions.MinEpochMilliseconds, DateTimeExtensions.MaxEpochMilliseconds);

			return new DateTime(DateTimeExtensions.UnixEpochTicks + (dateTime * DateTimeExtensions.TicksPerMillisecond), DateTimeKind.Utc);
		}

		/// <summary>
		/// Converts a Unix timestamp expressed in seconds since 1970-01-01T00:00:00Z to a <see cref="DateTime" />.
		/// </summary>
		/// <param name="dateTime">The number of seconds elapsed since the Unix epoch (1970-01-01T00:00:00Z).</param>
		/// <returns>A <see cref="DateTime" /> in UTC that represents the specified Unix timestamp.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dateTime" /> falls outside the supported Unix timestamp range.
		/// </exception>
		/// <remarks>The returned <see cref="DateTime" /> has <see cref="DateTimeKind.Utc" />.</remarks>
		public static DateTime FromUnixTimeSeconds(this long dateTime)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(dateTime, DateTimeExtensions.MinEpochSeconds, DateTimeExtensions.MaxEpochSeconds);

			return new DateTime(DateTimeExtensions.UnixEpochTicks + (dateTime * DateTimeExtensions.TicksPerSecond), DateTimeKind.Utc);
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
		public static long ToUnixTimeMilliseconds(this DateTime dateTime)
			=> (dateTime.ToUniversalTime().Ticks / DateTimeExtensions.TicksPerMillisecond) - DateTimeExtensions.UnixEpochMilliseconds;

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to the number of seconds that have elapsed since the Unix epoch (1970-01-01T00:00:00Z).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to convert. It is converted to UTC before calculation.</param>
		/// <returns>The number of seconds since the Unix epoch.</returns>
		/// <remarks>
		/// This method converts <paramref name="dateTime" /> to UTC via <see cref="DateTime.ToUniversalTime()" /> before computing the
		/// elapsed time.
		/// </remarks>
		public static long ToUnixTimeSeconds(this DateTime dateTime)
			=> (dateTime.ToUniversalTime().Ticks / DateTimeExtensions.TicksPerSecond) - DateTimeExtensions.UnixEpochSeconds;
	}
}
