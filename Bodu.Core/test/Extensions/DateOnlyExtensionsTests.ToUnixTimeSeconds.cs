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
		[DynamicData(nameof(DateTimeExtensionsTests.FromUnixTimeSecondsTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Property)]
		public void ToUnixTimeSeconds_WhenUtcInput_ShouldReturnExpected(long expected, DateTime inputDateTime)
		{
			DateOnly input = DateOnly.FromDateTime(inputDateTime);
			long remainder = expected % 86400L;
			long flooredExpected = expected >= 0
				? expected - remainder                                      // floor to 00:00 UTC
				: expected - remainder - (remainder != 0 ? 86400L : 0);		// ceil toward earlier day

			long actual = input.ToUnixTimeSeconds();

			Assert.AreEqual(flooredExpected, actual);
		}

		[TestMethod]
		public void ToUnixTimeSeconds_WhenUsingMinValue_ShouldBeNegativeLarge()
		{
			long actual = DateOnly.MinValue.ToUnixTimeSeconds();

			Assert.IsTrue(actual < 0);
		}

		[TestMethod]
		public void ToUnixTimeSeconds_WhenUsingMaxValue_ShouldBePositiveLarge()
		{
			long actual = DateOnly.MaxValue.ToUnixTimeSeconds();

			Assert.IsTrue(actual > 0);
		}

		[TestMethod]
		public void ToUnixTimeSeconds_RoundTripWithFromUnixTimeSeconds_ShouldMatchUtc()
		{
			DateOnly input = new DateOnly(2024, 4, 18);
			long seconds = input.ToUnixTimeSeconds();

			DateOnly roundTrip = DateOnlyExtensions.FromUnixTimeSeconds(seconds);

			Assert.AreEqual(input, roundTrip);
		}
	}
}
