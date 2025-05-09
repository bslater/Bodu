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
		[DataRow("2020-01-01", 366)]
		[DataRow("2021-01-01", 365)]
		public void DaysInYear_WhenCalled_ShouldReturnCorrectDays(string inputDate, int expected)
		{
			DateTime input = DateTime.Parse(inputDate);

			Assert.AreEqual(expected, input.DaysInYear());
		}

		[TestMethod]
		public void DaysInYear_WhenUsingCustomCalendar_ShouldMatchExpected()
		{
			DateTime input = new DateTime(2024, 4, 18);
			var calendar = new System.Globalization.GregorianCalendar();
			int result = input.DaysInYear(calendar);

			Assert.AreEqual(366, result);
		}
	}
}
