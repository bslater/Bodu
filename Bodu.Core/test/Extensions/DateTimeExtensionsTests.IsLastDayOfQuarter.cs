using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		private static IEnumerable<object[]> GetIsLastDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Date == e.EndDate)
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		private static IEnumerable<object[]> GetIsLastDayOfQuarterJanuaryDecemberTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfQuarter(DateTime)" /> returns <c>true</c> only when the date is the
		/// first day of a quarter based on the <see cref="CalendarQuarterDefinition.JanuaryDecember" /> structure.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsLastDayOfQuarterJanuaryDecemberTestData), DynamicDataSourceType.Method)]
		public void GetIsLastDayOfQuarter_WhenUsingJanuaryDecemberDefinition_ShouldMatchExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedEnd)
		{
			bool result = inputDate.IsLastDayOfQuarter();

			if (inputDate == expectedEnd)
			{
				Assert.IsTrue(result);
			}
			else
			{
				Assert.IsFalse(result);
			}
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>true</c> only
		/// when the input date equals the computed start of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenComparedToExpectedStart_ShouldReturnExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedEnd)
		{
			bool result = inputDate.IsLastDayOfQuarter(definition);

			Assert.IsTrue(result);
		}

		private static IEnumerable<object[]> GetNotLastDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Date != e.EndDate)
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>false</c>
		/// when the input date is not the first day of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetNotLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenDateIsNotStartOfQuarter_ShouldReturnFalse(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedEnd)
		{
			bool result = inputDate.IsLastDayOfQuarter(definition);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsLastDayOfQuarter_WhenDefinitionIsInvalid_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);
			var definition = (CalendarQuarterDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.IsLastDayOfQuarter(definition);
			});
		}

		[TestMethod]
		public void IsLastDayOfQuarter_WhenDefinitionIsCustom_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = input.IsLastDayOfQuarter(CalendarQuarterDefinition.Custom);
			});
		}

		public static IEnumerable<object[]> GetIsLastDayOfQuarterWithCustomProviderTestData =>
			DateTimeExtensionsTests.ValidQuarterProvider.QuarterTestData
				.Where(e => e.Date == e.StartDate)
				.Select(e => new object[] { e.EndDate });

		[TestMethod]
		[DynamicData(nameof(GetIsLastDayOfQuarterWithCustomProviderTestData), DynamicDataSourceType.Property)]
		public void IsLastDayOfQuarter_WhenUsingValidQuarterProvider_ShouldReturnExpectedDate(DateTime input)
		{
			var provider = new ValidQuarterProvider();
			var result = input.IsLastDayOfQuarter(provider);
			Assert.IsTrue(result);
		}
	}
}