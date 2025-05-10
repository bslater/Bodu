using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetAddTestCases), DynamicDataSourceType.Method)]
		public void Add_WhenValidInputsProvided_ShouldReturnExpectedResult(string inputDate, int years, int months, double days, string expectedStr)
		{
			DateTime input = DateTime.Parse(inputDate);
			DateTime expected = DateTime.Parse(expectedStr);
			DateTime actual = input.Add(years, months, days);
			Assert.AreEqual(expected, actual);
		}

		public static IEnumerable<object[]> GetAddTestCases()
		{
			yield return new object[] { "2024-01-01", 1, 0, 0, "2025-01-01" };
			yield return new object[] { "2024-01-01", 0, 1, 0, "2024-02-01" };
			yield return new object[] { "2024-01-01", 0, 0, 1, "2024-01-02" };
			yield return new object[] { "2024-01-01", -1, 0, 0, "2023-01-01" };
			yield return new object[] { "2024-01-01", 0, -1, 0, "2023-12-01" };
			yield return new object[] { "2024-01-01", 0, 0, -1, "2023-12-31" };
			yield return new object[] { "2024-01-31", 0, 1, 0, "2024-02-29" };
			yield return new object[] { "2023-01-31", 0, 1, 0, "2023-02-28" };
			yield return new object[] { "2024-02-29", 1, 0, 0, "2025-02-28" };
			yield return new object[] { "2025-05-01", -1, -2, -1, "2024-02-29" };
			yield return new object[] { "2024-01-01", 0, 0, 1.5, "2024-01-02T12:00:00" };
			yield return new object[] { "2024-01-01", 0, 0, 0.25, "2024-01-01T06:00:00" };
			yield return new object[] { "0001-01-01", 0, 0, 0, "0001-01-01" };
			yield return new object[] { "9999-12-31", 0, 0, 0, "9999-12-31" };
			yield return new object[] { "2024-01-01T00:00:00", 0, 0, 1.0 / 86400000.0, "2024-01-01T00:00:00.001" };
			yield return new object[] { "2024-01-01T00:00:00", 0, 0, 1.0 / 86400000, "2024-01-01T00:00:00.001" };
		}

		[DataTestMethod]
		[DynamicData(nameof(GetAddExceptionCases), DynamicDataSourceType.Method)]
		public void Add_WhenOutOfRange_ShouldThrowArgumentOutOfRangeException(DateTime input, int years, int months, double days)
		{
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => input.Add(years, months, days));
		}

		public static IEnumerable<object[]> GetAddExceptionCases()
		{
			yield return new object[] { DateTime.MaxValue.AddDays(-1), 0, 0, 2 };
			yield return new object[] { DateTime.MaxValue, 1, 0, 0 };
			yield return new object[] { DateTime.MaxValue, 0, 1, 0 };
			yield return new object[] { DateTime.MinValue, -1, 0, 0 };
		}

		[TestMethod]
		public void Add_WhenAllParametersZero_ShouldReturnSameDate()
		{
			DateTime input = new DateTime(2024, 1, 1, 12, 0, 0);
			Assert.AreEqual(input, input.Add(0, 0, 0));
		}

		[TestMethod]
		public void Add_WhenAddingNegativeFractionalDay_ShouldSubtractAccurately()
		{
			DateTime input = new DateTime(2024, 1, 2, 12, 0, 0);
			DateTime expected = new DateTime(2024, 1, 2, 6, 0, 0);
			Assert.AreEqual(expected, input.Add(0, 0, -0.25));
		}

		[TestMethod]
		public void Add_WhenDaysIsLessThanEpsilon_ShouldBeIgnored()
		{
			DateTime input = new DateTime(2024, 1, 1, 0, 0, 0);
			DateTime result = input.Add(0, 0, 1e-12); // Below epsilon
			Assert.AreEqual(input, result);
		}

		[TestMethod]
		public void Add_WhenAddingToFeb28LeapYear_ShouldIncludeFeb29()
		{
			DateTime input = new DateTime(2024, 2, 28);
			DateTime result = input.Add(0, 0, 1);
			Assert.AreEqual(new DateTime(2024, 2, 29), result);
		}

		[DataTestMethod]
		[DataRow("2024-03-10T01:30:00", 0, 0, 1.0 / 24, "2024-03-10T03:30:00", "Pacific Standard Time")]
		[DataRow("2024-11-03T01:30:00", 0, 0, 1.0, "2024-11-04T01:30:00", "Pacific Standard Time")]
		public void Add_WhenInDstTransitionZone_ShouldRespectTimezone(string inputDate, int years, int months, double days, string expectedDate, string timeZoneId)
		{
			var tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
			DateTime input = DateTime.Parse(inputDate, null, DateTimeStyles.AssumeLocal);
			var unspecified = DateTime.SpecifyKind(input, DateTimeKind.Unspecified);
			var expected = DateTime.Parse(expectedDate);

			var utc = TimeZoneInfo.ConvertTimeToUtc(unspecified, tz);
			var resultUtc = utc.AddYears(years).AddMonths(months).AddDays(days);
			var resultLocal = TimeZoneInfo.ConvertTimeFromUtc(resultUtc, tz);

			Assert.AreEqual(expected, resultLocal);
		}
	}
}