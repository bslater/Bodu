// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="Truncate.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> truncated to the specified <paramref name="resolution" />, with all smaller time
		/// components reset to zero.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to truncate.</param>
		/// <param name="resolution">The <see cref="DateTimeResolution" /> level to truncate to.</param>
		/// <returns>
		/// A new <see cref="DateTime" /> with all components smaller than the specified <paramref name="resolution" /> cleared, while
		/// preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="resolution" /> is not a valid member of the <see cref="DateTimeResolution" /> enumeration.
		/// </exception>
		/// <remarks>
		/// The following table illustrates truncation results for the value <c>2024-04-18T14:37:56.7891234</c>:
		/// <list type="table">
		/// <listheader>
		/// <term>Resolution</term>
		/// <description>Result</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="DateTimeResolution.Year" /></term>
		/// <description><c>2024-01-01T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Month" /></term>
		/// <description><c>2024-04-01T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Day" /></term>
		/// <description><c>2024-04-18T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Hour" /></term>
		/// <description><c>2024-04-18T14:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Minute" /></term>
		/// <description><c>2024-04-18T14:37:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Second" /></term>
		/// <description><c>2024-04-18T14:37:56.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Millisecond" /></term>
		/// <description><c>2024-04-18T14:37:56.7890000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Tick" /></term>
		/// <description><c>2024-04-18T14:37:56.7891234</c> (unchanged)</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTime Truncate(this DateTime dateTime, DateTimeResolution resolution)
		{
			switch (resolution)
			{
				case DateTimeResolution.Year:
					return new (GetTicksForDate(dateTime.Year, 1, 1), dateTime.Kind);

				case DateTimeResolution.Month:
					return new (GetTicksForDate(dateTime.Year, dateTime.Month, 1), dateTime.Kind);

				case DateTimeResolution.Day:
					return new (GetDateTicks(dateTime), dateTime.Kind);

				case DateTimeResolution.Hour:
					return dateTime.AddTicks(-(dateTime.Ticks % TicksPerHour));

				case DateTimeResolution.Minute:
					return dateTime.AddTicks(-(dateTime.Ticks % TicksPerMinute));

				case DateTimeResolution.Second:
					return dateTime.AddTicks(-(dateTime.Ticks % TicksPerSecond));

				case DateTimeResolution.Millisecond:
					return dateTime.AddTicks(-(dateTime.Ticks % TicksPerMillisecond));

				case DateTimeResolution.Tick:
					return dateTime;

				default:
					throw new ArgumentException(
						string.Format(ResourceStrings.Arg_OutOfRangeException_EnumValue, resolution, nameof(DateTimeResolution)),
						nameof(resolution));
			}
		}
	}
}