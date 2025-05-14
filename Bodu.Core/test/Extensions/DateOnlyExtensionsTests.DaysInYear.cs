using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{
		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.DaysInYearGregorianCalendarTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void DaysInYear_WhenCalled_ShouldReturnCorrectDays(DateTime inputDateTime, int expected)
		{
			var input = DateOnly.FromDateTime(inputDateTime);
			var actual = input.DaysInYear();
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.DaysInYearTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void DaysInYear_WhenUsingCustomCalendar_ShouldMatchExpected(int year, Calendar calendar, int expectedDays)
		{
			var input = new DateOnly(year, 1, 1);
			var actual = input.DaysInYear(calendar);
			Assert.AreEqual(expectedDays, actual, $"{calendar.GetType().Name} returned {actual} days for year {year}.");
		}

		[TestMethod]
		public void DaysInYear_WhenUsingMinValue_ShouldNotThrow()
		{
			Assert.IsTrue(DateOnly.MinValue.DaysInYear() > 0);
		}

		[TestMethod]
		public void DaysInYear_WhenUsingMaxValue_ShouldNotThrow()
		{
			Assert.IsTrue(DateOnly.MaxValue.DaysInYear() > 0);
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

				DateOnly input = new(1445, 1, 1); // 1445 AH (2023-07-19 Gregorian)
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