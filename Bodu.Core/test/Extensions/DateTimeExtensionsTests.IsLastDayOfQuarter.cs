﻿namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>false</c>
		/// when the input date is not the first day of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(IsNotLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenDateIsNotStartOfQuarterDefinition_ShouldReturnFalse(DateTime input, CalendarQuarterDefinition definition)
		{
			bool actual = input.IsLastDayOfQuarter(definition);
			Assert.IsFalse(actual);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfQuarter(DateTime)" /> returns <c>true</c> only when the date is the
		/// first day of a quarter based on the <see cref="CalendarQuarterDefinition.JanuaryToDecember" /> structure.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(IsLastDayOfQuarterJanuaryDecemberTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenDateIsQuarterStartAndDefaultDefinition_ShouldReturnTrue(DateTime input, bool expected)
		{
			bool actual = input.IsLastDayOfQuarter();

			Assert.AreEqual(actual, expected);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfQuarter(DateTime, CalendarQuarterDefinition)" /> returns <c>true</c> only
		/// when the input date equals the computed start of the quarter.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(IsLastDayOfQuarterTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenDateMatchesStartOfQuarterDefinition_ShouldReturnTrue(DateTime input, CalendarQuarterDefinition definition)
		{
			bool actual = input.IsLastDayOfQuarter(definition);

			Assert.IsTrue(actual);
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
		[DynamicData(nameof(DateTimeExtensionsTests.ValidQuarterProvider.IsLastDayOfQuarterTestData), typeof(DateTimeExtensionsTests.ValidQuarterProvider), DynamicDataSourceType.Method)]
		public void IsLastDayOfQuarter_WhenUsingValidQuarterProvider_ShouldReturnExpectedDate(DateTime input, bool expected)
		{
			var provider = new ValidQuarterProvider();
			var actual = input.IsLastDayOfQuarter(provider);

			Assert.AreEqual(expected, actual);
		}
	}
}