using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetAddTestCases), DynamicDataSourceType.Method)]
		public void Add_WhenValidInputsProvided_ShouldReturnExpectedResult(string inputDate, int years, int months, int days, string expectedDate)
		{
			var input = DateOnly.Parse(inputDate);
			var expected = DateOnly.Parse(expectedDate);
			var result = input.Add(years, months, days);
			Assert.AreEqual(expected, result);
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
			yield return new object[] { "0001-01-01", 0, 0, 0, "0001-01-01" };
			yield return new object[] { "9999-12-31", 0, 0, 0, "9999-12-31" };
			yield return new object[] { "2000-01-01", 1000, 0, 0, "3000-01-01" };
			yield return new object[] { "2000-01-01", -999, 0, 0, "1001-01-01" };
		}

		[DataTestMethod]
		[DynamicData(nameof(GetAddExceptionCases), DynamicDataSourceType.Method)]
		public void Add_WhenOutOfRange_ShouldThrowExactly(string inputDate, int years, int months, int days)
		{
			var input = DateOnly.Parse(inputDate);
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				input.Add(years, months, days);
			});
		}

		public static IEnumerable<object[]> GetAddExceptionCases()
		{
			yield return new object[] { DateOnly.MaxValue.ToString("yyyy-MM-dd"), 1, 0, 0 };
			yield return new object[] { DateOnly.MaxValue.ToString("yyyy-MM-dd"), 0, 1, 0 };
			yield return new object[] { DateOnly.MaxValue.ToString("yyyy-MM-dd"), 0, 0, 1 };
			yield return new object[] { DateOnly.MinValue.ToString("yyyy-MM-dd"), -1, 0, 0 };
			yield return new object[] { DateOnly.MinValue.ToString("yyyy-MM-dd"), 0, -1, 0 };
			yield return new object[] { DateOnly.MinValue.ToString("yyyy-MM-dd"), 0, 0, -1 };
		}

		[TestMethod]
		public void Add_WhenAllParametersZero_ShouldReturnSameDate()
		{
			var input = new DateOnly(2024, 1, 1);
			var result = input.Add(0, 0, 0);
			Assert.AreEqual(input, result);
		}

		[TestMethod]
		public void Add_WhenAddingToFeb28InLeapYear_ShouldReturnFeb29()
		{
			var input = new DateOnly(2024, 2, 28);
			var result = input.Add(0, 0, 1);
			Assert.AreEqual(new DateOnly(2024, 2, 29), result);
		}
	}
}