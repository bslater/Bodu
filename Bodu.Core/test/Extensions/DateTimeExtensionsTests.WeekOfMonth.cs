using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		[DataTestMethod]
		[DynamicData(nameof(WeekOfMonthCalendarWeekRuleTestData), DynamicDataSourceType.Method)]
		public void WeekOfMonth_WithCalendarWeekAndRule_ShouldReturnExpected(DateTime input, CalendarWeekRule rule, DayOfWeek firstDay, int expected)
		{
			int actual = input.WeekOfMonth(rule, firstDay);
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DynamicData(nameof(WeekOfMonthCultureTestData), DynamicDataSourceType.Method)]
		public void WeekOfMonth_WithCulture_ShouldReturnExpected(DateTime input, CultureInfo culture, int expected)
		{
			int actual = input.WeekOfMonth(culture);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WeekOfMonth_WhenUsingCultureInfo_ShouldRespectCultureSettings()
		{
			var date = new DateTime(2024, 05, 15);
			var culture = new CultureInfo("en-US") { DateTimeFormat = { FirstDayOfWeek = DayOfWeek.Sunday } };
			var expected = date.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
			var actual = date.WeekOfMonth(culture);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void WeekOfMonth_WhenUsingDefaultCulture_ShouldMatchExplicitCall()
		{
			var date = new DateTime(2024, 05, 15);
			var expected = date.WeekOfMonth(CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
											CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
			var actual = date.WeekOfMonth();
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DataRow("en-US")]
		[DataRow("en-GB")]
		[DataRow("de-DE")]
		[DataRow("fr-FR")]
		[DataRow("ar-SA")]
		[DataRow("he-IL")]
		public void WeekOfMonth_WhenGivenVariousCultures_ShouldReturnConsistent(string cultureName)
		{
			var date = new DateTime(2024, 04, 05);
			var culture = new CultureInfo(cultureName);
			var expected = date.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
			var actual = date.WeekOfMonth(culture);
			Assert.AreEqual(expected, actual, $"Culture: {cultureName}");
		}

		[TestMethod]
		public void WeekOfMonth_WhenCultureIsNull_ShouldUseCurrentCulture()
		{
			var original = CultureInfo.CurrentCulture;
			try
			{
				CultureInfo.CurrentCulture = new CultureInfo("fr-FR"); // Starts week on Monday
				DateTime input = new(2024, 01, 08); // 2nd Monday of Jan 2024
				int week = input.WeekOfMonth(); // Should use fr-FR's firstDayOfWeek (Monday)

				Assert.AreEqual(2, week); // Jan 8 falls in 2nd full week
			}
			finally
			{
				CultureInfo.CurrentCulture = original;
			}
		}

		[TestMethod]
		public void WeekOfMonth_WhenDayOfWeekIsInvalid_ShouldThrowExactly()
		{
			var date = new DateTime(2024, 01, 01);
			var invalidDay = (DayOfWeek)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = date.WeekOfMonth(CalendarWeekRule.FirstDay, invalidDay);
			});
		}

		[TestMethod]
		public void WeekOfMonth_WhenCalendarWeekRuleIsInvalid_ShouldThrowExactly()
		{
			var date = new DateTime(2024, 01, 01);
			var invalidRule = (CalendarWeekRule)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = date.WeekOfMonth(invalidRule, DayOfWeek.Monday);
			});
		}
	}
}