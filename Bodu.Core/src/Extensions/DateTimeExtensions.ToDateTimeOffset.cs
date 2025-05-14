// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.ToDateTimeOffset.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a <see cref="DateTimeOffset" />, interpreting the value based on its <see cref="DateTime.Kind" />.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> to convert.</param>
		/// <returns>
		/// A <see cref="DateTimeOffset" /> representing the same point in time as <paramref name="dateTime" />, using its
		/// <see cref="DateTime.Kind" /> to determine the offset (local, UTC, or unspecified).
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the conversion results in a UTC time that is outside the allowable range for <see cref="DateTimeOffset" />.
		/// </exception>
		/// <remarks>
		/// This method respects the <see cref="DateTime.Kind" /> value:
		/// <list type="bullet">
		/// <item>
		/// <term>UTC</term>
		/// <description>Returns a <see cref="DateTimeOffset" /> with zero offset.</description>
		/// </item>
		/// <item>
		/// <term>Local</term>
		/// <description>Applies the system’s local offset.</description>
		/// </item>
		/// <item>
		/// <term>Unspecified</term>
		/// <description>Assumes the local time zone context.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
			=> new(dateTime);

		/// <summary>
		/// Converts the specified <see cref="DateTime" /> to a <see cref="DateTimeOffset" /> using the provided UTC offset.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateTime" /> to convert.</param>
		/// <param name="offset">The <see cref="TimeSpan" /> offset from UTC to associate with the resulting <see cref="DateTimeOffset" />.</param>
		/// <returns>
		/// A <see cref="DateTimeOffset" /> representing the same clock time as <paramref name="dateTime" /> with the specified
		/// <paramref name="offset" /> applied.
		/// </returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="offset" /> is not valid for the specified <paramref name="dateTime.Kind" />, or if it creates an
		/// invalid combination.
		/// </exception>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if the resulting UTC time is outside the valid range supported by <see cref="DateTimeOffset" />.
		/// </exception>
		/// <remarks>
		/// Use this overload when the offset is known or must be explicitly applied (e.g., fixed timezone data). The
		/// <paramref name="offset" /> must be within ±14 hours.
		/// </remarks>
		public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime, TimeSpan offset)
			=> new(dateTime, offset);
	}
}