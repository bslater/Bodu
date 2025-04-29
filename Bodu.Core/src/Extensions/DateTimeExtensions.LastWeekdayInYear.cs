// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.LastWeekdayInYear.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the last occurrence of the specified <see cref="DayOfWeek" /> within the same calendar year as the given <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose year defines the search range.</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate. For example, <see cref="DayOfWeek.Sunday" /> returns the last Sunday in the year.
		/// </param>
		/// <returns>
		/// A new <see cref="DateTime" /> set to midnight on the last occurrence of the specified <paramref name="dayOfWeek" /> in the same
		/// year as <paramref name="dateTime" />, preserving the original <see cref="DateTime.Kind" />.
		/// </returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid <see cref="DayOfWeek" /> enumeration value.
		/// </exception>
		/// <remarks>
		/// The search begins from December 31 of the given year and proceeds backward until the specified <paramref name="dayOfWeek" /> is
		/// found. The time component is normalized to 00:00:00 (midnight), and the <see cref="DateTime.Kind" /> is preserved.
		/// </remarks>
		public static DateTime LastWeekdayInYear(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			long ticks = DateTimeExtensions.GetTicksForDate(dateTime.Year, 12, 31);
			ticks += DateTimeExtensions.GetPreviousDayOfWeekTicksFrom(ticks, dayOfWeek);
			return new DateTime(ticks, dateTime.Kind);
		}
	}
}
