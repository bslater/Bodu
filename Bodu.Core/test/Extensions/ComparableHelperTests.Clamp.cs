using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Extensions
{
	public partial class ComparableHelperTests
	{
		private static IEnumerable<object[]> GetClampTestData()
		{
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(1), new SimpleTestObject(10), new SimpleTestObject(5) };  // inside range
			yield return new object[] { new SimpleTestObject(0), new SimpleTestObject(1), new SimpleTestObject(10), new SimpleTestObject(1) };  // below min
			yield return new object[] { new SimpleTestObject(15), new SimpleTestObject(1), new SimpleTestObject(10), new SimpleTestObject(10) }; // above max
			yield return new object[] { null, new SimpleTestObject(1), new SimpleTestObject(10), null };                                         // null value
			yield return new object[] { new SimpleTestObject(5), null, new SimpleTestObject(10), new SimpleTestObject(5) };                      // no lower bound
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(1), null, new SimpleTestObject(5) };                       // no upper bound
		}

		/// <summary>
		/// Verifies that Clamp restricts a <see cref="SimpleTestObject" /> value within the specified bounds.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetClampTestData), DynamicDataSourceType.Method)]
		public void Clamp_WhenValueOutsideBounds_ShouldReturnBound(SimpleTestObject? value, SimpleTestObject? min, SimpleTestObject? max, SimpleTestObject? expected)
		{
			var result = ComparableHelper.Clamp(value, min, max);
			Assert.AreEqual(expected, result);
		}
	}
}