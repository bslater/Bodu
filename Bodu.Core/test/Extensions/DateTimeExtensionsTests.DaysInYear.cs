using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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

		public static IEnumerable<object[]> GetDaysInYearTestData()
		{
			yield return new object[] { 1900, new GregorianCalendar(), 365 };
			yield return new object[] { 2000, new GregorianCalendar(), 366 };
			yield return new object[] { 2100, new GregorianCalendar(), 365 };
			yield return new object[] { 2400, new GregorianCalendar(), 366 };
			yield return new object[] { 2024, new GregorianCalendar(), 366 };
			yield return new object[] { 2023, new GregorianCalendar(), 365 };

			yield return new object[] { 2000, new JulianCalendar(), 366 };
			yield return new object[] { 2100, new JulianCalendar(), 366 };
			yield return new object[] { 2024, new JulianCalendar(), 366 };

			yield return new object[] { 5758, new HebrewCalendar(), 354 };
			yield return new object[] { 5776, new HebrewCalendar(), 385 };
			yield return new object[] { 5784, new HebrewCalendar(), 383 };
			yield return new object[] { 5786, new HebrewCalendar(), 354 };
			yield return new object[] { 5793, new HebrewCalendar(), 383 };
			yield return new object[] { 5802, new HebrewCalendar(), 354 };

			yield return new object[] { 1445, new HijriCalendar(), 355 };
			yield return new object[] { 1444, new HijriCalendar(), 354 };
			yield return new object[] { 1443, new HijriCalendar(), 354 };

			yield return new object[] { 1445, new UmAlQuraCalendar(), 354 };
			yield return new object[] { 1444, new UmAlQuraCalendar(), 354 };
			yield return new object[] { 1443, new UmAlQuraCalendar(), 355 };
		}

		[DataTestMethod]
		[DynamicData(nameof(GetDaysInYearTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void DaysInYear_WhenUsingCustomCalendar_ShouldMatchExpected(int year, Calendar calendar, int expectedDays)
		{
			DateTime input = new DateTime(year, 1, 1);
			int actual = input.DaysInYear(calendar);
			Assert.AreEqual(expectedDays, actual, $"{calendar.GetType().Name} returned {actual} days for year {year}.");
		}

		[TestMethod]
		public void DaysInYear_WhenUsingMinValue_ShouldNotThrow()
		{
			Assert.IsTrue(DateTime.MinValue.DaysInYear() > 0);
		}

		[TestMethod]
		public void DaysInYear_WhenUsingMaxValue_ShouldNotThrow()
		{
			Assert.IsTrue(DateTime.MaxValue.DaysInYear() > 0);
		}

		[TestMethod]
		public void DaysInYear_WhenNoCalendarProvided_ShouldUseCurrentCultureCalendar()
		{
			var previousCulture = CultureInfo.CurrentCulture;
			try
			{
				// Set a known culture with a distinct calendar
				var customCulture = new CultureInfo("ar-SA");
				customCulture.DateTimeFormat.Calendar = new UmAlQuraCalendar(); // Has non-Gregorian year lengths
				CultureInfo.CurrentCulture = customCulture;

				DateTime input = new DateTime(1445, 1, 1); // 1445 AH (2023-07-19 Gregorian)
				int expected = customCulture.DateTimeFormat.Calendar.GetDaysInYear(1445);
				int actual = input.DaysInYear(); // Should use current culture calendar

				Assert.AreEqual(expected, actual);
			}
			finally
			{
				CultureInfo.CurrentCulture = previousCulture;
			}
		}
	}
}