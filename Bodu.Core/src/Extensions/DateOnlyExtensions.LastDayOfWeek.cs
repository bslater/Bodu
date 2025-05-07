// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.LastDayOfWeek.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Globalization.Extensions;
using System;
using System.Globalization;
using System.Threading;

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateOnly" />, using the current culture's settings.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateOnly" /> for which to determine the end of the week.</param>
		/// <returns>A <see cref="DateOnly" /> representing the last day of the week that includes <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses <see cref="CultureInfo.CurrentCulture" /> to determine the last day of the week, inferred from <see cref="DateTimeFormatInfo.FirstDayOfWeek" />.
		/// </remarks>
		public static DateOnly LastDayOfWeek(this DateOnly dateTime) =>
			dateTime.LastDayOfWeek(null!);

		/// <summary>
		/// Returns the last day of the week that contains the specified <see cref="DateOnly" />, using the provided or current culture.
		/// </summary>
		/// <param name="dateTime">The input <see cref="DateOnly" /> for which to determine the end of the week.</param>
		/// <param name="culture">
		/// An optional <see cref="CultureInfo" /> used to determine the first day of the week via
		/// <see cref="DateTimeFormatInfo.FirstDayOfWeek" />. If <c>null</c>, <see cref="CultureInfo.CurrentCulture" /> is used.
		/// </param>
		/// <returns>A <see cref="DateOnly" /> representing the last day of the week that includes <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method calculates the number of days to add to <paramref name="dateTime" /> to reach the culturally defined last day of the
		/// week. The result is validated to ensure it falls within the valid range of <see cref="DateOnly" />.
		/// </remarks>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the calculated result exceeds the supported range for <see cref="DateOnly" />.</exception>
		public static DateOnly LastDayOfWeek(this DateOnly dateTime, CultureInfo culture)
		{
			culture ??= Thread.CurrentThread.CurrentCulture;
			DayOfWeek lastDayOfWeek = culture.DateTimeFormat.LastDayOfWeek();

			int offsetDays = dateTime.DayOfWeek == lastDayOfWeek
				? 0
				: DateOnlyExtensions.GetNextDayOfWeekOffset(dateTime, lastDayOfWeek);

			int dayNumber = dateTime.DayNumber + offsetDays;

			if (dayNumber < DateOnly.MinValue.DayNumber || dayNumber > DateOnly.MaxValue.DayNumber)
				throw new ArgumentOutOfRangeException(nameof(dateTime),
					string.Format(ResourceStrings.Arg_OutOfRange_ResultingValueOutOfRangeForType, nameof(DateOnly)));

			return DateOnly.FromDayNumber(dayNumber);
		}
	}
}