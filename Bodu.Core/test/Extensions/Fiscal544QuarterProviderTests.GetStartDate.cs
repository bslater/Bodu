using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Extensions
{
	public partial class Fiscal544QuarterProviderTests
	{
		/// <summary>
		/// Verifies that GetStartDate(DateTime) returns the expected start of the fiscal quarter containing the given date.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetFiscal544QuarterTestData), DynamicDataSourceType.Method)]
		public void GetStartDate_WhenCalledWithDate_ShouldReturnExpectedStart(
			Fiscal544QuarterProvider provider,
			DateTime date,
			int _,
			DateTime expectedStart,
			DateTime __)
		{
			var result = provider.GetStartDate(date);
			Assert.AreEqual(expectedStart, result);
		}

		/// <summary>
		/// Verifies that GetStartDate(int) returns the expected date for the given quarter number.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetFiscal544QuarterTestData), DynamicDataSourceType.Method)]
		public void GetStartDate_WhenCalledWithQuarter_ShouldReturnExpectedStart(
			Fiscal544QuarterProvider provider,
			DateTime _,
			int quarter,
			DateTime expectedStart,
			DateTime __)
		{
			var result = provider.GetStartDate(quarter);
			Assert.AreEqual(expectedStart, result);
		}

		/// <summary>
		/// Verifies that calling GetStartDate with a date before the fiscal year throws ArgumentOutOfRangeException.
		/// </summary>
		[TestMethod]
		public void GetStartDate_WhenDateIsBeforeFiscalYear_ShouldThrowExactly()
		{
			var provider = new Fiscal544QuarterProvider(new DateTime(2023, 1, 29), DayOfWeek.Sunday);
			var invalidDate = new DateTime(2023, 1, 22); // One week before fiscal year

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetStartDate(invalidDate);
			});
		}

		/// <summary>
		/// Verifies that calling GetStartDate with a date after the fiscal year throws ArgumentOutOfRangeException.
		/// </summary>
		[TestMethod]
		public void GetStartDate_WhenDateIsBeyondFiscalYear_ShouldThrowExactly()
		{
			var provider = new Fiscal544QuarterProvider(new DateTime(2023, 1, 29), DayOfWeek.Sunday);

			// 2023 is not a 53-week year; Q4 ends on 2024-01-27
			var invalidDate = new DateTime(2024, 1, 28); // One day too far

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetStartDate(invalidDate);
			});
		}

		/// <summary>
		/// Verifies that calling GetStartDate with an invalid quarter throws ArgumentOutOfRangeException.
		/// </summary>
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(5)]
		public void GetStartDate_WhenQuarterIsInvalid_ShouldThrowExactly(int quarter)
		{
			var provider = SundayProvider;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				provider.GetStartDate(quarter);
			});
		}
	}
}