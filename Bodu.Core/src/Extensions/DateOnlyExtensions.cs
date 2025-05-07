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
		/// <summary>
		/// Returns the tick count between the <paramref name="dateTime" /> and next <paramref name="dayOfWeek" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="System.DateTime" /> to which to get the next day of week.</param>
		/// <param name="dayOfWeek">An enumerated constant that indicates the next day of the week.</param>
		/// <returns>
		/// The number of ticks that represent the date and time difference between the specified <paramref name="dateTime" /> and the
		/// specified next <paramref name="dayOfWeek" />. The value is between <see cref="System.TimeSpan.MinValue" /> and <see cref="System.TimeSpan.MaxValue" />.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static int GetNextDayOfWeekOffset(DateOnly dateTime, DayOfWeek dayOfWeek)
			=> (((int)dayOfWeek - (int)dateTime.DayOfWeek + 7) % 7);
	}
}