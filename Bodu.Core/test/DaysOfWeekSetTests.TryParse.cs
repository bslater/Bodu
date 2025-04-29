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
		/// Verifies that <see cref="DaysOfWeekSet.TryParse(string, out DaysOfWeekSet)" /> returns the expected success flag and parsed
		/// value for valid and invalid inputs using auto-detected format.
		/// </summary>
		/// <param name="input">The input string to parse.</param>
		/// <param name="expectedSuccess">Whether the operation is expected to succeed.</param>
		/// <param name="expected">The expected <see cref="DaysOfWeekSet" /> bitmask if successful.</param>
		[DataTestMethod]
		[DataRow("SMTWTFS", true, (byte)0b1111111)]        // Valid Sunday-first
		[DataRow("_______", true, (byte)0b0000000)]        // All unselected
		[DataRow("1010101", true, (byte)0b1010101)]        // Binary, and auto-detect should assume binary in this context
		[DataRow("", false, (byte)0)]                      // Too short
		[DataRow("Invalid", false, (byte)0)]               // Invalid letters
		[DataRow(null, false, (byte)0)]                    // Null input
		[DataRow("MTWTFSS", true, (byte)0b1111111)]        // Valid Monday-first input
		[DataRow("M_WTF_S", true, (byte)0b1101110)]        // Monday-first with unselected symbols (auto-detected)
		[DataRow("M*WTF*S", true, (byte)0b1101110)]        // Alternative unselected symbol
		public void TryParse_WhenGivenInput_ShouldReturnExpectedResultAndParsedValue(string input, bool expectedSuccess, byte expected)
		{
			bool success = DaysOfWeekSet.TryParse(input, out var result);
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