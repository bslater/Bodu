// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	/// <summary>
	/// Provides a set of <see langword="static" /> ( <see langword="Shared" /> in Visual Basic) methods that extend the
	/// <see cref="System.DateOnly" /> class.
	/// </summary>
	public static partial class DateOnlyExtensions
	{
		internal static void GetDateParts(this DateOnly date, out int year, out int month, out int day)
		{
			year = date.Year;
			month = date.Month;
			day = date.Day;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int GetDayNumber(int year, int month, int day)
		{
			bool isLeap = DateTime.IsLeapYear(year);
			int[] days = isLeap ? DateTimeExtensions.DaysToMonth366 : DateTimeExtensions.DaysToMonth365;

			return (int)(
				(long)(year - 1) * 365
				+ (year - 1) / 4
				- (year - 1) / 100
				+ (year - 1) / 400
				+ days[month - 1]
				+ day - 1); // Subtract 1 to match DateOnly.DayNumber origin
		}

		/// <summary>
		/// Calculates the <see cref="System.DayOfWeek" /> for a date represented as a tick count.
		/// </summary>
		/// <param name="days">
		/// A date and time expressed as the number of 100-nanosecond intervals (ticks) that have elapsed since January 1, 0001 at
		/// 00:00:00.000, based on the proleptic Gregorian calendar.
		/// </param>
		/// <returns>A <see cref="DayOfWeek" /> value indicating the day of the week corresponding to the specified <paramref name="ticks" />.</returns>
		/// <remarks>
		/// <para>
		/// This method computes the day of the week using modulo arithmetic based on the number of days since 0001-01-01. It is equivalent
		/// in result to <see cref="DateTime.DayOfWeek" /> but avoids object instantiation and is optimized for tick-level operations.
		/// </para>
		/// <para>
		/// No argument validation is performed. The caller is responsible for ensuring that the input <paramref name="ticks" /> value is
		/// valid within the supported <see cref="DateTime" /> range.
		/// </para>
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static DayOfWeek GetDayOfWeekFromDayNumber(int days)
		=> (DayOfWeek)((days + 1) % 7);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetPreviousDayOfWeekFromDayNumber(int days, DayOfWeek dayOfWeek)
		{
			DayOfWeek day = GetDayOfWeekFromDayNumber(days);
			return (((int)dayOfWeek - (int)day - 7) % 7);
		}
	}
}