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
		[DynamicData(nameof(DateTimeExtensionsTests.FirstDayOfMonthDataTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void FirstDayOfMonth_WhenCalled_ShouldReturnExpectedDate(DateTime inputDateTime, DateTime expectedDateTime)
		{
			var input = DateOnly.FromDateTime(inputDateTime);
			var expected = DateOnly.FromDateTime(expectedDateTime);
			var actual = input.FirstDayOfMonth();

			Assert.AreEqual(expected, actual);
		}
	}
}
