﻿// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;
using System.Collections.Generic;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{
		[DataTestMethod]
		[DataRow("2024-04-14", "2024-04-14", "en-US")] // Sunday is first day
		[DataRow("2024-04-15", "2024-04-14", "en-US")] // Monday
		[DataRow("2024-04-16", "2024-04-14", "en-US")] // Tuesday

		[DataRow("2024-04-14", "2024-04-08", "en-GB")] // GB: Monday is first day
		[DataRow("2024-04-15", "2024-04-15", "en-GB")] // GB: Monday
		[DataRow("2024-04-20", "2024-04-15", "en-GB")] // GB: Saturday
		public void FirstDayOfWeek_WhenUsingDefaultCulture_ShouldReturnExpected(string inputDate, string expectedDate, string cultureName)
		{
			var originalCulture = CultureInfo.CurrentCulture;

			try
			{
				CultureInfo.CurrentCulture = new CultureInfo(cultureName);

				DateOnly input = DateOnly.Parse(inputDate, CultureInfo.CurrentCulture);
				DateOnly expected = DateOnly.Parse(expectedDate, CultureInfo.CurrentCulture);

				DateOnly result = input.FirstDayOfWeek();

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
		public void FirstDayOfWeek_WhenUsingFrenchCulture_ShouldReturnExpected(string inputDate, string expectedDate)
		{
			CultureInfo frenchCulture = new CultureInfo("fr-FR");

			DateOnly input = DateOnly.Parse(inputDate);
			DateOnly expected = DateOnly.Parse(expectedDate);
			DateOnly result = input.FirstDayOfWeek(frenchCulture);

			Assert.AreEqual(expected, result);
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenCultureIsNull_ShouldUseCurrentCulture()
		{
			var originalCulture = CultureInfo.CurrentCulture;
			try
			{
				CultureInfo.CurrentCulture = DateTimeExtensionsTests.TestCulture;

				DateOnly input = new DateOnly(2024, 4, 18); // Thursday
															// Backtrack to previous Wednesday → 2024-04-17
				DateOnly expected = new DateOnly(2024, 4, 17);

				DateOnly result = input.FirstDayOfWeek(null!);

				Assert.AreEqual(expected, result, "Expected fallback to CultureInfo.CurrentCulture with Wednesday as start of week.");
			}
			finally
			{
				CultureInfo.CurrentCulture = originalCulture; // Always restore
			}
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenMinValue_ShouldReturnMin()
		{
			DateOnly min = DateOnly.MinValue;
			DateOnly result = min.FirstDayOfWeek(new CultureInfo("en-GB")); // Monday is first day

			Assert.AreEqual(min, result);
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenMinValueAndCultureIsUS_ShouldReturnThrowArgumentOutOfRangeException()
		{
			DateOnly min = DateOnly.MinValue;
			var culture = new CultureInfo("en-US"); // Sunday is first day

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = min.FirstDayOfWeek(culture); // is outside the range for a DateOnly value
			});
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenMaxValue_ShouldReturnStartOfWeek()
		{
			DateOnly max = DateOnly.MaxValue;
			DateOnly result = max.FirstDayOfWeek(new CultureInfo("en-US"));

			Assert.IsTrue(result <= max);
		}


		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> returns the expected result based on the specified weekend definition.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.FirstAndLastDayOfWeekTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Property)]
		public void FirstDayOfWeek_WhenUsingWeekendDefinition_ShouldReturnExpectedStart(DateTime dateTimeInput, CalendarWeekendDefinition weekend, DateTime dateTimeExpected, DateTime _)
		{
			var input = DateOnly.FromDateTime(dateTimeInput);
			var expected = DateOnly.FromDateTime(dateTimeExpected);
			var actual = input.FirstDayOfWeek(weekend);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> throws when given an undefined <see cref="CalendarWeekendDefinition"/>.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenWeekendIsUndefined_ShouldThrowArgumentOutOfRangeException()
		{
			var date = new DateOnly(2024, 1, 1);
			var invalidWeekend = (CalendarWeekendDefinition)999;

			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				_ = date.FirstDayOfWeek(invalidWeekend);
			});
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> throws if the calculated result underflows <see cref="DateTime.MinValue"/>.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenResultUnderflowsMinValue_ShouldThrowArgumentOutOfRangeException()
		{
			var nearMin = DateOnly.MinValue.AddDays(1); // e.g., Jan 2, 0001
			var weekend = CalendarWeekendDefinition.FridaySaturday; // Start of week = Sunday → offset = -1

			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				_ = nearMin.FirstDayOfWeek(weekend);
			});
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> works near <see cref="DateTime.MinValue"/> without throwing.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenNearMinValue_ShouldReturnValidResult()
		{
			var date = DateOnly.MinValue.AddDays(6); // 0001-01-07
			var result = date.FirstDayOfWeek(CalendarWeekendDefinition.SaturdaySunday);

			Assert.IsTrue(result >= DateOnly.MinValue);
			Assert.AreEqual(DayOfWeek.Monday, result.DayOfWeek);
		}
	}
}
