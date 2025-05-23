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
		[DynamicData(nameof(DateTimeExtensionsTests.LeapYearTestData), typeof(DateTimeExtensionsTests),DynamicDataSourceType.Method)]
		public void IsLeapYear_WhenCalled_ShouldReturnExpected(int year, bool expected)
		{
			DateTime input = new DateTime(year, 1, 1);
			bool actual = input.IsLeapYear();

			Assert.AreEqual(expected, actual, $"Expected leap year check for {year} to be {expected}.");
		}
	}
}
