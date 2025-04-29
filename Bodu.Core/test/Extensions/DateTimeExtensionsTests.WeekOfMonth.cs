using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		public static IEnumerable<object[]> GetUnifiedWeekOfMonthTestCases()
		{
			// Culture-specific tests
			yield return new object[] { new DateTime(2024, 01, 01), "en-US", CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 1 };
			yield return new object[] { new DateTime(2024, 01, 07), "en-US", CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 2 };
			yield return new object[] { new DateTime(2024, 01, 31), "en-US", CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 5 };
			yield return new object[] { new DateTime(2024, 03, 31), "en-US", CalendarWeekRule.FirstDay, DayOfWeek.Monday, 5 };
			yield return new object[] { new DateTime(2024, 06, 30), "en-US", CalendarWeekRule.FirstDay, DayOfWeek.Sunday, 6 };

			yield return new object[] { new DateTime(2024, 01, 01), "fr-FR", CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 1 };
			yield return new object[] { new DateTime(2024, 01, 08), "fr-FR", CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday, 2 };

			yield return new object[] { new DateTime(2024, 01, 01), "ar-SA", CalendarWeekRule.FirstDay, DayOfWeek.Saturday, 1 };
			yield return new object[] { new DateTime(2024, 01, 07), "ar-SA", CalendarWeekRule.FirstDay, DayOfWeek.Saturday, 2 };

			// Culture-agnostic (null means use current)
			yield return new object[] { new DateTime(2024, 01, 01), null!, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday, 1 };
			yield return new object[] { new DateTime(2024, 01, 10), null!, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday, 2 };
		}

		[DataTestMethod]
		[DynamicData(nameof(GetUnifiedWeekOfMonthTestCases), DynamicDataSourceType.Method)]
		public void WeekOfMonth_WithVariousCultureAndRules_ShouldReturnExpected(
			DateTime date,
			string? cultureName,
			CalendarWeekRule rule,
			DayOfWeek firstDay,
			int expectedWeek)
		{
			CultureInfo culture = cultureName != null
				? new CultureInfo(cultureName)
				: (CultureInfo)CultureInfo.CurrentCulture.Clone();

			culture.DateTimeFormat.FirstDayOfWeek = firstDay;
			culture.DateTimeFormat.CalendarWeekRule = rule;

			int result = date.WeekOfMonth(culture);
			Assert.AreEqual(expectedWeek, result, $"Failed for {date:yyyy-MM-dd}, culture={cultureName ?? "Current"}, rule={rule}, firstDay={firstDay}");
		}

		[TestMethod]
		public void WeekOfMonth_WhenUsingCultureInfo_ShouldRespectCultureSettings()
		{
			var date = new DateTime(2024, 05, 15);
			var culture = new CultureInfo("en-US") { DateTimeFormat = { FirstDayOfWeek = DayOfWeek.Sunday } };
			var expected = date.WeekOfMonth(culture.DateTimeFormat.CalendarWeekRule, culture.DateTimeFormat.FirstDayOfWeek);
			var result = date.WeekOfMonth(culture);
			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void WeekOfMonth_WhenUsingDefaultCulture_ShouldMatchExplicitCall()
		{
			var date = new DateTime(2024, 05, 15);
			var expected = date.WeekOfMonth(CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule,
											CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek);
			var result = date.WeekOfMonth();
			Assert.AreEqual(expected, result);
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
			var result = date.WeekOfMonth(culture);
			Assert.AreEqual(expected, result, $"Culture: {cultureName}");
		}

		[TestMethod]
		public void WeekOfMonth_WhenCultureIsNull_ShouldUseCurrentCulture()
		{
			var original = CultureInfo.CurrentCulture;
			try
			{
				CultureInfo.CurrentCulture = new CultureInfo("fr-FR"); // Starts week on Monday
				DateTime input = new DateTime(2024, 01, 08); // 2nd Monday of Jan 2024
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