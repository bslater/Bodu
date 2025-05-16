// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="EndOfDay.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a new <see cref="DateTime" /> representing the final possible tick of the same day as the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value whose date component is used to calculate the end of day.</param>
		/// <returns>
		/// A <see cref="DateTime" /> set to 23:59:59.9999999 on the same calendar day as <paramref name="dateTime" />, with the original
		/// <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// <para>
		/// The result represents the last representable moment of the specified day, just before midnight of the following day, with a time
		/// component of 23:59:59.9999999 (the maximum value supported by <see cref="DateTime" />).
		/// </para>
		/// <para>
		/// The date component is preserved, and the <see cref="DateTime.Kind" /> (e.g., <see cref="DateTimeKind.Utc" /> or
		/// <see cref="DateTimeKind.Local" />) is copied from the input.
		/// </para>
		/// </remarks>
		public static DateTime EndOfDay(this DateTime dateTime)
			=> new(GetDateTicks(dateTime) + (TicksPerDay - 1), dateTime.Kind);
	}
}