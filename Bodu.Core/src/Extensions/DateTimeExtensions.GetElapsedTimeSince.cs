// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.GetElapsedTimeSince.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the time elapsed between the specified <see cref="DateTime" /> and the current UTC time.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to compare against.</param>
		/// <returns>
		/// A <see cref="TimeSpan" /> representing the duration between <paramref name="dateTime" /> and <see cref="DateTime.UtcNow" />. A
		/// positive value indicates a past timestamp, while a negative value indicates a future timestamp.
		/// </returns>
		/// <exception cref="ArgumentException">Thrown if <paramref name="dateTime" /> has <see cref="DateTimeKind.Unspecified" />.</exception>
		/// <remarks>
		/// <para>
		/// This method calculates elapsed time relative to the current UTC time using <see cref="DateTime.UtcNow" />. It automatically
		/// converts local time inputs to UTC using <see cref="DateTime.ToUniversalTime()" />.
		/// </para>
		/// <para>
		/// If the input <see cref="DateTime" /> has a <see cref="DateTimeKind" /> of <see cref="DateTimeKind.Unspecified" />, an exception
		/// is thrown to avoid ambiguous behavior. Callers should explicitly specify <see cref="DateTimeKind.Utc" /> or
		/// <see cref="DateTimeKind.Local" /> when using this method.
		/// </para>
		/// </remarks>
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