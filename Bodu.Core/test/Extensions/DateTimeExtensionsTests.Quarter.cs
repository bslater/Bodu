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
		private struct QuarterData
		{
			public DateTime Date { get;  }
			public CalendarQuarterDefinition Definition { get;  }
			public int Quarter { get;  }
			public DateTime StartDate { get;  }
			public DateTime EndDate { get;  }

			public QuarterData(DateTime date, CalendarQuarterDefinition definition, int quarter, DateTime startDate, DateTime endDate)
			{
				Date = date;
				Definition = definition;
				Quarter = quarter;
				StartDate = startDate;
				EndDate = endDate;
			}
		}


		private static readonly QuarterData[] QuarterTestData = new[]
		{
			// January�December
			new QuarterData(new DateTime(2024, 01, 01), CalendarQuarterDefinition.JanuaryDecember, 1, new DateTime(2024, 01, 01), new DateTime(2024, 03, 31)),
			new QuarterData(new DateTime(2024, 02, 01), CalendarQuarterDefinition.JanuaryDecember, 1, new DateTime(2024, 01, 01), new DateTime(2024, 03, 31)),
			new QuarterData(new DateTime(2024, 03, 01), CalendarQuarterDefinition.JanuaryDecember, 1, new DateTime(2024, 01, 01), new DateTime(2024, 03, 31)),
			new QuarterData(new DateTime(2024, 04, 01), CalendarQuarterDefinition.JanuaryDecember, 2, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 05, 01), CalendarQuarterDefinition.JanuaryDecember, 2, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 06, 01), CalendarQuarterDefinition.JanuaryDecember, 2, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 07, 01), CalendarQuarterDefinition.JanuaryDecember, 3, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 08, 01), CalendarQuarterDefinition.JanuaryDecember, 3, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 09, 01), CalendarQuarterDefinition.JanuaryDecember, 3, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 10, 01), CalendarQuarterDefinition.JanuaryDecember, 4, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.JanuaryDecember, 4, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 12, 01), CalendarQuarterDefinition.JanuaryDecember, 4, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),

			// July�June
			new QuarterData(new DateTime(2024, 07, 01), CalendarQuarterDefinition.JulyToJune, 1, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 08, 01), CalendarQuarterDefinition.JulyToJune, 1, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 09, 01), CalendarQuarterDefinition.JulyToJune, 1, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 10, 01), CalendarQuarterDefinition.JulyToJune, 2, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.JulyToJune, 2, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 12, 01), CalendarQuarterDefinition.JulyToJune, 2, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2025, 01, 01), CalendarQuarterDefinition.JulyToJune, 3, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 02, 01), CalendarQuarterDefinition.JulyToJune, 3, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 03, 01), CalendarQuarterDefinition.JulyToJune, 3, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 04, 01), CalendarQuarterDefinition.JulyToJune, 4, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),
			new QuarterData(new DateTime(2025, 05, 01), CalendarQuarterDefinition.JulyToJune, 4, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),
			new QuarterData(new DateTime(2025, 06, 01), CalendarQuarterDefinition.JulyToJune, 4, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),

			// April�March
			new QuarterData(new DateTime(2024, 04, 01), CalendarQuarterDefinition.AprilToMarch, 1, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 05, 01), CalendarQuarterDefinition.AprilToMarch, 1, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 06, 01), CalendarQuarterDefinition.AprilToMarch, 1, new DateTime(2024, 04, 01), new DateTime(2024, 06, 30)),
			new QuarterData(new DateTime(2024, 07, 01), CalendarQuarterDefinition.AprilToMarch, 2, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 08, 01), CalendarQuarterDefinition.AprilToMarch, 2, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 09, 01), CalendarQuarterDefinition.AprilToMarch, 2, new DateTime(2024, 07, 01), new DateTime(2024, 09, 30)),
			new QuarterData(new DateTime(2024, 10, 01), CalendarQuarterDefinition.AprilToMarch, 3, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.AprilToMarch, 3, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 12, 01), CalendarQuarterDefinition.AprilToMarch, 3, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2025, 01, 01), CalendarQuarterDefinition.AprilToMarch, 4, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 02, 01), CalendarQuarterDefinition.AprilToMarch, 4, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 03, 01), CalendarQuarterDefinition.AprilToMarch, 4, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),

			// October�September
			new QuarterData(new DateTime(2024, 10, 01), CalendarQuarterDefinition.OctoberToSeptember, 1, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.OctoberToSeptember, 1, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2024, 12, 01), CalendarQuarterDefinition.OctoberToSeptember, 1, new DateTime(2024, 10, 01), new DateTime(2024, 12, 31)),
			new QuarterData(new DateTime(2025, 01, 01), CalendarQuarterDefinition.OctoberToSeptember, 2, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 02, 01), CalendarQuarterDefinition.OctoberToSeptember, 2, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 03, 01), CalendarQuarterDefinition.OctoberToSeptember, 2, new DateTime(2025, 01, 01), new DateTime(2025, 03, 31)),
			new QuarterData(new DateTime(2025, 04, 01), CalendarQuarterDefinition.OctoberToSeptember, 3, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),
			new QuarterData(new DateTime(2025, 05, 01), CalendarQuarterDefinition.OctoberToSeptember, 3, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),
			new QuarterData(new DateTime(2025, 06, 01), CalendarQuarterDefinition.OctoberToSeptember, 3, new DateTime(2025, 04, 01), new DateTime(2025, 06, 30)),
			new QuarterData(new DateTime(2025, 07, 01), CalendarQuarterDefinition.OctoberToSeptember, 4, new DateTime(2025, 07, 01), new DateTime(2025, 09, 30)),
			new QuarterData(new DateTime(2025, 08, 01), CalendarQuarterDefinition.OctoberToSeptember, 4, new DateTime(2025, 07, 01), new DateTime(2025, 09, 30)),
			new QuarterData(new DateTime(2025, 09, 01), CalendarQuarterDefinition.OctoberToSeptember, 4, new DateTime(2025, 07, 01), new DateTime(2025, 09, 30)),

			// February�January
			new QuarterData(new DateTime(2024, 02, 01), CalendarQuarterDefinition.FebruaryJanuary, 1, new DateTime(2024, 02, 01), new DateTime(2024, 04, 30)),
			new QuarterData(new DateTime(2024, 03, 01), CalendarQuarterDefinition.FebruaryJanuary, 1, new DateTime(2024, 02, 01), new DateTime(2024, 04, 30)),
			new QuarterData(new DateTime(2024, 04, 01), CalendarQuarterDefinition.FebruaryJanuary, 1, new DateTime(2024, 02, 01), new DateTime(2024, 04, 30)),
			new QuarterData(new DateTime(2024, 05, 01), CalendarQuarterDefinition.FebruaryJanuary, 2, new DateTime(2024, 05, 01), new DateTime(2024, 07, 31)),
			new QuarterData(new DateTime(2024, 06, 01), CalendarQuarterDefinition.FebruaryJanuary, 2, new DateTime(2024, 05, 01), new DateTime(2024, 07, 31)),
			new QuarterData(new DateTime(2024, 07, 01), CalendarQuarterDefinition.FebruaryJanuary, 2, new DateTime(2024, 05, 01), new DateTime(2024, 07, 31)),
			new QuarterData(new DateTime(2024, 08, 01), CalendarQuarterDefinition.FebruaryJanuary, 3, new DateTime(2024, 08, 01), new DateTime(2024, 10, 31)),
			new QuarterData(new DateTime(2024, 09, 01), CalendarQuarterDefinition.FebruaryJanuary, 3, new DateTime(2024, 08, 01), new DateTime(2024, 10, 31)),
			new QuarterData(new DateTime(2024, 10, 01), CalendarQuarterDefinition.FebruaryJanuary, 3, new DateTime(2024, 08, 01), new DateTime(2024, 10, 31)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.FebruaryJanuary, 4, new DateTime(2024, 11, 01), new DateTime(2025, 01, 31)),
			new QuarterData(new DateTime(2024, 12, 01), CalendarQuarterDefinition.FebruaryJanuary, 4, new DateTime(2024, 11, 01), new DateTime(2025, 01, 31)),
			new QuarterData(new DateTime(2025, 01, 01), CalendarQuarterDefinition.FebruaryJanuary, 4, new DateTime(2024, 11, 01), new DateTime(2025, 01, 31)),

			// April 6 � April 5 (Fiscal 5-4-4)
			new QuarterData(new DateTime(2024, 04, 06), CalendarQuarterDefinition.April6ToApril5, 1, new DateTime(2024, 04, 06), new DateTime(2024, 07, 05)),
			new QuarterData(new DateTime(2024, 05, 06), CalendarQuarterDefinition.April6ToApril5, 1, new DateTime(2024, 04, 06), new DateTime(2024, 07, 05)),
			new QuarterData(new DateTime(2024, 06, 06), CalendarQuarterDefinition.April6ToApril5, 1, new DateTime(2024, 04, 06), new DateTime(2024, 07, 05)),
			new QuarterData(new DateTime(2024, 07, 06), CalendarQuarterDefinition.April6ToApril5, 2, new DateTime(2024, 07, 06), new DateTime(2024, 10, 05)),
			new QuarterData(new DateTime(2024, 08, 06), CalendarQuarterDefinition.April6ToApril5, 2, new DateTime(2024, 07, 06), new DateTime(2024, 10, 05)),
			new QuarterData(new DateTime(2024, 09, 06), CalendarQuarterDefinition.April6ToApril5, 2, new DateTime(2024, 07, 06), new DateTime(2024, 10, 05)),
			new QuarterData(new DateTime(2024, 10, 06), CalendarQuarterDefinition.April6ToApril5, 3, new DateTime(2024, 10, 06), new DateTime(2025, 01, 05)),
			new QuarterData(new DateTime(2024, 11, 01), CalendarQuarterDefinition.April6ToApril5, 3, new DateTime(2024, 10, 06), new DateTime(2025, 01, 05)),
			new QuarterData(new DateTime(2024, 12, 06), CalendarQuarterDefinition.April6ToApril5, 3, new DateTime(2024, 10, 06), new DateTime(2025, 01, 05)),
			new QuarterData(new DateTime(2025, 01, 06), CalendarQuarterDefinition.April6ToApril5, 4, new DateTime(2025, 01, 06), new DateTime(2025, 04, 05)),
			new QuarterData(new DateTime(2025, 02, 06), CalendarQuarterDefinition.April6ToApril5, 4, new DateTime(2025, 01, 06), new DateTime(2025, 04, 05)),
			new QuarterData(new DateTime(2025, 03, 06), CalendarQuarterDefinition.April6ToApril5, 4, new DateTime(2025, 01, 06), new DateTime(2025, 04, 05)),
			new QuarterData(new DateTime(2024, 07, 05), CalendarQuarterDefinition.April6ToApril5, 1, new DateTime(2024, 04, 06), new DateTime(2024, 07, 05)),
			new QuarterData(new DateTime(2024, 10, 05), CalendarQuarterDefinition.April6ToApril5, 2, new DateTime(2024, 07, 06), new DateTime(2024, 10, 05)),
			new QuarterData(new DateTime(2025, 01, 05), CalendarQuarterDefinition.April6ToApril5, 3, new DateTime(2024, 10, 06), new DateTime(2025, 01, 05)),
			new QuarterData(new DateTime(2025, 04, 05), CalendarQuarterDefinition.April6ToApril5, 4, new DateTime(2025, 01, 06), new DateTime(2025, 04, 05)),
		};

		public static IEnumerable<object[]> GetQuarterWithDefinitionTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Select(e => new object[] { e.Date, e.Definition, e.Quarter });

		[DataTestMethod]
		[DynamicData(nameof(GetQuarterWithDefinitionTestData), DynamicDataSourceType.Property)]
		public void GetQuarter_WhenUsingQuarterDefinition_ShouldReturnExpectedQuarter(DateTime input, CalendarQuarterDefinition definition, int expected)
		{
			int result = input.Quarter(definition);

			Assert.AreEqual(expected, result);
		}

		public static IEnumerable<object[]> GetQuarterTestData =>
			DateTimeExtensionsTests.QuarterTestData
				.Where(e => e.Definition == CalendarQuarterDefinition.JanuaryDecember)
				.Select(e => new object[] { e.Date, e.Quarter });

		[DataTestMethod]
		[DynamicData(nameof(GetQuarterTestData), typeof(DateTimeExtensionsTests))]
		public void GetQuarter_WhenOnlyDateTime_ShouldReturnExpectedQuarter(DateTime input, int expected)
		{
			int result = input.Quarter();

			Assert.AreEqual(expected, result);
		}

		public static IEnumerable<object[]> GetQuarterWithCustomProviderTestData =>
			DateTimeExtensionsTests.ValidQuarterProvider.QuarterTestData
				.Select(e => new object[] { e.Date, e.Quarter });

		[DataTestMethod]
		[DynamicData(nameof(GetQuarterWithCustomProviderTestData), DynamicDataSourceType.Property)]
		public void GetQuarter_WhenUsingValidProvider_ShouldReturnExpectedQuarter(DateTime input, int expected)
		{
			var provider = new DateTimeExtensionsTests.ValidQuarterProvider();
			int result = input.Quarter(provider);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void GetQuarter_WhenUsingInvalidProvider_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);
			var provider = new DateTimeExtensionsTests.InValidQuarterProvider();

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.Quarter(provider);
			});
		}

		[TestMethod]
		public void GetQuarter_WhenUsingCustomQuarterDefinitionWithoutProvider_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 20);

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = input.Quarter(CalendarQuarterDefinition.Custom);
			});
		}
	}
}