// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Parsing.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Converts the string representation of selected days into a corresponding <see cref="DaysOfWeekSet" /> instance, automatically
		/// inferring the format if none is specified.
		/// </summary>
		/// <param name="input">The input string that represents selected days.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> instance corresponding to the selected days.</returns>
		/// <exception cref="FormatException">
		/// Thrown if the <paramref name="input" /> is <c>null</c>, incorrectly formatted, or cannot be parsed.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If the input string does not clearly indicate whether the week starts on Sunday or Monday, the parser will attempt to infer the
		/// starting day based on the logical alignment of selected days.
		/// </para>
		/// <para>
		/// For example, if the input string is <c>"______S"</c>, it could plausibly represent either: Sunday selected (Sunday-first,
		/// <c>0b1000000</c>), or Saturday selected (Monday-first, <c>0b0000001</c>). In such ambiguous cases, the parser defaults to
		/// interpreting the input as Sunday-first.
		/// </para>
		/// <para>
		/// To guarantee correct parsing without relying on inference, use <see cref="ParseExact(string, string)" /> with an explicit format specifier.
		/// </para>
		/// </remarks>
		public static DaysOfWeekSet Parse(string input) =>
			ParseCore(input, formatInfo: null);

		/// <summary>
		/// Converts the string representation of selected days into a <see cref="DaysOfWeekSet" /> instance using a specified format.
		/// </summary>
		/// <param name="input">The input string that represents selected days.</param>
		/// <param name="format">The format specifier that defines how the input string is interpreted.</param>
		/// <returns>A new <see cref="DaysOfWeekSet" /> instance parsed according to the specified format.</returns>
		/// <exception cref="ArgumentNullException">Thrown if the <paramref name="format" /> is <c>null</c>.</exception>
		/// <exception cref="FormatException">
		/// Thrown if the <paramref name="input" /> or <paramref name="format" /> are invalid or cannot be parsed.
		/// </exception>
		public static DaysOfWeekSet ParseExact(string input, string format)
		{
			ThrowHelper.ThrowIfNull(format);

			return ParseCore(input, ParseFormatString(format, false));
		}

		/// <summary>
		/// Attempts to parse the string representation of selected days into a <see cref="DaysOfWeekSet" />, automatically inferring the
		/// format if none is specified.
		/// </summary>
		/// <param name="input">The input string that represents selected days.</param>
		/// <param name="result">
		/// When this method returns, contains the parsed <see cref="DaysOfWeekSet" /> if parsing succeeded; otherwise, contains <see cref="DaysOfWeekSet.Empty" />.
		/// </param>
		/// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
		public static bool TryParse(string input, out DaysOfWeekSet result)
		{
			try
			{
				result = ParseCore(input, formatInfo: null);
				return true;
			}
			catch
			{
				result = DaysOfWeekSet.Empty;
				return false;
			}
		}

		/// <summary>
		/// Attempts to parse the string representation of selected days into a <see cref="DaysOfWeekSet" /> using a specified format.
		/// </summary>
		/// <param name="input">The input string that represents selected days.</param>
		/// <param name="format">The format specifier that defines how the input string is interpreted.</param>
		/// <param name="result">
		/// When this method returns, contains the parsed <see cref="DaysOfWeekSet" /> if parsing succeeded; otherwise, contains <see cref="DaysOfWeekSet.Empty" />.
		/// </param>
		/// <returns><c>true</c> if parsing succeeded; otherwise, <c>false</c>.</returns>
		public static bool TryParseExact(string input, string format, out DaysOfWeekSet result)
		{
			try
			{
				result = ParseCore(input, ParseFormatString(format, false));
				return true;
			}
			catch
			{
				result = DaysOfWeekSet.Empty;
				return false;
			}
		}
	}
}