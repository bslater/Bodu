using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	public partial class DaysOfWeekSetTests
	{
		/// <summary>
		/// Verifies that <see cref="DaysOfWeekSet.TryParseExact(string, string, out DaysOfWeekSet)" /> returns the expected success flag
		/// and value for various valid and invalid combinations of input and format.
		/// </summary>
		/// <param name="input">The input string to parse.</param>
		/// <param name="format">The format specifier string.</param>
		/// <param name="expectedSuccess">Whether parsing is expected to succeed.</param>
		/// <param name="expected">The expected bitmask result if successful.</param>
		[DataTestMethod]
		[DataRow("SMTWTFS", "S", true, (byte)0b1111111)]   // Valid Sunday-first
		[DataRow("MTWTFSS", "M", true, (byte)0b1111111)]   // Valid Monday-first
		[DataRow("1010101", "B", true, (byte)0b1010101)]   // Valid binary
		[DataRow("_______", "S", true, (byte)0b0000000)]   // All unselected
		[DataRow("M_WTF_S", "MU", true, (byte)0b1101110)]  // Monday format, explicit underscore
		[DataRow("M-WTF-S", "MD", true, (byte)0b1101110)]  // Monday format, explicit dash
		[DataRow("       ", "MB", true, (byte)0b0000000)]  // All unselected (space)
		[DataRow("SMTWT", "S", false, (byte)0)]            // Too short
		[DataRow("XXXXXXX", "S", false, (byte)0)]          // Bad letters
		[DataRow("101X101", "B", false, (byte)0)]          // Invalid binary character
		[DataRow(null, "S", false, (byte)0)]               // Null input
		[DataRow("SMTWTFS", null, false, (byte)0)]         // Null format (should throw in ParseExact, but TryParseExact returns false)
		[DataRow("SMTWTFS", "", false, (byte)0)]           // Empty format string (invalid)
		[DataRow("SMTWTFS", "X", false, (byte)0)]          // Invalid format character
		[DataRow("SMTWTFS", "SZ", false, (byte)0)]         // Unsupported unselected symbol
		[DataRow("SMTWTFS", "SUU", false, (byte)0)]        // Too long format string
		public void TryParseExact_WhenGivenInputAndFormat_ShouldReturnExpectedResultAndParsedValue(
			string input,
			string format,
			bool expectedSuccess,
			byte expected)
		{
			bool success = DaysOfWeekSet.TryParseExact(input, format, out var result);
			Assert.AreEqual(expectedSuccess, success);

			if (success)
			{
				Assert.AreEqual(expected, (byte)result);
			}
			else
			{
				Assert.AreEqual(DaysOfWeekSet.Empty, result);
			}
		}
	}
}