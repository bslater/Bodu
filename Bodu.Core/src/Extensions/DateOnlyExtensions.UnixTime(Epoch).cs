// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.UnixTime(Epoch).cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Converts a Unix timestamp in milliseconds since 1970-01-01T00:00:00Z to a <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">A Unix time expressed in milliseconds.</param>
		/// <returns>A <see cref="DateOnly" /> in UTC corresponding to the Unix time.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the value is outside the supported Unix timestamp range.
		/// </exception>
		public static DateOnly FromUnixTimeMilliseconds(this long dateTime)
		{
			ThrowHelper.ThrowIfNotBetweenExclusive(dateTime, DateOnlyExtensions.MinEpochMilliseconds, DateOnlyExtensions.MaxEpochMilliseconds);

			return new DateOnly(DateOnlyExtensions.UnixEpochTicks + (dateTime * DateOnlyExtensions.TicksPerMillisecond), DateOnlyKind.Utc);
		}

		/// <summary>
		/// Converts a Unix timestamp in seconds since 1970-01-01T00:00:00Z to a <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">A Unix time expressed in seconds.</param>
		/// <returns>A <see cref="DateOnly" /> in UTC corresponding to the Unix time.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the value is outside the supported Unix timestamp range.
		/// </exception>
		public static DateOnly FromUnixTimeSeconds(this long dateTime)
		{
			ThrowHelper.ThrowIfNotBetweenExclusive(dateTime, DateOnlyExtensions.MinEpochSeconds, DateOnlyExtensions.MaxEpochSeconds);

			return new DateOnly(DateOnlyExtensions.UnixEpochTicks + (dateTime * DateOnlyExtensions.TicksPerSecond), DateOnlyKind.Utc);
		}

		/// <summary>
		/// Returns the number of milliseconds since 1970-01-01T00:00:00Z for the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="this">The <see cref="DateOnly" /> to convert.</param>
		/// <returns>The number of milliseconds elapsed since Unix epoch.</returns>
		/// <remarks>The input <see cref="DateOnly" /> is converted to UTC before the calculation.</remarks>
		public static long ToUnixTimeMilliseconds(this DateOnly @this)
			=> (@this.ToUniversalTime().Ticks / DateOnlyExtensions.TicksPerMillisecond) - DateOnlyExtensions.UnixEpochMilliseconds;

		/// <summary>
		/// Returns the number of seconds since 1970-01-01T00:00:00Z for the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="this">The <see cref="DateOnly" /> to convert.</param>
		/// <returns>The number of seconds elapsed since Unix epoch.</returns>
		/// <remarks>The input <see cref="DateOnly" /> is converted to UTC before the calculation.</remarks>
		public static long ToUnixTimeSeconds(this DateOnly @this)
			=> (@this.ToUniversalTime().Ticks / DateOnlyExtensions.TicksPerSecond) - DateOnlyExtensions.UnixEpochSeconds;
	}
}