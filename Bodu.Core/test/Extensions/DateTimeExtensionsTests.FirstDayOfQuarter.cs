// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{

		public static IEnumerable<object[]> GetFirstDayOfQuarterWithDefinitionTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Select(e => new object[] { e.Date, e.Definition,e.StartDate });

		[DataTestMethod]
		[DynamicData(nameof(GetFirstDayOfQuarterWithDefinitionTestData), typeof(DateTimeExtensionsTests))]
		public void FirstDayOfQuarter_WhenUsingQuarterDefinition_ShouldReturnExpectedDate(DateTime input, CalendarQuarterDefinition definition, DateTime expected)
		{
			var result = input.FirstDayOfQuarter(definition);
			Assert.AreEqual(expected, result);
		}

		public static IEnumerable<object[]> GetFirstDayOfQuarterTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.StartDate });

		[DataTestMethod]
		[DynamicData(nameof(GetFirstDayOfQuarterTestData), typeof(DateTimeExtensionsTests))]
		public void FirstDayOfQuarter_WhenUsingDateOnly_ShouldReturnExpectedStartOfCalendarQuarter(DateTime input,DateTime expected)
		{
			var result = input.FirstDayOfQuarter();
			Assert.AreEqual(expected, result);
		}

		public static IEnumerable<object[]> GetFirstDayOfQuarterWithQuarterAndDefinitionTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Select(e => new object[] { e.Date, e.Definition, e.Quarter,e.StartDate });

		[DataTestMethod]
		[DynamicData(nameof(GetFirstDayOfQuarterWithQuarterAndDefinitionTestData), typeof(DateTimeExtensionsTests))]
		public void FirstDayOfQuarter_WhenUsingQuarterAndDefinition_ShouldReturnExpectedDate(DateTime date, CalendarQuarterDefinition definition, int quarter, DateTime expected)
		{
			int year = expected.AddMonths(-((quarter - 1) * 3)).Year;
			var result = DateTimeExtensions.FirstDayOfQuarter(definition, quarter, year);
			Assert.AreEqual(expected, result);
		}

		public static IEnumerable<object[]> GetFirstDayOfQuarterWithQuarterTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.Quarter,e.StartDate });

		[DataTestMethod]
		[DynamicData(nameof(GetFirstDayOfQuarterWithQuarterTestData), typeof(DateTimeExtensionsTests))]
		public void FirstDayOfQuarter_WhenUsingQuarterAndCalendarDefinition_ShouldReturnExpectedDate(DateTime date, int quarter, DateTime expected)
		{
			int year = expected.AddMonths(-((quarter - 1) * 3)).Year;
			var result = DateTimeExtensions.FirstDayOfQuarter(quarter, year);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenUsingInvalidProvider_ShouldThrowArgumentOutOfRange()
		{
			var input = new DateTime(2024, 4, 20);
			var provider = new InValidQuarterProvider();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.FirstDayOfQuarter(provider);
			});
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenUsingCustomQuarterDefinitionWithoutProvider_ShouldThrowInvalidOperation()
		{
			var input = new DateTime(2024, 4, 20);

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = input.FirstDayOfQuarter(CalendarQuarterDefinition.Custom);
			});
		}

		public static IEnumerable<object[]> GetFirstDayOfQuarterWithCustomProviderTestData =>
			DateTimeExtensionsTests.ValidQuarterProvider.QuarterTestData
				.Select(e => new object[] { e.Date, e.Quarter, e.StartDate });

		/// <summary>
		/// Verifies that FirstDayOfQuarter returns the correct date using a valid quarter provider.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(GetFirstDayOfQuarterWithCustomProviderTestData), DynamicDataSourceType.Property)]
		public void FirstDayOfQuarter_WhenUsingValidQuarterProvider_ShouldReturnExpectedDate(DateTime input, int quarter, DateTime expectedFirstDay)
		{
			var provider = new ValidQuarterProvider();
			var actual = input.FirstDayOfQuarter(provider);
			Assert.AreEqual(expectedFirstDay, actual, $"Expected first day of Q{quarter} for {input:yyyy-MM-dd} to be {expectedFirstDay:yyyy-MM-dd}, but got {actual:yyyy-MM-dd}.");
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenInputIsMinValue_ShouldReturnExpectedDate()
		{
			var result = DateTime.MinValue.FirstDayOfQuarter(CalendarQuarterDefinition.JanuaryDecember);
			Assert.AreEqual(new DateTime(1, 1, 1), result);
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenInputIsMaxValue_ShouldReturnExpectedDate()
		{
			var result = DateTime.MaxValue.FirstDayOfQuarter(CalendarQuarterDefinition.JanuaryDecember);
			Assert.AreEqual(new DateTime(9999, 10, 1), result); // Q4 of 9999
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenDefinitionIsInvalid_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);
			var definition = (CalendarQuarterDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.FirstDayOfQuarter(definition);
			});
		}

		[TestMethod]
		public void FirstDayOfQuarter_WhenDefinitionIsInvalidAndQuarterIsValid_ShouldThrowExactly()
		{
			var definition = (CalendarQuarterDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = DateTimeExtensions.FirstDayOfQuarter(definition, 1, 2025);
			});
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		[DataRow(5)]
		public void FirstDayOfQuarter_WhenQuarterIsOutOfRangeAndDefinitionIsValid_ShouldThrowExactly(int quarter)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = DateTimeExtensions.FirstDayOfQuarter(CalendarQuarterDefinition.JanuaryDecember, quarter, 2025);
			});
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(0)]
		[DataRow(5)]
		public void FirstDayOfQuarter_WhenQuarterIsOutOfRange_ShouldThrowExactly(int quarter)
		{
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = DateTimeExtensions.FirstDayOfQuarter(quarter, 2025);
			});
		}


	}
}