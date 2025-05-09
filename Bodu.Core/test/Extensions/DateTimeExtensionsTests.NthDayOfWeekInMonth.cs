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
		[DataRow("2024-04-01", DayOfWeek.Monday, WeekOfMonthOrdinal.First, "2024-04-01")]   // 1st Monday
		[DataRow("2024-04-01", DayOfWeek.Friday, WeekOfMonthOrdinal.Second, "2024-04-12")]  // 2nd Friday
		[DataRow("2024-04-01", DayOfWeek.Sunday, WeekOfMonthOrdinal.Third, "2024-04-21")]   // 3rd Sunday
		[DataRow("2024-04-01", DayOfWeek.Monday, WeekOfMonthOrdinal.Fourth, "2024-04-22")]  // 4th Monday
		[DataRow("2024-04-01", DayOfWeek.Monday, WeekOfMonthOrdinal.Last, "2024-04-29")]    // Last Monday

		[DataRow("2024-02-01", DayOfWeek.Thursday, WeekOfMonthOrdinal.First, "2024-02-01")] // Feb 1st Thursday
		[DataRow("2024-02-01", DayOfWeek.Thursday, WeekOfMonthOrdinal.Last, "2024-02-29")]  // Leap year � last Thursday

		[DataRow("2023-02-01", DayOfWeek.Sunday, WeekOfMonthOrdinal.Last, "2023-02-26")]    // Non-leap Feb � last Sunday
		[DataRow("2023-02-01", DayOfWeek.Monday, WeekOfMonthOrdinal.Third, "2023-02-20")]   // Feb 2023 � 3rd Monday
		public void GetNthDayOfWeekInMonth_WhenCalled_ShouldReturnExpected(string inputDate, DayOfWeek dayOfWeek, WeekOfMonthOrdinal ordinal, string expectedDate)
		{
			DateTime input = DateTime.Parse(inputDate);
			DateTime expected = DateTime.Parse(expectedDate);
			DateTime actual = input.NthDayOfWeekInMonth(dayOfWeek, ordinal);

			Assert.AreEqual(expected, actual);
		}


		[TestMethod]
		public void GetNthDayOfWeekInMonth_WhenFifthDoesNotExist_ShouldThrowExactly()
		{
			DateTime input = new DateTime(2023, 2, 1); // February 2023 has only 4 Wednesdays

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				input.NthDayOfWeekInMonth(DayOfWeek.Wednesday, WeekOfMonthOrdinal.Fifth);
			});
		}

		[TestMethod]
		public void GetNthDayOfWeekInMonth_WhenOrdinalIsInvalidEnum_ShouldThrowExactly()
		{
			DateTime input = new DateTime(2024, 1, 1);
			WeekOfMonthOrdinal invalidOrdinal = (WeekOfMonthOrdinal)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				input.NthDayOfWeekInMonth(DayOfWeek.Monday, invalidOrdinal);
			});
		}

		[TestMethod]
		public void GetNthDayOfWeekInMonth_WhenDayOfWeekIsInvalidEnum_ShouldThrowExactly()
		{
			DateTime input = new DateTime(2024, 1, 1);
			DayOfWeek invalidDay = (DayOfWeek)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				input.NthDayOfWeekInMonth(invalidDay, WeekOfMonthOrdinal.First);
			});
		}
	}
}