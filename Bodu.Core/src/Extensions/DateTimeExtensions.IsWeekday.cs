// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Globalization;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> falls on a weekday using the default
		/// <see cref="StandardWeekend.SaturdaySunday" /> rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns><see langword="true" /> if the <paramref name="dateTime" /> is not Saturday or Sunday; otherwise, <see langword="false" />.</returns>
		/// <remarks>A weekday is defined as any day not considered a weekend by the default rule.</remarks>
		public static bool IsWeekday(this DateTime dateTime) =>
			!IsWeekend(dateTime, StandardWeekend.SaturdaySunday, null);

		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> falls on a weekday, based on the provided
		/// <see cref="StandardWeekend" /> rule.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> to evaluate.</param>
		/// <param name="weekend">A <see cref="StandardWeekend" /> value that defines which days are considered part of the weekend.</param>
		/// <param name="provider">An optional <see cref="IWeekendProvider" /> used for custom weekend rules.</param>
		/// <returns>
		/// <see langword="true" /> if the <paramref name="dateTime" /> is considered a weekday under the specified weekend rule; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="weekend" /> is <see cref="StandardWeekend.Custom" /> but <paramref name="provider" /> is null.
		/// </exception>
		public static bool IsWeekday(this DateTime dateTime, StandardWeekend weekend, IWeekendProvider? provider = null) =>
			!IsWeekend(dateTime, weekend, provider);
	}
}
