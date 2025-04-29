// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Formatting.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using System.Text;

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the default format.
		/// </summary>
		/// <returns>A string representing the selected days in Sunday-to-Saturday order, with underscore ('_') for unselected days.</returns>
		public override string ToString() => this.ToString("S", null!);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the specified format.
		/// </summary>
		/// <param name="format">
		/// A single-character format specifier that determines the ordering and output style:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'S'</c> or <c>'s'</c> — Sunday-to-Saturday ordering using weekday symbols (default).</description>
		/// </item>
		/// <item>
		/// <description><c>'M'</c> or <c>'m'</c> — Monday-to-Sunday ordering using weekday symbols.</description>
		/// </item>
		/// <item>
		/// <description><c>'B'</c> or <c>'b'</c> — Binary format; '1' for selected days, '0' for unselected days.</description>
		/// </item>
		/// </list>
		/// </param>
		/// <returns>A formatted string representing the selected days.</returns>
		/// <exception cref="ArgumentException">Thrown if the <paramref name="format" /> is invalid.</exception>
		public string ToString(string format) => this.ToString(format, null!);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the default format and the specified
		/// culture-specific formatting information.
		/// </summary>
		/// <param name="provider">An <see cref="IFormatProvider" /> supplying culture-specific formatting information (currently ignored).</param>
		/// <returns>A string representing the selected days in Sunday-to-Saturday order.</returns>
		public string ToString(IFormatProvider provider) => this.ToString("S", provider);

		/// <summary>
		/// Returns a string representation of the current <see cref="DaysOfWeekSet" /> using the specified format and culture-specific
		/// formatting information.
		/// </summary>
		/// <param name="format">
		/// A format string that defines the day ordering and symbol used for unselected days. Supported formats are:
		/// <list type="bullet">
		/// <item>
		/// <description>Single character:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'S'</c> or <c>'s'</c> — Sunday-to-Saturday, using underscore ('_') for unselected days (default).</description>
		/// </item>
		/// <item>
		/// <description><c>'M'</c> or <c>'m'</c> — Monday-to-Sunday, using underscore ('_') for unselected days.</description>
		/// </item>
		/// <item>
		/// <description><c>'B'</c> or <c>'b'</c> — Binary format; '1' for selected, '0' for unselected days.</description>
		/// </item>
		/// </list>
		/// </description>
		/// </item>
		/// <item>
		/// <description>Two characters:
		/// <list type="bullet">
		/// <item>
		/// <description>
		/// The first character must be <c>'S'</c> or <c>'M'</c> (case-insensitive), indicating Sunday-first or Monday-first ordering.
		/// </description>
		/// </item>
		/// <item>
		/// <description>The second character specifies the symbol for unselected days:
		/// <list type="bullet">
		/// <item>
		/// <description><c>'U'</c> — Underscore ('_')</description>
		/// </item>
		/// <item>
		/// <description><c>'D'</c> — Dash ('-')</description>
		/// </item>
		/// <item>
		/// <description><c>'A'</c> — Asterisk ('*')</description>
		/// </item>
		/// <item>
		/// <description><c>'B'</c> — Space (' ')</description>
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
			var (startDay, unselectedChar, isBinary) = ParseFormatString(format ?? "S", true);

			bool isMondayStart = startDay == 'M';
			var buffer = new char[7];

			for (int i = 0; i < 7; i++)
			{
				int dayIndex = isMondayStart ? (i + 1) % 7 : i;
				bool selected = this[dayIndex];

				buffer[i] = selected
					? (isBinary ? '1' : WeekdaySymbols[dayIndex])
					: (isBinary ? '0' : unselectedChar);
			}

			return new string(buffer);
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