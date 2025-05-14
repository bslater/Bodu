// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.NearestDayOfWeek.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the nearest date to <paramref name="dateTime" /> that falls on the specified <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">The reference date.</param>
		/// <param name="dayOfWeek">The <see cref="DayOfWeek" /> to find.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the closest date (before or after) to <paramref name="dateTime" /> that falls on the
		/// specified <paramref name="dayOfWeek" />. If two dates are equally close, the earlier one is returned.
		/// </returns>
		public static DateTime NearestDayOfWeek(this DateTime dateTime, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return new DateTime(GetNearestDayOfWeek(dateTime.Ticks, dayOfWeek), dateTime.Kind);
		}

		/// <summary>
		/// Returns the nearest date to the specified year/month/day that falls on the given <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="year">The year component of the reference date.</param>
		/// <param name="month">The month component of the reference date.</param>
		/// <param name="day">The day component of the reference date.</param>
		/// <param name="dayOfWeek">The target <see cref="DayOfWeek" /> to find.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the closest date (before or after) to the input date that falls on the specified
		/// <paramref name="dayOfWeek" />. If two dates are equally close, the earlier one is returned.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid <see cref="DayOfWeek" /> value.
		/// </exception>
		public static DateTime NearestDayOfWeek(int year, int month, int day, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return new DateTime(GetNearestDayOfWeek(GetTicksForDate(year, month, day), dayOfWeek), DateTimeKind.Unspecified);
		}
	}
}