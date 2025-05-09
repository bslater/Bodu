// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{

		[DataTestMethod]
		[DataRow("2024-04-18", "2024-01-01")]
		[DataRow("2024-01-01", "2024-01-01")] // Already Jan 1
		[DataRow("2023-12-31", "2023-01-01")] // End of year
		[DataRow("2020-02-29", "2020-01-01")] // Leap year
		public void FirstDayOfYear_WhenCalled_ShouldReturnExpectedStartOfYear(string inputDate, string expectedDate)
		{
			DateTime input = DateTime.Parse(inputDate);
			DateTime expected = DateTime.Parse(expectedDate);
			DateTime result = input.FirstDayOfYear();

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void FirstDayOfYear_ShouldPreserveDateTimeKind()
		{
			DateTime local = new DateTime(2024, 4, 18, 12, 0, 0, DateTimeKind.Local);
			DateTime utc = new DateTime(2024, 4, 18, 12, 0, 0, DateTimeKind.Utc);
			DateTime unspecified = new DateTime(2024, 4, 18, 12, 0, 0, DateTimeKind.Unspecified);

			Assert.AreEqual(DateTimeKind.Local, local.FirstDayOfYear().Kind);
			Assert.AreEqual(DateTimeKind.Utc, utc.FirstDayOfYear().Kind);
			Assert.AreEqual(DateTimeKind.Unspecified, unspecified.FirstDayOfYear().Kind);
		}

		[TestMethod]
		public void FirstDayOfYear_WhenMinValue_ShouldReturnExpected()
		{
			DateTime min = DateTime.MinValue; // 0001-01-01
			DateTime result = min.FirstDayOfYear();

			Assert.AreEqual(new DateTime(1, 1, 1), result);
		}

		[TestMethod]
		public void FirstDayOfYear_WhenMaxValue_ShouldReturnExpected()
		{
			DateTime max = DateTime.MaxValue; // 9999-12-31
			DateTime result = max.FirstDayOfYear();

			Assert.AreEqual(new DateTime(9999, 1, 1), result);
		}
	}
}
