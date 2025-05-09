using System;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfMonth(DateTime)" /> returns <c>true</c> when the date represents the
		/// first day of the month.
		/// </summary>
		[DataTestMethod]
		[DataRow("2024-01-01")]
		[DataRow("2024-02-01")]
		[DataRow("2024-12-01")]
		[DataRow("2020-02-01")] // leap year
		[DataRow("2023-04-01")]
		public void IsFirstDayOfMonth_WhenDateIsFirstDay_ShouldReturnTrue(string dateString)
		{
			var date = DateTime.Parse(dateString);
			Assert.IsTrue(date.IsFirstDayOfMonth());
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsFirstDayOfMonth(DateTime)" /> returns <c>false</c> when the date does not
		/// represent the first day of the month.
		/// </summary>
		[DataTestMethod]
		[DataRow("2024-01-02")]
		[DataRow("2024-02-15")]
		[DataRow("2024-12-31")]
		[DataRow("2020-02-29")] // leap day
		[DataRow("2023-04-30")]
		public void IsFirstDayOfMonth_WhenDateIsNotFirstDay_ShouldReturnFalse(string dateString)
		{
			var date = DateTime.Parse(dateString);
			Assert.IsFalse(date.IsFirstDayOfMonth());
		}
	}
}