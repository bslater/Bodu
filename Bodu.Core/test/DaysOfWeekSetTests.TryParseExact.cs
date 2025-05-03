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
		[DataTestMethod]
		[DynamicData(nameof(GetTryParseExactTestData), typeof(DaysOfWeekSetTests))]
		public void TryParseExact_WhenGivenInputAndFormat_ShouldReturnExpectedResultAndParsedValue(string input, string format, byte expected, bool isValid)
		{
			bool success = DaysOfWeekSet.TryParseExact(input, format, out var result);
			Assert.AreEqual(isValid, success);

			if (success)
			{
				Assert.AreEqual(expected, result);
			}
			else
			{
				Assert.AreEqual(DaysOfWeekSet.Empty, result);
			}
		}
	}
}