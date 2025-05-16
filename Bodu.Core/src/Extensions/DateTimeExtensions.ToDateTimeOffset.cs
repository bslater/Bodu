// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="ToDateTimeOffset.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a <see cref="DateTimeOffset" />, using its <see cref="DateTime.Kind" /> to
		/// determine the time zone offset.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to convert.</param>
		/// <returns>
		/// A <see cref="DateTimeOffset" /> that represents the same point in time as <paramref name="dateTime" />, adjusted according to
		/// its <see cref="DateTime.Kind" /> ( <c>Local</c>, <c>Utc</c>, or <c>Unspecified</c>).
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the resulting UTC time is outside the valid range supported by <see cref="DateTimeOffset" />.
		/// </exception>
		/// <remarks>
		/// The <see cref="DateTime.Kind" /> property determines how the offset is interpreted:
		/// <list type="bullet">
		/// <item>
		/// <term><see cref="DateTimeKind.Utc" /></term>
		/// <description>Creates a <see cref="DateTimeOffset" /> with a zero offset (UTC).</description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeKind.Local" /></term>
		/// <description>Applies the local system time zone offset.</description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeKind.Unspecified" /></term>
		/// <description>Assumes the value is local and applies the system time zone offset.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime) =>
			new(dateTime);

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a <see cref="DateTimeOffset" /> using the provided UTC offset.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to convert.</param>
		/// <param name="offset">The UTC <see cref="TimeSpan" /> offset to associate with the resulting <see cref="DateTimeOffset" />.</param>
		/// <returns>
		/// A <see cref="DateTimeOffset" /> representing the same clock time as <paramref name="dateTime" />, with the specified
		/// <paramref name="offset" /> applied.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="offset" /> is not within the range ±14 hours, or if it is not compatible with the
		/// <see cref="DateTime.Kind" /> of <paramref name="dateTime" />.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the resulting UTC time falls outside the supported range of <see cref="DateTimeOffset" />.</exception>
		/// <remarks>
		/// Use this overload when a specific UTC offset must be applied—such as when working with historical time zone data or non-local
		/// time zones. The offset must be in the range [-14:00, +14:00].
		/// </remarks>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset) =>
			new(dateTime, offset);
	}
}