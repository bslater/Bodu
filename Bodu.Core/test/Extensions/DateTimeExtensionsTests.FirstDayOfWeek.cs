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
		[DynamicData(nameof(FirstDayOfWeekCultureInfoTestData), DynamicDataSourceType.Method)]
		public void FirstDayOfWeek_WhenCurrentCultureSet_ShouldReturnExpectedStart(DateTime input, CultureInfo culture, DateTime expected)
		{
			var originalCulture = CultureInfo.CurrentCulture;

			try
			{
				CultureInfo.CurrentCulture = culture;

				DateTime actual = input.FirstDayOfWeek();

				Assert.AreEqual(expected, actual, $"Failed for culture: {culture.Name}");
			}
			finally
			{
				CultureInfo.CurrentCulture = originalCulture;
			}
		}

		[DataTestMethod]
		[DynamicData(nameof(FirstDayOfWeekCultureInfoTestData), DynamicDataSourceType.Method)]
		public void FirstDayOfWeek_WhenCulture_ShouldReturnExpectedStart(DateTime input, CultureInfo culture, DateTime expected)
		{
			DateTime actual = input.FirstDayOfWeek(culture);

			Assert.AreEqual(expected, actual);
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

				DateTime actual = input.FirstDayOfWeek(null!);

				Assert.AreEqual(expected, actual, "Expected fallback to CultureInfo.CurrentCulture with Wednesday as start of week.");
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
			var actual = original.FirstDayOfWeek();
			Assert.AreEqual(kind, actual.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void FirstDayOfWeek_WhenUsingCulture_ShouldPreserveDateTimeKind(DateTimeKind kind)
		{
			var original = new DateTime(2024, 1, 3, 0, 0, 0, kind);
			var actual = original.FirstDayOfWeek(CultureInfo.CurrentCulture);

			Assert.AreEqual(kind, actual.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		[TestMethod]
		public void FirstDayOfWeek_WhenUsingMinValue_ShouldReturnMin()
		{
			DateTime min = DateTime.MinValue;
			DateTime actual = min.FirstDayOfWeek(new CultureInfo("en-GB")); // Monday is first day

			Assert.AreEqual(min.Date, actual);
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
			DateTime actual = max.FirstDayOfWeek(new CultureInfo("en-US"));

			Assert.IsTrue(actual <= max);
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> returns the expected actual based on the specified weekend definition.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(FirstDayOfWeekDefinitionTestData), DynamicDataSourceType.Method)]
		public void FirstDayOfWeek_WhenUsingWeekendDefinition_ShouldReturnExpectedStart(DateTime input, CalendarWeekendDefinition weekend, DateTime expected)
		{
			var actual = input.FirstDayOfWeek(weekend);

			Assert.AreEqual(expected, actual);
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
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> throws if the calculated actual underflows <see cref="DateTime.MinValue"/>.
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
		/// Verifies that the actual of <see cref="DateTimeExtensions.FirstDayOfWeek"/> preserves the <see cref="DateTime.Kind" />.
		/// </summary>
		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void FirstDayOfWeek_WhenUsingSaturdaySunday_ShouldPreserveDateTimeKind(DateTimeKind kind)
		{
			var original = new DateTime(2024, 1, 3, 0, 0, 0, kind);
			var actual = original.FirstDayOfWeek(CalendarWeekendDefinition.SaturdaySunday);
			Assert.AreEqual(kind, actual.Kind, $"Kind mismatch for FirstDayOfWeek with {kind}");
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.FirstDayOfWeek"/> works near <see cref="DateTime.MinValue"/> without throwing.
		/// </summary>
		[TestMethod]
		public void FirstDayOfWeek_WhenNearMinValue_ShouldReturnExpectedStart()
		{
			var date = DateTime.MinValue.AddDays(6); // 0001-01-07
			var actual = date.FirstDayOfWeek(CalendarWeekendDefinition.SaturdaySunday);

			Assert.IsTrue(actual >= DateTime.MinValue);
			Assert.AreEqual(DayOfWeek.Monday, actual.DayOfWeek);
		}


		[DataTestMethod]
		[DynamicData(nameof(CalendarWeekendDefinitionDateTimeKindTestData), DynamicDataSourceType.Method)]
		public void FirstDayOfWeek_WhenWeekendDefinitionAndKindIsSet_ShouldPreserveKind(CalendarWeekendDefinition definition, DateTimeKind kind)
		{
			DateTime input = new DateTime(2024, 7, 5, 10, 0, 0, kind);
			DateTime actual = input.FirstDayOfWeek(definition);

			Assert.AreEqual(kind, actual.Kind);

		}
	}
}