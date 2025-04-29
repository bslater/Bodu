﻿// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Comparable.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
		: System.IComparable<DaysOfWeekSet>
		, System.IComparable<byte>
		, System.IComparable
	{
		/// <summary>
		/// Compares this instance to a specified <see cref="object" /> and returns an indication of their relative values.
		/// </summary>
		/// <param name="obj">An object to compare, expected to be a <see cref="DaysOfWeekSet" /> or <see cref="byte" />.</param>
		/// <returns>
		/// A signed integer that indicates the relative order of the objects being compared:
		/// <list type="bullet">
		/// <item>
		/// <description>Less than zero: This instance is less than <paramref name="obj" />.</description>
		/// </item>
		/// <item>
		/// <description>Zero: This instance is equal to <paramref name="obj" />.</description>
		/// </item>
		/// <item>
		/// <description>
		/// Greater than zero: This instance is greater than <paramref name="obj" />, or <paramref name="obj" /> is <c>null</c>.
		/// </description>
		/// </item>
		/// </list>
		/// </returns>
		/// <exception cref="ArgumentException">Thrown if <paramref name="obj" /> is not a <see cref="DaysOfWeekSet" /> or <see cref="byte" />.</exception>
		public int CompareTo(object? obj) =>
			 ReferenceEquals(obj, null) ? 1 :
			 obj is DaysOfWeekSet otherSet ? CompareTo(otherSet) :
			 obj is byte otherByte ? CompareTo(otherByte) :
			 throw new ArgumentException($"Object must be of type {nameof(DaysOfWeekSet)} or {nameof(Byte)}.", nameof(obj));

		/// <summary>
		/// Compares this instance to a specified <see cref="DaysOfWeekSet" /> and returns an indication of their relative values.
		/// </summary>
		/// <param name="other">A <see cref="DaysOfWeekSet" /> to compare with this instance.</param>
		/// <returns>
		/// A signed integer that indicates the relative order:
		/// <list type="bullet">
		/// <item>
		/// <description>Less than zero: This instance is less than <paramref name="other" />.</description>
		/// </item>
		/// <item>
		/// <description>Zero: This instance is equal to <paramref name="other" />.</description>
		/// </item>
		/// <item>
		/// <description>Greater than zero: This instance is greater than <paramref name="other" />.</description>
		/// </item>
		/// </list>
		/// </returns>
		public int CompareTo(DaysOfWeekSet other) => this.selectedDays.CompareTo(other.selectedDays);

		/// <summary>
		/// Compares this instance to a specified <see cref="byte" /> and returns an indication of their relative values.
		/// </summary>
		/// <param name="other">A <see cref="byte" /> value representing selected days to compare with this instance.</param>
		/// <returns>
		/// A signed integer that indicates the relative order:
		/// <list type="bullet">
		/// <item>
		/// <description>Less than zero: This instance is less than <paramref name="other" />.</description>
		/// </item>
		/// <item>
		/// <description>Zero: This instance is equal to <paramref name="other" />.</description>
		/// </item>
		/// <item>
		/// <description>Greater than zero: This instance is greater than <paramref name="other" />.</description>
		/// </item>
		/// </list>
		/// </returns>
		public int CompareTo(byte other) => this.selectedDays.CompareTo(other);
	}
}