// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Formatting.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the default format.
		/// </summary>
		/// <returns>
		/// A string representing the selected days in Sunday-to-Saturday order, using underscore ( <c>'_'</c>) for unselected days.
		/// </returns>
		public override string ToString() => ToString("S", null!);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the specified format.
		/// </summary>
		/// <param name="format">
		/// A format string that defines the day ordering and symbol used for unselected days. Supported formats include:
		/// <list type="bullet">
		/// <item>
		/// <description>
		/// <c>'S'</c> or <c>'s'</c> — Sunday-to-Saturday using weekday symbols; unselected days shown as underscore ( <c>'_'</c>).
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// <c>'M'</c> or <c>'m'</c> — Monday-to-Sunday using weekday symbols; unselected days shown as underscore ( <c>'_'</c>).
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// <c>'E'</c>, <c>'U'</c>, <c>'D'</c>, or <c>'A'</c> — Sunday-to-Saturday using weekday symbols with a specific symbol for
		/// unselected days:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'E'</c> — space ( <c>' '</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'U'</c> — underscore ( <c>'_'</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'D'</c> — dash ( <c>'-'</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'A'</c> — asterisk ( <c>'*'</c>)</description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// <c>'0'</c> or <c>'1'</c> — Binary representation; <c>'1'</c> for selected and <c>'0'</c> for unselected days.
		/// </description>
		/// </item>
		/// <item>
		/// <description>Two-character format:
		/// <list type="bullet">
		/// <item>
		/// <description>First character: <c>'S'</c> or <c>'M'</c> to indicate Sunday- or Monday-first ordering.</description>
		/// </item>
		/// <item>
		/// <description>
		/// Second character: <c>'E'</c>, <c>'U'</c>, <c>'D'</c>, or <c>'A'</c> to specify the symbol for unselected days.
		/// </description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>A formatted string representing the selected days.</returns>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="format" /> is invalid.</exception>
		public string ToString(string format) =>
			ToString(format, null!);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the default format and the specified
		/// culture-specific formatting information.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider" /> supplying culture-specific formatting information (currently ignored).</param>
		/// <returns>
		/// A string representing the selected days in Sunday-to-Saturday order using underscore ( <c>'_'</c>) for unselected days.
		/// </returns>
		public string ToString(IFormatProvider provider) =>
			ToString("S", provider);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the specified format and culture-specific
		/// formatting information.
		/// </summary>
		/// <param name="format">
		/// A format string that defines the day ordering and symbol used for unselected days. Supported formats include:
		/// <list type="bullet">
		/// <item>
		/// <description>Single-character formats:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'S'</c> — Sunday-first, unselected = underscore ( <c>'_'</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'M'</c> — Monday-first, unselected = underscore ( <c>'_'</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'E'</c> — Sunday-first, unselected = space ( <c>' '</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'U'</c> — Sunday-first, unselected = underscore ( <c>'_'</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'D'</c> — Sunday-first, unselected = dash ( <c>'-'</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'A'</c> — Sunday-first, unselected = asterisk ( <c>'*'</c>).</description>
		/// </item>
		/// <item>
		/// <description><c>'0'</c> or <c>'1'</c> — Binary; <c>'1'</c> = selected, <c>'0'</c> = unselected.</description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// <item>
		/// <description>Two-character formats:
		/// <list type="bullet">
		/// <item>
		/// <description>First character: <c>'S'</c> or <c>'M'</c> — day ordering.</description>
		/// </item>
		/// <item>
		/// <description>Second character: <c>'E'</c>, <c>'U'</c>, <c>'D'</c>, or <c>'A'</c> — unselected day symbol:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'E'</c> — space ( <c>' '</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'U'</c> — underscore ( <c>'_'</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'D'</c> — dash ( <c>'-'</c>)</description>
		/// </item>
		/// <item>
		/// <description><c>'A'</c> — asterisk ( <c>'*'</c>)</description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// </list>
		/// </param>
		/// <param name="provider">An <see cref="IFormatProvider" /> supplying culture-specific formatting information (currently ignored).</param>
		/// <returns>A formatted string representing the selected days.</returns>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="format" /> is invalid.</exception>
		public string ToString(string format, IFormatProvider provider)
		{
			var (startDay, unselectedChar, isBinary) = ParseFormatForToString(format);
			unselectedChar ??= '_'; // Default to underscore if not specified
			bool isMondayStart = startDay == 'M';
			var buffer = new char[7];

			for (int i = 0; i < 7; i++)
			{
				int dayIndex = isMondayStart ? (i + 1) % 7 : i;
				bool selected = this[dayIndex];

				buffer[i] = selected
					? (isBinary ? '1' : WeekdaySymbols[dayIndex])
					: (isBinary ? '0' : unselectedChar.Value);
			}

			return new string(buffer);
		}

		/// <summary>
		/// Parses the format string for use in <see cref="ToString(string, IFormatProvider)" />, throwing <see cref="ArgumentException" />
		/// if invalid.
		/// </summary>
		private static (char? startDay, char? unselectedChar, bool isBinary) ParseFormatForToString(string format)
		{
			format ??= "S";
			if (!TryParseFormatInfo(format, out var info))
				throw new ArgumentException(ResourceStrings.Arg_Invalid_FormatString, nameof(format));

			return info;
		}

		//private static (char startDay, char unselectedChar, bool isBinary) ParseFormatString(string format, bool throwAsArgumentException)
		//{
		//	static Exception CreateException(bool throwArg, string paramName)
		//		=> throwArg ? new ArgumentException(ResourceStrings.Arg_Invalid_FormatString, paramName)
		//					: new FormatException(ResourceStrings.Arg_Invalid_FormatString);

		// throw new Exception("NEED to UPDATE THIS TO HANDLE WHEN FORMAT IS NULL BUT WE WANT IT TO DECTECT FORMAT AUTOMATICALLY WHEN
		// PARSING"); if (string.IsNullOrEmpty(format)) return ('S', '_', false);

		// format = format.ToUpperInvariant();

		// if (format == "B") return ('S', '0', true);

		// if (format == "S") return ('S', '_', false);

		// if (format == "M") return ('M', '_', false);

		// if (format.Length == 2) { char startDay = format[0]; char unselectedSpecifier = format[1];

		// if (startDay is not ('S' or 'M')) throw CreateException(throwAsArgumentException, nameof(format));

		// char unselectedChar = unselectedSpecifier switch { 'U' => '_', 'D' => '-', 'A' => '*', 'B' => ' ', _ => throw
		// CreateException(throwAsArgumentException, nameof(format)) };

		// return (startDay, unselectedChar, false); }

		//	throw CreateException(throwAsArgumentException, nameof(format));
		//}
	}
}