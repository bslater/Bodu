namespace Bodu.Extensions
{
	public partial class ComparableHelperTests
	{
		/// <summary>
		/// Provides test data for verifying <see cref="ComparableHelper.IsOutside{T}(T, T, T)" />.
		/// </summary>
		private static IEnumerable<object[]> GetIsOutsideTestData()
		{
			// Normal order (value1 < value2)
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(1), new SimpleTestObject(10), false };  // inside range
			yield return new object[] { new SimpleTestObject(1), new SimpleTestObject(1), new SimpleTestObject(10), false };  // on lower boundary
			yield return new object[] { new SimpleTestObject(10), new SimpleTestObject(1), new SimpleTestObject(10), false }; // on upper boundary
			yield return new object[] { new SimpleTestObject(0), new SimpleTestObject(1), new SimpleTestObject(10), true };   // below range
			yield return new object[] { new SimpleTestObject(11), new SimpleTestObject(1), new SimpleTestObject(10), true };  // above range

			// Reversed order (value1 > value2)
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(10), new SimpleTestObject(1), false };  // inside reversed range
			yield return new object[] { new SimpleTestObject(1), new SimpleTestObject(10), new SimpleTestObject(1), false };  // on lower boundary (reversed)
			yield return new object[] { new SimpleTestObject(10), new SimpleTestObject(10), new SimpleTestObject(1), false }; // on upper boundary (reversed)
			yield return new object[] { new SimpleTestObject(0), new SimpleTestObject(10), new SimpleTestObject(1), true };   // below reversed range
			yield return new object[] { new SimpleTestObject(11), new SimpleTestObject(10), new SimpleTestObject(1), true };  // above reversed range

			// Null value input
			yield return new object[] { null, new SimpleTestObject(1), new SimpleTestObject(10), false };                     // null value
			yield return new object[] { new SimpleTestObject(5), null, new SimpleTestObject(10), false };                     // null value1
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(1), null, false };                      // null value2
			yield return new object[] { new SimpleTestObject(5), null, null, false };                                         // both value1 and value2 null
			yield return new object[] { null, null, new SimpleTestObject(10), false };                                        // value and value1 null
			yield return new object[] { null, new SimpleTestObject(1), null, false };                                         // value and value2 null
			yield return new object[] { null, null, null, false };                                                            // all null
		}

		/// <summary>
		/// Verifies that IsOutside returns true when a <see cref="SimpleTestObject" /> value lies outside the specified bounds.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsOutsideTestData), DynamicDataSourceType.Method)]
		public void IsOutside_WhenValueOutsideBounds_ShouldReturnTrue(SimpleTestObject? value, SimpleTestObject? lower, SimpleTestObject? upper, bool expected)
		{
			var actual = ComparableHelper.IsOutside(value, lower, upper);
			Assert.AreEqual(expected, actual);
		}
	}
}