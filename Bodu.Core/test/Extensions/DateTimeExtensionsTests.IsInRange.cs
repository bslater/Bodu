// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeExtensionsTests.IsInRange.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Bodu.Extensions
{
	public partial class DateTimeExtensionsTests
	{
		private static readonly DateTime SampleDate = new(2024, 4, 15); // 15-Apr-2024

		/// <summary>
		/// Provides test cases for verifying <see cref="DateTimeExtensions.IsInRange(DateTime, DateTime, DateTime)" />.
		/// </summary>
		public static IEnumerable<object[]> GetIsInRangeTestCases()
		{
			yield return new object[] { SampleDate, SampleDate.AddDays(-1), SampleDate.AddDays(1), true };    // Inside range
			yield return new object[] { SampleDate, SampleDate, SampleDate.AddDays(2), true };                // Equal to start
			yield return new object[] { SampleDate, SampleDate.AddDays(-2), SampleDate, true };               // Equal to end
			yield return new object[] { SampleDate, SampleDate.AddDays(1), SampleDate.AddDays(5), false };    // Before range
			yield return new object[] { SampleDate, SampleDate.AddDays(-5), SampleDate.AddDays(-1), false };  // After range
			yield return new object[] { SampleDate, SampleDate.AddDays(1), SampleDate.AddDays(-1), true };    // Reversed range
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsInRange(DateTime, DateTime, DateTime)" /> correctly evaluates date range inclusion.
		/// </summary>
		/// <param name="value">The date to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <param name="expected">The expected result.</param>
		[DataTestMethod]
		[DynamicData(nameof(GetIsInRangeTestCases), DynamicDataSourceType.Method)]
		public void IsInRange_ForDateTime_ShouldReturnExpectedResult(DateTime value, DateTime start, DateTime end, bool expected)
		{
			bool result = value.IsInRange(start, end);

			Assert.AreEqual(expected, result, $"Failed for value={value:yyyy-MM-dd}, start={start:yyyy-MM-dd}, end={end:yyyy-MM-dd}");
		}

		/// <summary>
		/// Provides test cases for verifying <see cref="DateTimeExtensions.IsInRange(DateTime?, DateTime, DateTime)" />.
		/// </summary>
		public static IEnumerable<object[]> GetIsInRangeNullableTestCases()
		{
			yield return new object[] { (DateTime?)SampleDate, SampleDate.AddDays(-1), SampleDate.AddDays(1), true };    // Inside range
			yield return new object[] { (DateTime?)SampleDate, SampleDate, SampleDate.AddDays(2), true };                // Equal to start
			yield return new object[] { (DateTime?)SampleDate, SampleDate.AddDays(-2), SampleDate, true };               // Equal to end
			yield return new object[] { (DateTime?)SampleDate, SampleDate.AddDays(1), SampleDate.AddDays(5), false };    // Before range
			yield return new object[] { (DateTime?)SampleDate, SampleDate.AddDays(-5), SampleDate.AddDays(-1), false };  // After range
			yield return new object[] { (DateTime?)SampleDate, SampleDate.AddDays(1), SampleDate.AddDays(-1), true };    // Reversed range
			yield return new object[] { (DateTime?)null, SampleDate.AddDays(-10), SampleDate.AddDays(10), false };        // Null input
		}

		/// <summary>
		/// Verifies that <see cref="DateTimeExtensions.IsInRange(DateTime?, DateTime, DateTime)" /> correctly evaluates nullable date range inclusion.
		/// </summary>
		/// <param name="value">The nullable date to evaluate.</param>
		/// <param name="start">The start of the range.</param>
		/// <param name="end">The end of the range.</param>
		/// <param name="expected">The expected result.</param>
		[DataTestMethod]
		[DynamicData(nameof(GetIsInRangeNullableTestCases), DynamicDataSourceType.Method)]
		public void IsInRange_ForNullableDateTime_ShouldReturnExpectedResult(DateTime? value, DateTime start, DateTime end, bool expected)
		{
			bool result = value.IsInRange(start, end);

			Assert.AreEqual(expected, result, $"Failed for value={(value.HasValue ? value.Value.ToString("yyyy-MM-dd") : "null")}, start={start:yyyy-MM-dd}, end={end:yyyy-MM-dd}");
		}
	}
}