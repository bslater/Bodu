// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Comparable.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Core parsing logic that interprets a string into a <see cref="DaysOfWeekSet" />, supporting auto-detection or strict format parsing.
		/// </summary>
		/// <param name="input">
		/// The 7-character input string representing selected days. Each character indicates whether a day is selected or not, based on the
		/// provided or inferred format.
		/// </param>
		/// <param name="formatInfo">
		/// An optional format specification that controls parsing behavior:
		/// <list type="bullet">
		/// <item>
		/// <description><c>startDay</c> — Specifies the first day of the week: 'S' for Sunday or 'M' for Monday.</description>
		/// </item>
		/// <item>
		/// <description><c>unselectedChar</c> — The character symbolizing an unselected day (e.g., '_', '-', '*', or space).</description>
		/// </item>
		/// <item>
		/// <description><c>isBinary</c> — Indicates whether the input uses binary format ("0" for unselected, "1" for selected).</description>
		/// </item>
		/// </list>
		/// If <paramref name="formatInfo" /> is <c>null</c>, the method attempts to auto-detect the format from the input string.
		/// </param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> instance representing the parsed days.</returns>
		/// <exception cref="FormatException">
		/// Thrown if the <paramref name="input" /> is <c>null</c>, not exactly 7 characters long, contains invalid symbols, or day
		/// alignment cannot be determined.
		/// </exception>
		private static DaysOfWeekSet ParseCore(string input, (char startDay, char unselectedChar, bool isBinary)? formatInfo)
		{
			ThrowHelper.ThrowIfNull(input);
			if (input.Length != 7)
				throw new FormatException(string.Format(ResourceStrings.Format_Invalid_StringLength, 7));

			DaysOfWeekSet temp = Empty;

			bool isBinary;
			bool? isMondayStart = null;
			char? unselectedChar = null;

			if (formatInfo is null)
			{
				// Auto-detect binary based on first character
				char firstChar = input[0];
				isBinary = firstChar == '0' || firstChar == '1';
			}
			else
			{
				(char startDay, char unselChar, bool binary) = formatInfo.Value;
				isBinary = binary;
				isMondayStart = startDay == 'M';
				unselectedChar = unselChar;
			}

			for (int i = 0; i < 7; i++)
			{
				char c = input[i];

				if (isBinary)
				{
					temp[i] = c switch
					{
						'0' => false,
						'1' => true,
						_ => throw new FormatException($"Invalid binary character '{c}' at position {i + 1}.")
					};
				}
				else
				{
					// Initialize unselected character if unknown
					if (unselectedChar is null && (c == ' ' || c == '-' || c == '*' || c == '_'))
						unselectedChar = c;

					// Still inferring Monday start if necessary
					if (isMondayStart is null)
					{
						char normalized = char.ToUpperInvariant(c);
						if (normalized == WeekdaySymbols[i] || i == 6) // if last day, default to Sunday start
							isMondayStart = false;
						else if (normalized == WeekdaySymbols[(i + 1) % 7])
							isMondayStart = true;
					}

					int dayIndex = isMondayStart == true ? (i + 1) % 7 : i;

					char normalizedDay = char.ToUpperInvariant(c);
					if (normalizedDay == WeekdaySymbols[dayIndex])
					{
						temp[dayIndex] = true;
					}
					else if (c == unselectedChar)
					{
						temp[dayIndex] = false;
					}
					else
					{
						throw new FormatException($"Unexpected character '{c}' at position {i + 1}.");
					}
				}
			}

			return temp;
		}

		/// <summary>
		/// Parses the provided format string into its constituent parts for interpreting or formatting a <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <param name="format">
		/// The format string specifying parsing rules. It must be either one or two characters:
		/// <list type="bullet">
		/// <item>
		/// <description><c>"S"</c> — Sunday-to-Saturday ordering; unselected days use underscore ('_').</description>
		/// </item>
		/// <item>
		/// <description><c>"M"</c> — Monday-to-Sunday ordering; unselected days use underscore ('_').</description>
		/// </item>
		/// <item>
		/// <description><c>"B"</c> — Binary format; '1' for selected days, '0' for unselected days.</description>
		/// </item>
		/// <item>
		/// <description><c>"SU"</c>, <c>"SD"</c>, <c>"SA"</c>, <c>"SB"</c> — Sunday-first ordering with explicit unselected character:
		/// <list type="bullet">
		/// <item>
		/// <description><c>U</c> — Underscore ('_')</description>
		/// </item>
		/// <item>
		/// <description><c>D</c> — Dash ('-')</description>
		/// </item>
		/// <item>
		/// <description><c>A</c> — Asterisk ('*')</description>
		/// </item>
		/// <item>
		/// <description><c>B</c> — Space (' ')</description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// <item>
		/// <description>
		/// <c>"MU"</c>, <c>"MD"</c>, <c>"MA"</c>, <c>"MB"</c> — Monday-first ordering with explicit unselected character (same mapping as above).
		/// </description>
		/// </item>
		/// </list>
		/// </param>
		/// <param name="throwAsArgumentException">
		/// If <c>true</c>, an <see cref="ArgumentException" /> is thrown on invalid format; otherwise, a <see cref="FormatException" /> is thrown.
		/// </param>
		/// <returns>
		/// A tuple containing:
		/// <list type="bullet">
		/// <item>
		/// <description>The start day ('S' for Sunday or 'M' for Monday).</description>
		/// </item>
		/// <item>
		/// <description>The character representing an unselected day.</description>
		/// </item>
		/// <item>
		/// <description>A flag indicating whether binary formatting is active.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <exception cref="ArgumentException">
		/// Thrown if the <paramref name="format" /> is invalid and <paramref name="throwAsArgumentException" /> is <c>true</c>.
		/// </exception>
		/// <exception cref="FormatException">
		/// Thrown if the <paramref name="format" /> is invalid and <paramref name="throwAsArgumentException" /> is <c>false</c>.
		/// </exception>
		private static (char startDay, char unselectedChar, bool isBinary) ParseFormatString(string format, bool throwAsArgumentException)
		{
			static Exception CreateFormatException(bool throwArg, string paramName)
				=> throwArg ? new ArgumentException(ResourceStrings.Format_Invalid_String, paramName) : new FormatException(ResourceStrings.Format_Invalid_String);

			if (string.IsNullOrEmpty(format))
				throw CreateFormatException(throwAsArgumentException, nameof(format));

			format = format.ToUpperInvariant();

			if (format == "B")
				return ('B', '0', true);

			if (format == "S")
				return ('S', '_', false);

			if (format == "M")
				return ('M', '_', false);

			if (format.Length == 2)
			{
				char startDay = format[0];
				char unselectedSpecifier = format[1];

				if (startDay is not ('S' or 'M'))
					throw CreateFormatException(throwAsArgumentException, nameof(format));

				char unselectedChar = unselectedSpecifier switch
				{
					'U' => '_',
					'D' => '-',
					'A' => '*',
					'B' => ' ',
					_ => throw CreateFormatException(throwAsArgumentException, nameof(format))
				};

				return (startDay, unselectedChar, false);
			}

			throw CreateFormatException(throwAsArgumentException, nameof(format));
		}
	}
}