// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="GetIso8601Year.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

using System.Globalization;
using System.Runtime.CompilerServices;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the ISO 8601 year associated with the specified <see cref="DateTime" />.
		/// </summary>
		/// <param name="date">The <see cref="DateTime" /> to evaluate.</param>
		/// <returns>The ISO 8601 calendar year that contains the date’s week.</returns>
		/// <remarks>ISO weeks may belong to the previous or next calendar year depending on where the week falls.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetIsoYear(this DateTime date)
		{
#if NETSTANDARD2_0

			// ISO 8601 rule: Week 1 is the week with Jan 4 in it. So we shift the date to Thursday (which is always in the same ISO week)
			DayOfWeek day = date.DayOfWeek;
			int delta = DayOfWeek.Thursday - day;
			DateTime adjusted = date.AddDays(delta);

			return adjusted.Year;
#else
			return ISOWeek.GetYear(date);
#endif
		}
	}
}