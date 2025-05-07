// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.FirstDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateOnly" />, using the current culture's settings.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateOnly" /> for which to determine the start of the week.</param>
		/// <returns>A <see cref="DateOnly" /> representing the first day of the week that includes <paramref name="dateTime" />.</returns>
		/// <remarks>This method uses <see cref="CultureInfo.CurrentCulture" /> to determine the first day of the week, based on <see cref="DateTimeFormatInfo.FirstDayOfWeek" />.</remarks>
		public static DateOnly FirstDayOfWeek(this DateOnly dateTime)
			=> dateTime.FirstDayOfWeek(null!);

		/// <summary>
		/// Returns the first day of the week that contains the specified <see cref="DateOnly" />, using the provided culture or the current culture.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateOnly" /> for which to determine the start of the week.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> that defines the first day of the week via
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" />. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>A <see cref="DateOnly" /> representing the first day of the week that includes <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// The method calculates the number of days to subtract from <paramref name="dateTime" /> to reach the start of the week as defined
		/// by the specified or current culture. The result is validated to ensure it falls within the valid range of <see cref="DateOnly" />.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the calculated date is outside the valid range supported by <see cref="DateOnly" />.</exception>
		public static DateOnly FirstDayOfWeek(this DateOnly dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;

			int offsetDays = ((7 + (dateTime.DayOfWeek - firstDayOfWeek)) % 7);

			int dayNumber = dateTime.DayNumber - offsetDays;

			if (dayNumber < DateOnly.MinValue.DayNumber || dayNumber > DateOnly.MaxValue.DayNumber)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateOnly)));

			return DateOnly.FromDayNumber(dayNumber);
		}
	}
}