// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="DaysOfWeekSet.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu
{
	/// <summary>
	/// Represents a set of selected days in a standard seven-day week.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="DaysOfWeekSet" /> provides a compact bitmask representation of selected days, supporting parsing, formatting, comparison,
	/// and serialization.
	/// </para>
	/// <code language="csharp">
	///<![CDATA[
	/// // Create a set with Monday, Wednesday, and Friday selected
	/// var days = new DaysOfWeekSet(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday);
	///
	/// // Check if Friday is selected:
	/// bool isFridayIncluded = days[DayOfWeek.Friday]; // true
	///
	/// // Add Sunday to the set
	/// days[DayOfWeek.Sunday] = true;
	///
	/// // Count the number of selected days
	/// int selectedCount = days.Count; //  4
	///]]>
	/// </code>
	/// </remarks>
	[Serializable]
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Represents a <see cref="DaysOfWeekSet" /> with no selected days.
		/// </summary>
		public static readonly DaysOfWeekSet Empty = default;

		/// <summary>
		/// Represents a <see cref="DaysOfWeekSet" /> with Monday to Friday selected.
		/// </summary>
		public static readonly DaysOfWeekSet Weekdays = new(DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday);

		/// <summary>
		/// Represents a <see cref="DaysOfWeekSet" /> with Saturday and Sunday selected.
		/// </summary>
		public static readonly DaysOfWeekSet Weekend = new(DayOfWeek.Saturday, DayOfWeek.Sunday);

		private const int MaxValue = 0b1111111;
		private const int MinValue = 0b0000000;
		private const byte ShiftValue = 0x01;

		private static readonly char[] WeekdaySymbols = { 'S', 'M', 'T', 'W', 'T', 'F', 'S' };

		private byte selectedDays;

		/// <summary>
		/// Initializes a new instance of the <see cref="DaysOfWeekSet" /> structure with the specified selected days.
		/// </summary>
		/// <param name="daysOfWeek">An array of <see cref="DayOfWeek" /> values to mark as selected.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if any provided day is outside the valid <see cref="DayOfWeek" /> range.
		/// </exception>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		/// // Create a set with Monday, Wednesday, and Friday marked as selected
		/// var workdays = new DaysOfWeekSet(DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday);
		///
		/// // Check if a specific day is selected
		/// bool isFridaySelected = workdays[DayOfWeek.Friday]; // true
		///
		/// // Get the total number of selected days
		/// int count = workdays.Count; // 3
		///]]>
		/// </code>
		/// </example>
		public DaysOfWeekSet(params DayOfWeek[] daysOfWeek)
		{
			selectedDays = 0;
			if (daysOfWeek != null)
			{
				foreach (var day in daysOfWeek)
					this[day] = true;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DaysOfWeekSet" /> struct by parsing the specified string.
		/// </summary>
		/// <param name="input">The input string representing selected days.</param>
		/// <exception cref="FormatException">
		/// Thrown if the <paramref name="input" /> is <see langword="null" />, not exactly 7 characters long, or contains invalid characters.
		/// </exception>
		/// <remarks>
		/// This constructor behaves the same as <see cref="Parse(string)" />, automatically inferring the format from the input string.
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		/// // Parse a string where 'M', 'T', 'W', 'T', 'F' represent selected days, '-' means unselected
		/// var weekdays = new DaysOfWeekSet("MTWTF--");
		///
		/// // Check if Thursday is selected
		/// bool thursdayIncluded = weekdays[DayOfWeek.Thursday]; // true
		///
		/// // Check if Sunday is selected
		/// bool sundayIncluded = weekdays[DayOfWeek.Sunday]; // false
		///
		/// // Count the number of selected days
		/// int count = weekdays.Count; // 5
		///]]>
		/// </code>
		/// </example>
		public DaysOfWeekSet(string input)
			: this(Parse(input)) // delegates to the implicit copy constructor from static method
		{ }

		private DaysOfWeekSet(int value)
		{
			ThrowHelper.ThrowIfOutOfRange(value, MinValue, MaxValue);

			selectedDays = (byte)value;
		}

		/// <summary>
		/// Creates a <see cref="DaysOfWeekSet" /> from a raw <see cref="byte" /> value representing selected days.
		/// </summary>
		/// <param name="value">The byte value to interpret as selected days.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> instance.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the value exceeds the valid mask (0–127).</exception>
		public static DaysOfWeekSet FromByte(byte value) =>
			value <= MaxValue ? new DaysOfWeekSet(value) : throw new ArgumentOutOfRangeException(nameof(value));

		/// <summary>
		/// Gets or sets whether the specified <see cref="DayOfWeek" /> is selected.
		/// </summary>
		/// <param name="dayOfWeek">The day to get or set.</param>
		/// <returns><c>true</c> if selected; otherwise, <c>false</c>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="dayOfWeek" /> is not a valid day (0–6).</exception>
		public bool this[DayOfWeek dayOfWeek]
		{
			get
			{
				ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

				return this[(int)dayOfWeek];
			}

			set
			{
				ThrowHelper.ThrowIfEnumValueIsUndefined(dayOfWeek);

				this[(int)dayOfWeek] = value;
			}
		}

		private bool this[int dayOfWeek]
		{
			get => (selectedDays >> (6 - dayOfWeek) & 1) == 1;

			set
			{
				int realBit = 6 - dayOfWeek;
				byte mask = (byte)(ShiftValue << realBit);
				selectedDays = value
					? (byte)(selectedDays | mask)
					: (byte)(selectedDays & ~mask);
			}
		}

		/// <summary>
		/// Clears all selected days.
		/// </summary>
		public void Clear() => selectedDays = 0;

		/// <summary>
		/// Gets the number of days currently selected.
		/// </summary>
		public int Count => ((selectedDays >> 0) & 1)
						  + ((selectedDays >> 1) & 1)
						  + ((selectedDays >> 2) & 1)
						  + ((selectedDays >> 3) & 1)
						  + ((selectedDays >> 4) & 1)
						  + ((selectedDays >> 5) & 1)
						  + ((selectedDays >> 6) & 1);
	}
}