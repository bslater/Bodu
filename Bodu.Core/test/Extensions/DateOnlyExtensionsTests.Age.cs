// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateOnlyExtensions.Age.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Extensions.Tests
{
	public partial class DateOnlyExtensionTests
	{
		public static IEnumerable<object[]> GetAgeTestData => new[]
		{
			new object[] { "2000-01-01", "2024-04-18", 24 },
			new object[] { "2000-04-18", "2024-04-18", 24 },
			new object[] { "2000-04-18", "2024-04-17", 23 },
			new object[] { "2000-12-31", "2024-04-18", 23 },
			new object[] { "2000-12-31", "2021-01-01", 20 },
			new object[] { "2000-12-31", "2021-12-31", 21 },
			new object[] { "2000-04-18", "2000-04-18", 0 },
			new object[] { "2000-04-18", "1999-12-31", 0 },
			new object[] { "2000-02-29", "2023-02-28", 23 },
			new object[] { "2000-02-29", "2024-02-29", 24 },
			new object[] { "1900-01-01", "2000-01-01", 100 },
			new object[] { "2000-04-18", "2024-04-18", 24 },
			new object[] { "2000-01-01", "2024-12-31", 24 },
			new object[] { "2000-04-18", "2024-04-18", 24 },
			new object[] { "2000-04-18", "2024-04-17", 23 },
			new object[] { "2000-04-18", "1999-04-18", 0 },
			new object[] { "0001-01-01", "0001-12-31", 0 },
			new object[] { "0001-01-01", "9999-12-31", 9998 },
			new object[] { "2001-02-28", "2024-02-28", 23 }
		};

		/// <summary>
		/// Verifies that the <see cref="DateOnlyExtensions.Age(DateOnly, DateOnly)" /> method returns the expected age in full calendar
		/// years for various date combinations.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetAgeTestData), DynamicDataSourceType.Property)]
		public void Age_WhenCalculatedAgainstDate_ShouldReturnExpected(string birthAsString, string atDateString, int expected)
		{
			DateOnly birth = DateOnly.Parse(birthAsString);
			DateOnly atDate = DateOnly.Parse(atDateString);
			int actual = birth.Age(atDate);
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that <see cref="DateOnlyExtensions.Age(DateOnly)" /> returns the same result as
		/// <see cref="DateOnlyExtensions.Age(DateOnly, DateOnly)" /> when called with <see cref="DateOnly.FromDateTime(DateTime.Today)" />.
		/// </summary>
		[TestMethod]
		public void Age_WhenUsingDefaultToday_ShouldMatchExplicitCall()
		{
			DateOnly birth = DateOnly.FromDateTime(DateTime.Today.AddYears(-1));
			int expected = birth.Age(DateOnly.FromDateTime(DateTime.Today));
			int actual = birth.Age();
			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that dates before the minimum supported year do not throw but clamp to 0.
		/// </summary>
		[TestMethod]
		public void Age_WhenReferenceDateBeforeBirth_ShouldReturnZero()
		{
			DateOnly birth = new(2000, 1, 1);
			DateOnly earlier = new(1999, 12, 31);
			Assert.AreEqual(0, birth.Age(earlier));
		}

		/// <summary>
		/// Verifies that the age is correctly adjusted when the evaluation date is a non-leap year.
		/// </summary>
		[TestMethod]
		public void Age_WhenBornOnLeapDay_EvaluatedInNonLeapYear_ShouldAdjustToFeb28()
		{
			DateOnly birth = new(2000, 2, 29);
			DateOnly reference = new(2023, 2, 28);
			Assert.AreEqual(23, birth.Age(reference));
		}

		/// <summary>
		/// Verifies that minimum and maximum supported dates do not throw.
		/// </summary>
		[TestMethod]
		public void Age_WhenUsingMinAndMaxDateOnly_ShouldNotThrow()
		{
			int age = DateOnly.MinValue.Age(DateOnly.MaxValue);
			Assert.IsTrue(age > 0);
		}

		/// <summary>
		/// Verifies that calculating age from MaxValue to MinValue clamps to 0.
		/// </summary>
		[TestMethod]
		public void Age_WhenMaxEvaluatedBeforeMin_ShouldReturnZero()
		{
			int age = DateOnly.MaxValue.Age(DateOnly.MinValue);
			Assert.AreEqual(0, age);
		}
	}
}