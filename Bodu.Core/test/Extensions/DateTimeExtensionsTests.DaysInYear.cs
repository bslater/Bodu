using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		[DataTestMethod]
		[DynamicData(nameof(DaysInYearGregorianCalendarTestData), DynamicDataSourceType.Method)]
		public void DaysInYear_WhenCalled_ShouldReturnCorrectDays(DateTime input, int expected)
		{
			var actual = input.DaysInYear();
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DynamicData(nameof(DaysInYearTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void DaysInYear_WhenUsingCustomCalendar_ShouldMatchExpected(int year, Calendar calendar, int expectedDays)
		{
			DateTime input = new(year, 1, 1);
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

				DateTime input = new(1445, 1, 1); // 1445 AH (2023-07-19 Gregorian)
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