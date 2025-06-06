// ---------------------------------------------------------------------------------------------------------------
// <auto-generated />
// ---------------------------------------------------------------------------------------------------------------

using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bodu.Extensions;
using System.Globalization;

namespace Bodu.Extensions
{
	public partial class DateOnlyExtensionsTests
	{


		[DataTestMethod]
		[DynamicData(nameof(DateTimeExtensionsTests.WeekendTestData), typeof(DateTimeExtensionsTests), DynamicDataSourceType.Method)]
		public void IsWeekday_WhenUsingStandardWeekend_ShouldReturnExpected(DateTime input, CalendarWeekendDefinition weekend, Type? providerType, bool expected)
		{
			IWeekendDefinitionProvider? provider = providerType is null ? null : (IWeekendDefinitionProvider)Activator.CreateInstance(providerType)!;

			bool actual = input.IsWeekday(weekend, provider);
			Assert.AreEqual(!expected, actual, $"Failed for {input} with weekend {weekend}");
		}

		[TestMethod]
		public void IsWeekday_WhenCustomRuleMissingProvider_ShouldThrowExactly()
		{
			DateTime date = new DateTime(2024, 4, 19);
			Assert.ThrowsExactly<ArgumentOutOfRangeException>(() =>
			{
				_ = date.IsWeekday(CalendarWeekendDefinition.Custom, null!);
			});
		}

	}
}