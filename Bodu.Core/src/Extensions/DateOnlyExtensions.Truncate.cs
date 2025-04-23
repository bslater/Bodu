// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.Truncate.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		// <summary>
		/// Truncates the value of the specified <see cref="DateOnly"/> to the specified resolution.
		/// </summary> <param name="dateTime">The <see cref="DateOnly"/> to truncate.</param> <param
		/// name="resolution">The resolution to truncate to. See <paramref name="resolution"/>
		/// values in Remarks.</param> <returns>A new <see cref="DateOnly"/> truncated to the
		/// specified resolution.</returns> <exception cref="System.ArgumentException"><paramref
		/// name="resolution"/> is not a valid <see cref="DateOnlyResolution"/> value.</exception>
		/// <remarks> <para>This method returns a new <see cref="DateOnly"/> with all smaller time
		/// components set to zero based on the selected resolution.</para> <list type="table">
		/// <listheader><term>Resolution</term><description>Truncating
		/// Behavior</description></listheader> <item><term><see
		/// cref="DateOnlyResolution.Year"/></term><description>Truncates to the start of the year
		/// (January 1 at 00:00:00).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Month"/></term><description>Truncates to the start of the month
		/// (1st day at 00:00:00).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Day"/></term><description>Truncates to the start of the day
		/// (00:00:00).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Hour"/></term><description>Truncates to the start of the hour
		/// (minutes, seconds, and milliseconds set to 0).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Minute"/></term><description>Truncates to the start of the
		/// minute (seconds and milliseconds set to 0).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Second"/></term><description>Truncates to the start of the
		/// second (milliseconds set to 0).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Millisecond"/></term><description>Truncates to the start of the
		/// millisecond (ticks set to 0).</description></item> <item><term><see
		/// cref="DateOnlyResolution.Tick"/></term><description>No truncation is applied; the
		/// original value is returned.</description></item> </list> </remarks>
		public static DateOnly Truncate(this DateOnly date, DateOnlyResolution resolution)
		{
			switch (resolution)
			{
				case DateOnlyResolution.Year:
					return new DateOnly(DateOnlyExtensions.DateToTicks(dateTime.Year, 1, 1), dateTime.Kind);

				case DateOnlyResolution.Month:
					return new DateOnly(DateOnlyExtensions.DateToTicks(dateTime.Year, dateTime.Month, 1), dateTime.Kind);

				case DateOnlyResolution.Day:
					return new DateOnly(DateOnlyExtensions.DateTicks(dateTime), dateTime.Kind);

				case DateOnlyResolution.Tick:
					return dateTime.AddTicks(0);

				default:
					throw new ArgumentException(
						string.Format(SR.Arg_Invalid_EnumValue, nameof(DateOnlyResolution)),
						nameof(resolution));
			}
		}
	}
}