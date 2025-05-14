// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateOnlyExtensions.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

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
		internal static int GetDayNumber(DateTime dateTime) =>
			(int)((ulong)dateTime.Ticks / DateTimeExtensions.TicksPerDay);

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
		/// Returns the tick count that represents the date nearest to the specified <paramref name="dayOfWeek" /> relative to the provided
		/// <paramref name="dayNumber" /> value.
		/// </summary>
		internal static int GetNearestDayOfWeek(int dayNumber, DayOfWeek dayOfWeek)
		{
			int delta = ((int)dayOfWeek - (int)GetDayOfWeekFromDayNumber(dayNumber) + 7) % 7;
			return dayNumber + (delta > 3 ? delta - 7 : delta);
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

		/// <summary>
		/// Calculates the day number of the first occurrence of the specified <see cref="DayOfWeek" /> within a given month and year,
		/// without creating a <see cref="DateOnly" /> instance.
		/// </summary>
		/// <param name="year">The calendar year.</param>
		/// <param name="month">The calendar month (1 through 12).</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate (e.g., <see cref="DayOfWeek.Monday" /> for the first Monday of the month).
		/// </param>
		/// <returns>
		/// An integer representing the day number (days since 0001-01-01) of the first occurrence of <paramref name="dayOfWeek" /> in the
		/// specified month and year.
		/// </returns>
		/// <remarks>This method performs date arithmetic directly and avoids constructing <see cref="DateOnly" /> objects for performance.</remarks>
		internal static int GetFirstDayOfWeekInMonthDayNumber(int year, int month, DayOfWeek dayOfWeek) =>
			GetDayNumber(year, month, 1)
				+ (((int)dayOfWeek - (int)GetDayOfWeekFromDayNumber(GetDayNumber(year, month, 1)) + 7) % 7);

		/// <summary>
		/// Calculates the day number of the last occurrence of the specified <see cref="DayOfWeek" /> within a given month and year,
		/// without creating a <see cref="DateOnly" /> instance.
		/// </summary>
		/// <param name="year">The calendar year.</param>
		/// <param name="month">The calendar month (1 through 12).</param>
		/// <param name="dayOfWeek">
		/// The <see cref="DayOfWeek" /> to locate (e.g., <see cref="DayOfWeek.Friday" /> for the last Friday of the month).
		/// </param>
		/// <returns>
		/// An integer representing the day number (days since 0001-01-01) of the last occurrence of <paramref name="dayOfWeek" /> in the
		/// specified month and year.
		/// </returns>
		/// <remarks>This method performs date arithmetic directly and avoids constructing <see cref="DateOnly" /> objects for performance.</remarks>
		internal static int GetLastDayOfWeekInMonthDayNumber(int year, int month, DayOfWeek dayOfWeek) =>
			GetDayNumber(year, month, DateTime.DaysInMonth(year, month))
				- (((int)GetDayOfWeekFromDayNumber(GetDayNumber(year, month, DateTime.DaysInMonth(year, month))) - (int)dayOfWeek + 7) % 7);
	}
}