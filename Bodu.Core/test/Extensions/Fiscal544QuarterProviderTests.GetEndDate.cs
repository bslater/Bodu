using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Extensions
{
	public partial class Fiscal544QuarterProviderTests
	{
		/// <summary>
		/// Verifies that GetEndDate(DateTime) returns the expected end of the fiscal quarter containing the given date.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetFiscal544QuarterTestData), DynamicDataSourceType.Method)]
		public void GetEndDate_WhenCalledWithDate_ShouldReturnExpectedEnd(
			Fiscal544QuarterProvider provider,
			DateTime date,
			int _,
			DateTime __,
			DateTime expectedEnd)
		{
			var result = provider.GetEndDate(date);
			Assert.AreEqual(expectedEnd, result);
		}

		/// <summary>
		/// Verifies that GetEndDate(int) returns the expected end date for the given quarter number.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetFiscal544QuarterTestData), DynamicDataSourceType.Method)]
		public void GetEndDate_WhenCalledWithQuarter_ShouldReturnExpectedEnd(
			Fiscal544QuarterProvider provider,
			DateTime _,
			int quarter,
			DateTime __,
			DateTime expectedEnd)
		{
			var result = provider.GetEndDate(quarter);
			Assert.AreEqual(expectedEnd, result);
		}

		/// <summary>
		/// Verifies that calling GetEndDate with a date before the fiscal year throws ArgumentOutOfRangeException.
		/// </summary>
		[TestMethod]
		public void GetEndDate_WhenDateIsBeforeFiscalYear_ShouldThrowExactly()
		{
			var provider = new Fiscal544QuarterProvider(new DateTime(2023, 1, 29), DayOfWeek.Sunday);
			var invalidDate = new DateTime(2023, 1, 22); // One week before fiscal year

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetEndDate(invalidDate);
			});
		}

		/// <summary>
		/// Verifies that calling GetEndDate with a date beyond the fiscal year throws ArgumentOutOfRangeException.
		/// </summary>
		[TestMethod]
		public void GetEndDate_WhenDateIsBeyondFiscalYear_ShouldThrowExactly()
		{
			var provider = new Fiscal544QuarterProvider(new DateTime(2023, 1, 29), DayOfWeek.Sunday);

			// 2023 is not a 53-week year; Q4 ends on 2024-01-27
			var invalidDate = new DateTime(2024, 1, 28); // One day too far

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetEndDate(invalidDate);
			});
		}

		/// <summary>
		/// Verifies that calling GetEndDate with an invalid quarter throws ArgumentOutOfRangeException.
		/// </summary>
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(5)]
		public void GetEndDate_WhenQuarterIsInvalid_ShouldThrowExactly(int quarter)
		{
			var provider = SundayProvider;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetEndDate(quarter);
			});
		}
	}
}