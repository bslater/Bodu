// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.PreviousWeekday.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateOnly" /> that represents the previous occurrence of the
		/// specified <see cref="DayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">The starting <see cref="DateOnly" />.</param>
		/// <param name="dayOfWeek">The desired day of the week to find.</param>
		/// <returns>A <see cref="DateOnly" /> at the previous occurrence of the specified day.</returns>
		/// <exception cref="System.ArgumentOutOfRangeException">
		/// Thrown if <paramref name="dayOfWeek" /> is not a valid enum value.
		/// </exception>
		/// <remarks>
		/// If the <paramref name="dateTime" /> is already on the specified day, the result will be
		/// 7 days earlier.
		/// </remarks>
		public static DateOnly PreviousWeekday(this DateOnly date, DayOfWeek dayOfWeek)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

			return dateTime.AddTicks(dateTime.DayOfWeek == dayOfWeek ? -DateOnlyExtensions.TicksPerWeek : DateOnlyExtensions.PreviousDayOfWeekTicks(dateTime, dayOfWeek));
		}
	}
}