using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{
		private static IEnumerable<object[]> GetIsLastDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => DateOnly.FromDateTime(e.Date) == DateOnly.FromDateTime(e.EndDate))
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		private static IEnumerable<object[]> GetIsLastDayOfQuarterJanuaryDecemberTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		/// <summary>
		/// Verifies that <see cref="DateOnlyExtensions.IsFirstDayOfQuarter(DateOnly)" /> returns <c>true</c> only when the date is the
		/// first day of a quarter based on the <see cref="CalendarQuarterDefinition.JanuaryDecember" /> structure.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsLastDayOfQuarterJanuaryDecemberTestData), DynamicDataSourceType.Method)]
		public void GetIsLastDayOfQuarter_WhenUsingJanuaryDecemberDefinition_ShouldMatchExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedDate)
		{
			var input = DateOnly.FromDateTime(inputDate);
			bool result = input.IsLastDayOfQuarter();

			if (inputDate == expectedDate)
			{
				Assert.IsTrue(result);
			}
			else
			{
				Assert.IsFalse(result);
			}
		}

		/// <summary>
		/// Verifies that <see cref="DateOnlyExtensions.IsLastDayOfQuarter(DateOnly, CalendarQuarterDefinition)" /> returns <c>true</c> only
		/// when the input date equals the computed start of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenComparedToExpectedStart_ShouldReturnExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedDate)
		{
			var input = DateOnly.FromDateTime(inputDate);
			bool result = input.IsLastDayOfQuarter(definition);

			Assert.IsTrue(result);
		}

		private static IEnumerable<object[]> GetNotLastDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => DateOnly.FromDateTime(e.Date) != DateOnly.FromDateTime(e.EndDate))
				.Select(e => new object[] { e.Date, e.Definition, e.EndDate });

		/// <summary>
		/// Verifies that <see cref="DateOnlyExtensions.IsLastDayOfQuarter(DateOnly, CalendarQuarterDefinition)" /> returns <c>false</c>
		/// when the input date is not the first day of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetNotLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenDateIsNotStartOfQuarter_ShouldReturnFalse(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedDate)
		{
			var input = DateOnly.FromDateTime(inputDate);
			bool result = input.IsLastDayOfQuarter(definition);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsLastDayOfQuarter_WhenDefinitionIsInvalid_ShouldThrowExactly()
		{
			var input = new DateOnly(2024, 4, 20);
			var definition = (CalendarQuarterDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.IsLastDayOfQuarter(definition);
			});
		}

		[TestMethod]
		public void IsLastDayOfQuarter_WhenDefinitionIsCustom_ShouldThrowExactly()
		{
			var input = new DateOnly(2024, 4, 20);

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
		public void IsLastDayOfQuarter_WhenUsingValidQuarterProvider_ShouldReturnExpectedDate(DateTime inputDate)
		{
			var input = DateOnly.FromDateTime(inputDate);
			var provider = new DateTimeExtensionsTests.ValidQuarterProvider();
			var result = input.IsLastDayOfQuarter(provider);
			Assert.IsTrue(result);
		}
	}
}