namespace Bodu.Extensions
{
	public partial class ComparableHelperTests
	{
		/// <summary>
		/// Provides test data for verifying <see cref="ComparableHelper.IsBetween{T}(T, T, T)" />.
		/// </summary>
		private static IEnumerable<object[]> GetIsBetweenTestData()
		{
			// Normal order (value1 < value2)
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(1), new SimpleTestObject(10), true };   // inside range
			yield return new object[] { new SimpleTestObject(1), new SimpleTestObject(1), new SimpleTestObject(10), true };   // on lower boundary
			yield return new object[] { new SimpleTestObject(10), new SimpleTestObject(1), new SimpleTestObject(10), true };  // on upper boundary
			yield return new object[] { new SimpleTestObject(0), new SimpleTestObject(1), new SimpleTestObject(10), false };  // below range
			yield return new object[] { new SimpleTestObject(11), new SimpleTestObject(1), new SimpleTestObject(10), false }; // above range

			// Reversed order (value1 > value2)
			yield return new object[] { new SimpleTestObject(5), new SimpleTestObject(10), new SimpleTestObject(1), true };   // inside reversed range
			yield return new object[] { new SimpleTestObject(1), new SimpleTestObject(10), new SimpleTestObject(1), true };   // on lower boundary (reversed)
			yield return new object[] { new SimpleTestObject(10), new SimpleTestObject(10), new SimpleTestObject(1), true };  // on upper boundary (reversed)
			yield return new object[] { new SimpleTestObject(0), new SimpleTestObject(10), new SimpleTestObject(1), false };  // below reversed range
			yield return new object[] { new SimpleTestObject(11), new SimpleTestObject(10), new SimpleTestObject(1), false }; // above reversed range

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
		/// Verifies that IsBetween returns true when a <see cref="SimpleTestObject" /> value lies within the specified bounds.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetIsBetweenTestData), DynamicDataSourceType.Method)]
		public void IsBetween_WhenValueWithinBounds_ShouldReturnTrue(SimpleTestObject? value, SimpleTestObject? lower, SimpleTestObject? upper, bool expected)
		{
			var result = ComparableHelper.IsBetween(value, lower, upper);
			Assert.AreEqual(expected, result);
		}
	}
}