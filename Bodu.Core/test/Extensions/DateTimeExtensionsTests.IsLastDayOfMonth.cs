﻿namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfMonth(DateTime)" /> returns <c>true</c> when the date represents the last
		/// day of the month.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.IsLastDayOfMonthDataTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfMonth_WhenDateIsLastDay_ShouldReturnTrue(DateTime input)
		{
			var actual = input.IsLastDayOfMonth();

			Assert.IsTrue(actual);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfMonth(DateTime)" /> returns <c>false</c> when the date does not represent
		/// the last day of the month.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.IsNotLastDayOfMonthTestData), DynamicDataSourceType.Method)]
		public void IsLastDayOfMonth_WhenDateIsNotLastDay_ShouldReturnFalse(DateTime input)
		{
			var actual = input.IsLastDayOfMonth();

			Assert.IsFalse(actual);
		}
	}
}