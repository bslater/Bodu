// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Equals.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Determines whether the specified <see cref="object" /> is equal to the current <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <param name="obj">The object to compare with the current instance.</param>
		/// <returns><c>true</c> if the specified object is equal to this instance; otherwise, <c>false</c>.</returns>
		public override bool Equals(object? obj) =>
			obj is DaysOfWeekSet other && selectedDays == other.selectedDays;

		/// <summary>
		/// Determines whether the specified <see cref="DaysOfWeekSet" /> is equal to the current <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <param name="other">The <see cref="DaysOfWeekSet" /> to compare with this instance.</param>
		/// <returns><c>true</c> if the specified <see cref="DaysOfWeekSet" /> has the same selected days; otherwise, <c>false</c>.</returns>
		public bool Equals(DaysOfWeekSet other) =>
			selectedDays == other.selectedDays;

		/// <summary>
		/// Determines whether the specified <see cref="byte" /> value is equal to the current <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <param name="other">The <see cref="byte" /> value to compare with this instance.</param>
		/// <returns><c>true</c> if the bit pattern matches; otherwise, <c>false</c>.</returns>
		public bool Equals(byte other) =>
			selectedDays == other;

		/// <summary>
		/// Returns the hash code for this instance.
		/// </summary>
		/// <returns>A 32-bit signed integer hash code representing the current <see cref="DaysOfWeekSet" />.</returns>
		public override int GetHashCode() =>
			selectedDays.GetHashCode();
	}
}