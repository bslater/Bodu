﻿namespace Bodu.Extensions
{
	public partial class ComparableHelperTests
	{
		private static IEnumerable<object[]> GetMinTestData()
		{
			yield return new object[] { new SimpleTestObject(1), new SimpleTestObject(2), new SimpleTestObject(1) };
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(5), new SimpleTestObject(5) };
			yield return new object[] { new SimpleTestObject(10), new SimpleTestObject(3), new SimpleTestObject(3) };
			yield return new object[] { null, new SimpleTestObject(7), new SimpleTestObject(7) };
			yield return new object[] { new SimpleTestObject(8), null, new SimpleTestObject(8) };
			yield return new object[] { null, null, null };
		}

		/// <summary>
		/// Verifies that Min returns the smaller of two <see cref="SimpleTestObject" /> values, handling nulls appropriately.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetMinTestData), DynamicDataSourceType.Method)]
		public void Min_WhenComparingTwoValues_ShouldReturnSmaller(SimpleTestObject? first, SimpleTestObject? second, SimpleTestObject? expected)
		{
			var result = ComparableHelper.Min(first, second);
			Assert.AreEqual(expected, result);
		}
	}
}