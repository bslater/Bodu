// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Truncate.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> with the value truncated to the specified <paramref name="resolution" />, resetting all
		/// smaller time components to zero.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to truncate.</param>
		/// <param name="resolution">The <see cref="DateTimeResolution" /> level to which the input should be truncated.</param>
		/// <returns>A <see cref="DateTime" /> with smaller time components cleared based on the specified <paramref name="resolution" />.</returns>
		/// <exception cref="System.ArgumentException">
		/// Thrown if <paramref name="resolution" /> is not a defined value of the <see cref="DateTimeResolution" /> enumeration.
		/// </exception>
		/// <remarks>
		/// This method returns a new <see cref="DateTime" /> with smaller components reset to zero depending on the specified
		/// <paramref name="resolution" />. The original <see cref="DateTime.Kind" /> is preserved in the result.
		/// <para>For example, given the input <c>2024-04-18T14:37:56.7891234</c>:</para>
		/// <list type="table">
		/// <listheader>
		/// <term>Resolution</term>
		/// <description>Truncation Behavior and Result</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="DateTimeResolution.Year" /></term>
		/// <description>Truncates to the start of the year: January 1 at 00:00:00. <br /> Result: <c>2024-01-01T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Month" /></term>
		/// <description>Truncates to the start of the month: 1st of April at 00:00:00. <br /> Result: <c>2024-04-01T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Day" /></term>
		/// <description>Truncates to the start of the day (midnight). <br /> Result: <c>2024-04-18T00:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Hour" /></term>
		/// <description>Minute, second, and sub-second components set to zero. <br /> Result: <c>2024-04-18T14:00:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Minute" /></term>
		/// <description>Second and sub-second components set to zero. <br /> Result: <c>2024-04-18T14:37:00.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Second" /></term>
		/// <description>Sub-second (millisecond and ticks) components set to zero. <br /> Result: <c>2024-04-18T14:37:56.0000000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Millisecond" /></term>
		/// <description>Ticks below the millisecond level set to zero. <br /> Result: <c>2024-04-18T14:37:56.7890000</c></description>
		/// </item>
		/// <item>
		/// <term><see cref="DateTimeResolution.Tick" /></term>
		/// <description>No truncation applied; the original value is returned. <br /> Result: <c>2024-04-18T14:37:56.7891234</c></description>
		/// </item>
		/// </list>
		/// </remarks>
		public static DateTime Truncate(this DateTime dateTime, DateTimeResolution resolution)
		{
			switch (resolution)
			{
				case DateTimeResolution.Year:
					return new DateTime(DateTimeExtensions.GetTicksForDate(dateTime.Year, 1, 1), dateTime.Kind);

				case DateTimeResolution.Month:
					return new DateTime(DateTimeExtensions.GetTicksForDate(dateTime.Year, dateTime.Month, 1), dateTime.Kind);

				case DateTimeResolution.Day:
					return new DateTime(DateTimeExtensions.GetTicks(dateTime), dateTime.Kind);

				case DateTimeResolution.Hour:
					return dateTime.AddTicks(-(dateTime.Ticks % DateTimeExtensions.TicksPerHour));

				case DateTimeResolution.Minute:
					return dateTime.AddTicks(-(dateTime.Ticks % DateTimeExtensions.TicksPerMinute));

				case DateTimeResolution.Second:
					return dateTime.AddTicks(-(dateTime.Ticks % DateTimeExtensions.TicksPerSecond));

				case DateTimeResolution.Millisecond:
					return dateTime.AddTicks(-(dateTime.Ticks % DateTimeExtensions.TicksPerMillisecond));

				case DateTimeResolution.Tick:
					return dateTime.AddTicks(0);

				default:
					throw new ArgumentException(
						string.Format(ResourceStrings.Arg_Invalid_EnumValue, nameof(DateTimeResolution)),
						nameof(resolution));
			}
		}
	}
}
