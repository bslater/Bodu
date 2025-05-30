﻿// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{

		[DataTestMethod]
		[DynamicData(nameof(PreviousDayOfWeekTestData), DynamicDataSourceType.Method)]
		public void PreviousDayOfWeek_WhenCalled_ShouldReturnExpectedDate(DateTime input, DayOfWeek targetDay, DateTime expected)
		{
			DateTime actual = input.PreviousDayOfWeek(targetDay);

			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void PreviousDayOfWeek_WhenEnumIsInvalid_ShouldThrowExactly()
		{
			var input = new DateTime(2024, 4, 18);

			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = input.PreviousDayOfWeek((DayOfWeek)999);
			});
		}


		[DataTestMethod]
		[DataRow(DateTimeKind.Unspecified)]
		[DataRow(DateTimeKind.Utc)]
		[DataRow(DateTimeKind.Local)]
		public void PreviousDayOfWeek_WhenKindIsSet_ShouldPreserveKind(DateTimeKind kind)
		{
			DateTime input = new DateTime(2024, 4, 18, 10, 0, 0, kind);
			DateTime actual = input.PreviousDayOfWeek(DayOfWeek.Wednesday);

			Assert.AreEqual(kind, actual.Kind);
		}

		[TestMethod]
		public void PreviousDayOfWeek_WhenTimeIsSet_ShouldPreserveTimed()
		{
			var time = new TimeSpan(0, 12, 32, 55, 34, 903);
			var input =new DateTime(2024, 4, 18).Add(time);

			var actual = input.PreviousDayOfWeek(DayOfWeek.Monday).TimeOfDay;

			Assert.AreEqual(time , actual);
		}


		[TestMethod]
		public void PreviousDayOfWeek_WhenUsingMinValue_ShouldReturnSameOrGreater()
		{
			var actual = DateTime.MinValue.AddDays(7).PreviousDayOfWeek(DayOfWeek.Monday);

			Assert.IsTrue(actual >= DateTime.MinValue);
		}

		[TestMethod]
		public void PreviousDayOfWeek_WhenUsingMaxValue_ShouldSucceed()
		{
			var input = DateTime.MaxValue;
			var actual = input.PreviousDayOfWeek(DayOfWeek.Saturday);

			Assert.IsTrue(actual <= DateTime.MaxValue);
		}
	}
}
