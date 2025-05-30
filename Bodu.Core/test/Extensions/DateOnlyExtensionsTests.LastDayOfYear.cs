// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{

		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.LastDayOfYearTestData),typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void LastDayOfYear_WhenCalled_ShouldReturnDecember31(DateTime inputDateOnly, DateTime expectedDateOnly)
		{
			DateOnly input = DateOnly.FromDateTime(inputDateOnly);
			DateOnly expected = DateOnly.FromDateTime(expectedDateOnly);

			DateOnly actual = input.LastDayOfYear();

			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void LastDayOfYear_WhenMinValue_ShouldReturnEndOfYear1()
		{
			DateOnly input = DateOnly.MinValue;
			DateOnly actual = input.LastDayOfYear();

			Assert.AreEqual(new DateOnly(DateTime.MinValue.Year, 12, 31), actual);
		}

		[TestMethod]
		public void LastDayOfYear_WhenMaxValue_ShouldReturnItself()
		{
			DateOnly input = DateOnly.MaxValue;
			DateOnly actual = input.LastDayOfYear();

			Assert.AreEqual(DateOnly.MaxValue, actual);
		}
	}
}
