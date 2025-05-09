using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		private static IEnumerable<object[]> GetIsFirstDayOfQuarterJanuaryDecemberTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.Definition, e.StartDate });

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfQuarter(DateTime)" /> returns <c>true</c> only when the date is the
		/// first day of a quarter based on the <see cref="CalendarQuarterDefinition.JanuaryDecember" /> structure.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsFirstDayOfQuarterJanuaryDecemberTestData), DynamicDataSourceType.Method)]
		public void IsFirstDayOfQuarter_WhenUsingJanuaryDecemberDefinition_ShouldMatchExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedStart)
		{
			bool result = inputDate.IsFirstDayOfQuarter();

			if (inputDate == expectedStart)
			{
				Assert.IsTrue(result);
			}
			else
			{
				Assert.IsFalse(result);
			}
		}

		private static IEnumerable<object[]> GetIsFirstDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Date == e.StartDate)
				.Select(e => new object[] { e.Date, e.Definition, e.StartDate });

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>true</c>
		/// only when the input date equals the computed start of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsFirstDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsFirstDayOfQuarter_WhenComparedToExpectedStart_ShouldReturnExpectedResult(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedStart)
		{
			bool result = inputDate.IsFirstDayOfQuarter(definition);

			Assert.IsTrue(result);
		}

		private static IEnumerable<object[]> GetNotFirstDayOfQuarterTestData() =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Date != e.StartDate)
				.Select(e => new object[] { e.Date, e.Definition, e.StartDate });

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>false</c>
		/// when the input date is not the first day of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetNotFirstDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsFirstDayOfQuarter_WhenDateIsNotStartOfQuarter_ShouldReturnFalse(DateTime inputDate, CalendarQuarterDefinition definition, DateTime expectedStart)
		{
			bool result = inputDate.IsFirstDayOfQuarter(definition);
			Assert.IsFalse(result);
		}

		[TestMethod]
		public void IsFirstDayOfQuarter_WhenDefinitionIsInvalid_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);
			var definition = (CalendarQuarterDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.IsFirstDayOfQuarter(definition);
			});
		}

		[TestMethod]
		public void IsFirstDayOfQuarter_WhenDefinitionIsCustom_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = input.IsFirstDayOfQuarter(CalendarQuarterDefinition.Custom);
			});
		}

		public static IEnumerable<object[]> GetIsFirstDayOfQuarterWithCustomProviderTestData =>
			DateTimeExtensionsTests.ValidQuarterProvider.QuarterTestData
				.Where(e => e.Date == e.StartDate)
				.Select(e => new object[] { e.Date });

		[TestMethod]
		[DynamicData(nameof(GetIsFirstDayOfQuarterWithCustomProviderTestData), DynamicDataSourceType.Property)]
		public void IsFirstDayOfQuarter_WhenUsingValidQuarterProvider_ShouldReturnExpectedDate(DateTime input)
		{
			var provider = new ValidQuarterProvider();
			var result = input.IsFirstDayOfQuarter(provider);
			Assert.IsTrue(result);
		}
	}
}