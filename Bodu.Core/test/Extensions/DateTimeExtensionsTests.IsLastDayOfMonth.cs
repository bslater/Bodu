using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfMonth(DateTime)" /> returns <c>true</c> when the date represents the last
		/// day of the month.
		/// </summary>
		[DataTestMethod]
		[DataRow("2024-01-31")]
		[DataRow("2024-02-29")] // leap year
		[DataRow("2023-02-28")] // non-leap year
		[DataRow("2024-04-30")]
		[DataRow("2024-12-31")]
		public void IsLastDayOfMonth_WhenDateIsLastDay_ShouldReturnTrue(string dateString)
		{
			var date = DateTime.Parse(dateString);
			Assert.IsTrue(date.IsLastDayOfMonth());
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsLastDayOfMonth(DateTime)" /> returns <c>false</c> when the date does not represent
		/// the last day of the month.
		/// </summary>
		[DataTestMethod]
		[DataRow("2024-01-30")]
		[DataRow("2024-02-28")] // leap year, not last day
		[DataRow("2023-02-27")]
		[DataRow("2024-04-15")]
		[DataRow("2024-12-01")]
		public void IsLastDayOfMonth_WhenDateIsNotLastDay_ShouldReturnFalse(string dateString)
		{
			var date = DateTime.Parse(dateString);
			Assert.IsFalse(date.IsLastDayOfMonth());
		}
	}
}