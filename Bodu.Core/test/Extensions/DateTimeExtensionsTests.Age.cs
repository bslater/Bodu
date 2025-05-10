using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu.Extensions.Tests
{
	public partial class DateTimeExtensionsTests
	{
		public static IEnumerable<object[]> GetAgeTestData => new[]
			{
				new object[] { "2000-01-01", "2024-04-18", 24 },                    // Birthday passed this year
				new object[] { "2000-04-18", "2024-04-18", 24 },                    // Birthday today
				new object[] { "2000-04-18", "2024-04-17", 23 },                    // Day before birthday
				new object[] { "2000-12-31", "2024-04-18", 23 },                    // Birthday not yet reached
				new object[] { "2000-12-31", "2021-01-01", 20 },                    // Not yet birthday next year
				new object[] { "2000-12-31", "2021-12-31", 21 },                    // Birthday on Dec 31
				new object[] { "2000-04-18", "2000-04-18", 0 },                     // Same day
				new object[] { "2000-04-18", "1999-12-31", 0 },                     // Clamped to 0
				new object[] { "2000-02-29", "2023-02-28", 23 },                    // Leap day → non-leap year
				new object[] { "2000-02-29", "2024-02-29", 24 },                    // Leap day → leap year
				new object[] { "1900-01-01", "2000-01-01", 100 },                   // Century span
				new object[] { "2000-04-18T23:59:59", "2024-04-18T00:00:00", 24 },  // Time ignored
				new object[] { "2000-01-01", "2024-12-31", 24 },                    // Birthday passed early in year
				new object[] { "2000-04-18", "2024-04-18T23:59:59", 24 },           // Time late in day
				new object[] { "2000-04-18", "2024-04-17T00:00:00", 23 },           // Midnight day before
				new object[] { "2000-04-18", "1999-04-18", 0 },                     // Negative range
				new object[] { "0001-01-01", "0001-12-31", 0 },                     // First year
				new object[] { "0001-01-01", "9999-12-31", 9998 },                  // Max valid range
				new object[] { "2001-02-28", "2024-02-28", 23 }                     // Born non-leap, evaluated in leap year
			};

		/// <summary>
		/// Verifies that the <see cref="DateTimeExtensions.Age(DateTime, DateTime)" /> method returns the expected age in full calendar
		/// years for various date combinations.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetAgeTestData), DynamicDataSourceType.Property)]
		public void Age_WhenCalculatedAgainstDate_ShouldReturnExpected(string birthAsString, string atDateString, int expected)
		{
			DateTime input = DateTime.Parse(birthAsString);
			DateTime atDate = DateTime.Parse(atDateString);

			int result = input.Age(atDate);

			Assert.AreEqual(expected, result, $"Expected age {expected} but got {result} for input ({birthAsString}, {atDateString})");
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.Age(DateTime)" /> returns the same result as
		/// <see cref="DateTimeExtensions.Age(DateTime, DateTime)" /> when called with <see cref="DateTime.Today" />.
		/// </summary>
		[TestMethod]
		public void Age_WhenUsingDefaultToday_ShouldMatchExplicitCall()
		{
			DateTime birth = DateTime.Today.AddYears(-1);
			int expected = birth.Age(DateTime.Today);
			int actual = birth.Age();

			Assert.AreEqual(expected, actual, "Default overload did not match Age(input, DateTime.Today)");
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeKind" /> differences are ignored in age calculations.
		/// </summary>
		[TestMethod]
		public void Age_WhenUsingMixedDateTimeKinds_ShouldIgnoreKind()
		{
			var birth = new DateTime(2000, 4, 18, 0, 0, 0, DateTimeKind.Local);
			var atDate = new DateTime(2024, 4, 18, 0, 0, 0, DateTimeKind.Utc);

			Assert.AreEqual(24, birth.Age(atDate));
		}
	}
}