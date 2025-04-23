// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnly.Elapsed.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateOnlyExtensions
	{
		/// <summary>
		/// Returns the amount of time elapsed between now and the specified <see cref="DateOnly" />.
		/// </summary>
		/// <param name="dateTime">The past <see cref="DateOnly" />.</param>
		/// <returns>A <see cref="TimeSpan" /> representing the elapsed time.</returns>
		/// <remarks>
		/// This method returns a negative <see cref="TimeSpan" /> if the date is in the future.
		/// </remarks>
		public static TimeSpan Elapsed(this DateOnly date)
			=> DateOnly.Now - dateTime;
	}
}