﻿// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DaysOfWeekSet.Operators.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Computes the bitwise complement, ensuring only valid days are toggled.
		/// </summary>
		/// <param name="obj">The <see cref="DaysOfWeekSet" /> to complement.</param>
		/// <returns>The complemented set with valid day bits.</returns>
		public static DaysOfWeekSet operator ~(DaysOfWeekSet obj) =>
			new(~obj.selectedDays & MaxValue);

		/// <summary>
		/// Implicitly converts a <see cref="DaysOfWeekSet" /> to a <see cref="byte" />.
		/// </summary>
		/// <param name="obj">The <see cref="DaysOfWeekSet" /> to convert.</param>
		/// <returns>A <see cref="byte" /> representing the selected days.</returns>
		public static implicit operator byte(DaysOfWeekSet obj) =>
			obj.selectedDays;

		/// <summary>
		/// Implicitly converts a <see cref="DaysOfWeekSet" /> to an <see cref="int" />.
		/// </summary>
		/// <param name="obj">The <see cref="DaysOfWeekSet" /> to convert.</param>
		/// <returns>An <see cref="int" /> representing the selected days.</returns>
		public static implicit operator int(DaysOfWeekSet obj) =>
			obj.selectedDays;

		/// <summary>
		/// Implicitly converts a <see cref="DaysOfWeekSet" /> to a <see cref="short" />.
		/// </summary>
		/// <param name="obj">The <see cref="DaysOfWeekSet" /> to convert.</param>
		/// <returns>A <see cref="short" /> representing the selected days.</returns>
		public static implicit operator short(DaysOfWeekSet obj) =>
			obj.selectedDays;

		/// <summary>
		/// Determines whether two specified <see cref="DaysOfWeekSet" /> instances have the same value.
		/// </summary>
		/// <param name="obj1">The first instance.</param>
		/// <param name="obj2">The second instance.</param>
		/// <returns><c>true</c> if both instances are equal; otherwise, <c>false</c>.</returns>
		public static bool operator ==(DaysOfWeekSet obj1, DaysOfWeekSet obj2) =>
			obj1.selectedDays == obj2.selectedDays;

		/// <summary>
		/// Determines whether two specified <see cref="DaysOfWeekSet" /> instances have different values.
		/// </summary>
		/// <param name="obj1">The first instance.</param>
		/// <param name="obj2">The second instance.</param>
		/// <returns><c>true</c> if both instances are not equal; otherwise, <c>false</c>.</returns>
		public static bool operator !=(DaysOfWeekSet obj1, DaysOfWeekSet obj2) =>
			obj1.selectedDays != obj2.selectedDays;

		/// <summary>
		/// Performs a bitwise AND operation between two <see cref="DaysOfWeekSet" /> instances.
		/// </summary>
		/// <param name="obj1">The first operand.</param>
		/// <param name="obj2">The second operand.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> containing the result.</returns>
		public static DaysOfWeekSet operator &(DaysOfWeekSet obj1, DaysOfWeekSet obj2) =>
			new(obj1.selectedDays & obj2.selectedDays);

		/// <summary>
		/// Performs a bitwise OR operation between two <see cref="DaysOfWeekSet" /> instances.
		/// </summary>
		/// <param name="obj1">The first operand.</param>
		/// <param name="obj2">The second operand.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> containing the result.</returns>
		public static DaysOfWeekSet operator |(DaysOfWeekSet obj1, DaysOfWeekSet obj2) =>
			new(obj1.selectedDays | obj2.selectedDays);

		/// <summary>
		/// Performs a bitwise XOR operation between two <see cref="DaysOfWeekSet" /> instances.
		/// </summary>
		/// <param name="obj1">The first operand.</param>
		/// <param name="obj2">The second operand.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> containing the result.</returns>
		public static DaysOfWeekSet operator ^(DaysOfWeekSet obj1, DaysOfWeekSet obj2) =>
			new(obj1.selectedDays ^ obj2.selectedDays);
	}
}