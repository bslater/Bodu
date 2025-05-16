// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="GetElapsedTimeSince.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the time elapsed between the specified <see cref="DateTime" /> and the current UTC time.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to compare against the current UTC time.</param>
		/// <returns>
		/// A <see cref="TimeSpan" /> representing the duration between <paramref name="dateTime" /> and <see cref="DateTime.UtcNow" />. A
		/// positive value indicates that <paramref name="dateTime" /> is in the past; a negative value indicates it is in the future.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The method evaluates elapsed time relative to <see cref="DateTime.UtcNow" />. If the input is in local time, it is first
		/// converted to UTC using <see cref="DateTime.ToUniversalTime" />.
		/// </para>
		/// <para>
		/// If the <paramref name="dateTime" /> has a <see cref="DateTime.Kind" /> of <see cref="DateTimeKind.Unspecified" />, an exception
		/// is thrown to avoid ambiguity in time zone interpretation.
		/// </para>
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="dateTime" /> has <see cref="DateTimeKind.Unspecified" />, as the UTC offset cannot be determined.
		/// </exception>
		public static TimeSpan GetElapsedTimeSince(this DateTime dateTime)
		{
			DateTime utcInput = dateTime.Kind switch
			{
				DateTimeKind.Utc => dateTime,
				DateTimeKind.Local => dateTime.ToUniversalTime(),
				_ => throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_ValueForOperation, nameof(DateTime.Kind), $"{nameof(DateTimeKind.Utc)} or {nameof(DateTimeKind.Local)}", dateTime.Kind),
					nameof(dateTime))
			};

			return DateTime.UtcNow - utcInput;
		}
	}
}