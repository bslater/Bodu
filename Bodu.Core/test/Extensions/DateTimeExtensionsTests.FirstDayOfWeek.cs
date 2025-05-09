﻿// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;
using System.Collections.Generic;
using System.Data;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		[DataTestMethod]
		[DataRow("2024-04-14", "2024-04-14", "en-US")] // Sunday is first day
		[DataRow("2024-04-15", "2024-04-14", "en-US")] // Monday
		[DataRow("2024-04-16", "2024-04-14", "en-US")] // Tuesday

		[DataRow("2024-04-14", "2024-04-08", "en-GB")] // GB: Monday is first day
		[DataRow("2024-04-15", "2024-04-15", "en-GB")] // GB: Monday
		[DataRow("2024-04-20", "2024-04-15", "en-GB")] // GB: Saturday
		public void FirstDayOfWeek_WhenUsingCulture_ShouldReturnExpectedStart(string inputDate, string expectedDate, string cultureName)
		{
			var originalCulture = CultureInfo.CurrentCulture;

			try
			{
				CultureInfo.CurrentCulture = new CultureInfo(cultureName);

				DateTime input = DateTime.Parse(inputDate, CultureInfo.CurrentCulture);
				DateTime expected = DateTime.Parse(expectedDate, CultureInfo.CurrentCulture);

				DateTime result = input.FirstDayOfWeek();

				Assert.AreEqual(expected, result, $"Failed for culture: {cultureName}");
			}
			finally
			{
				CultureInfo.CurrentCulture = originalCulture;
			}
		}

		[DataTestMethod]
		[DataRow("2024-04-14", "2024-04-08")] // Sunday → previous Monday
		[DataRow("2024-04-15", "2024-04-15")] // Monday (start of week)
		[DataRow("2024-04-17", "2024-04-15")] // Wednesday
		[DataRow("2024-04-21", "2024-04-15")] // Sunday
		public void FirstDayOfWeek_WhenUsingFrenchCulture_ShouldReturnExpectedStart(string inputDate, string expectedDate)
		{
			CultureInfo frenchCulture = new CultureInfo("fr-FR");

			DateTime input = DateTime.Parse(inputDate);
			DateTime expected = DateTime.Parse(expectedDate);
			DateTime result = input.FirstDayOfWeek(frenchCulture);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenCultureIsNull_ShouldUseCurrentCulture()
		{
			var originalCulture = CultureInfo.CurrentCulture;
			try
			{
				CultureInfo.CurrentCulture = DateTimeExtensionsTests.TestCulture;

				DateTime input = new DateTime(2024, 4, 18); // Thursday
															// Backtrack to previous Wednesday → 2024-04-17
				DateTime expected = new DateTime(2024, 4, 17);

				DateTime result = input.FirstDayOfWeek(null!);

				Assert.AreEqual(expected, result, "Expected fallback to CultureInfo.CurrentCulture with Wednesday as start of week.");
			}
			finally
			{
				CultureInfo.CurrentCulture = originalCulture; // Always restore
			}
		}

		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void FirstDayOfWeek_WhenUsingDefaultOverload_ShouldPreserveDateTimeKind(DateTimeKind kind)
		{
			var original = new DateTime(2024, 1, 3, 0, 0, 0, kind);
			var result = original.FirstDayOfWeek();
			Assert.AreEqual(kind, result.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void FirstDayOfWeek_WhenUsingCulture_ShouldPreserveDateTimeKind(DateTimeKind kind)
		{
			var original = new DateTime(2024, 1, 3, 0, 0, 0, kind);
			var result = original.FirstDayOfWeek(CultureInfo.CurrentCulture);
			Assert.AreEqual(kind, result.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenUsingMinValue_ShouldReturnMin()
		{
			DateTime min = DateTime.MinValue;
			DateTime result = min.FirstDayOfWeek(new CultureInfo("en-GB")); // Monday is first day

			Assert.AreEqual(min.Date, result);
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenUsingMinValueWithSundayStart_ShouldThrowExactly()
		{
			DateTime min = DateTime.MinValue;
			var culture = new CultureInfo("en-US"); // Sunday is first day

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = min.FirstDayOfWeek(culture); // is outside the range for a DateTime value
			});
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenUsingMaxValue_ShouldReturnValidStart()
		{
			DateTime max = DateTime.MaxValue.Date;
			DateTime result = max.FirstDayOfWeek(new CultureInfo("en-US"));

			Assert.IsTrue(result <= max);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> returns the expected result based on the specified weekend definition.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(FirstAndLastDayOfWeekTestData), DynamicDataSourceType.Property)]
		public void FirstDayOfWeek_WhenUsingWeekendDefinition_ShouldReturnExpectedStart(DateTime input, CalendarWeekendDefinition weekend, DateTime expectedStart, DateTime _)
		{
			var actual = input.FirstDayOfWeek(weekend);
			Assert.AreEqual(expectedStart, actual);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> throws when given an undefined <see cref="CalendarWeekendDefinition"/>.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenWeekendDefinitionIsInvalid_ShouldThrowExactly()
		{
			var date = new DateTime(2024, 1, 1);
			var invalidWeekend = (CalendarWeekendDefinition)999;

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = date.FirstDayOfWeek(invalidWeekend);
			});
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> throws if the calculated result underflows <see cref="DateTime.MinValue"/>.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenResultWouldUnderflowMinValue_ShouldThrowExactly()
		{
			var nearMin = DateTime.MinValue.AddDays(1); // e.g., Jan 2, 0001
			var weekend = CalendarWeekendDefinition.FridaySaturday; // Start of week = Sunday → offset = -1

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = nearMin.FirstDayOfWeek(weekend);
			});
		}

		/// <summary>
		/// Verifies that the result of <see cref="DateTimeExtensions.FirstDayOfWeek"/> preserves the <see cref="DateTime.Kind" />.
		/// </summary>
		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void FirstDayOfWeek_WhenUsingSaturdaySunday_ShouldPreserveDateTimeKind(DateTimeKind kind)
		{
			var original = new DateTime(2024, 1, 3, 0, 0, 0, kind);
			var result = original.FirstDayOfWeek(CalendarWeekendDefinition.SaturdaySunday);
			Assert.AreEqual(kind, result.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> works near <see cref="DateTime.MinValue"/> without throwing.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenNearMinValue_ShouldReturnExpectedStart()
		{
			var date = DateTime.MinValue.AddDays(6); // 0001-01-07
			var result = date.FirstDayOfWeek(CalendarWeekendDefinition.SaturdaySunday);

			Assert.IsTrue(result >= DateTime.MinValue);
			Assert.AreEqual(DayOfWeek.Monday, result.DayOfWeek);
		}
	}
}
