using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Extensions
{
	public partial class Fiscal544QuarterProviderTests
	{
		private static IEnumerable<object[]> GetIs53WeekYearTestData()
		{
			yield return new object[] { new DateTime(2023, 1, 29), DayOfWeek.Sunday, true };
			yield return new object[] { new DateTime(2024, 1, 28), DayOfWeek.Sunday, true };
			yield return new object[] { new DateTime(2028, 1, 30), DayOfWeek.Sunday, true };
			yield return new object[] { new DateTime(2030, 1, 27), DayOfWeek.Sunday, true };
			yield return new object[] { new DateTime(2035, 1, 28), DayOfWeek.Sunday, true };

			yield return new object[] { new DateTime(2023, 1, 30), DayOfWeek.Monday, true };
			yield return new object[] { new DateTime(2024, 1, 29), DayOfWeek.Monday, true };
			yield return new object[] { new DateTime(2025, 1, 27), DayOfWeek.Monday, true };
		}

		/// <summary>
		/// Verifies that Is53WeekFiscalYear returns the correct result for different fiscal year start dates and first day of week combinations.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIs53WeekYearTestData), DynamicDataSourceType.Method)]
		public void Is53WeekYear_WhenCalled_ShouldReturnExpected(
			DateTime fiscalYearStart,
			DayOfWeek firstDayOfWeek,
			bool expected)
		{
			var provider = new Fiscal544QuarterProvider(fiscalYearStart, firstDayOfWeek);
			var result = provider.Is53WeekFiscalYear();

			Assert.AreEqual(expected, result);
		}
	}
}