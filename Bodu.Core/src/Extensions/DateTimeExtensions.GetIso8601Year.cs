// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

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
		public static int GetIso8601Year(this DateTime date)
			=> ISOWeek.GetYear(date);
	}
}
