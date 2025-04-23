using Bodu.Collections.Generic.Extensions;
using Bodu.Infrastructure;

namespace Bodu.Collections.Generic
{
	public partial class ShuffleHelpersTests
	{
		/// <summary>
		/// Verifies that ShuffleAndYield works correctly with buffers containing duplicate values using span input.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenBufferHasDuplicates_ShouldNotThrow_UsingSpan()
		{
			int[] buffer = new[] { 5, 5, 5, 5, 5 };
			var results = ShuffleHelpers.ShuffleAndYield<int>(buffer.AsSpan(), new XorShiftRandom(), 3).ToArray();

			Assert.AreEqual(3, results.Length);
			CollectionAssert.AreEqual(results, new[] { 5, 5, 5 });
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when the count exceeds the size of an empty array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenBufferIsEmptyAndCountIsOne_ShouldThrow_UsingArray()
		{
			int[] buffer = Array.Empty<int>();
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), 1).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns an empty result when the span is empty and the count is zero.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenBufferIsEmptyAndCountIsZero_ShouldReturnNothing_UsingSpan()
		{
			Span<int> span = Span<int>.Empty;
			var results = ShuffleHelpers.ShuffleAndYield<int>(span, new SystemRandomAdapter(), 0).ToArray();
			Assert.AreEqual(0, results.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield does not mutate the original array passed into the method.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCalled_ShouldNotModifySourceArray()
		{
			int[] original = Enumerable.Range(1, 100).ToArray();
			int[] snapshot = (int[])original.Clone();

			_ = ShuffleHelpers.ShuffleAndYield(original.ToArray(), new SystemRandomAdapter(), 50).ToArray();

			CollectionAssert.AreEqual(snapshot, original, "The source array was modified by ShuffleAndYield.");
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns all items in randomized order when count equals span length.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountEqualsLength_ShouldReturnAll_UsingSpan()
		{
			int[] buffer = Enumerable.Range(1, 5).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield<int>(buffer.AsSpan(), new XorShiftRandom(), buffer.Length).ToArray();

			CollectionAssert.AreEquivalent(buffer, results);
			Assert.AreEqual(buffer.Length, results.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when the requested count exceeds the buffer length.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountExceedsLength_ShouldThrowArgumentOutOfRange_UsingArray()
		{
			int[] buffer = Enumerable.Range(1, 10).ToArray();
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), 11).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when count is negative.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountNegative_ShouldThrow_UsingArray()
		{
			int[] buffer = new[] { 1, 2, 3 };
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), -1).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns an empty sequence when count is zero.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountZero_ShouldReturnEmpty_UsingArray()
		{
			int[] buffer = new[] { 1, 2, 3 };
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), 0).ToArray();

			Assert.AreEqual(0, results.Length);
		}

		/// <summary> Verifies that ShuffleAndYield returns the expected number of items when tested via DataRow using Span<T>. </summary>
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(5)]
		[DataRow(10)]
		public void ShuffleAndYield_WhenDataRowProvided_ShouldReturnExpectedCount_UsingSpan(int count)
		{
			int[] buffer = Enumerable.Range(1, 10).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield<int>(buffer.AsSpan(), new SystemRandomAdapter(), count).ToArray();

			Assert.AreEqual(count, results.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield on a large array returns only items from the original buffer.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenLargeBufferUsed_ShouldNot_UsingArray()
		{
			int[] buffer = Enumerable.Range(1, 10_000).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), 5000).ToArray();

			Assert.AreEqual(5000, results.Length);
			CollectionAssert.IsSubsetOf(results, buffer);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns the correct number of items from a large input array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenLargeBufferUsed_ShouldReturnCorrectCount_UsingArray()
		{
			int[] buffer = Enumerable.Range(1, 10_000).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), 5000).ToArray();

			Assert.AreEqual(5000, results.Length);
			CollectionAssert.IsSubsetOf(results, buffer);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns a randomized subset when a partial count is requested using an array.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenRequestedCountIsLessThanLength_ShouldReturnExpectedSubset_UsingArray()
		{
			int[] buffer = Enumerable.Range(1, 20).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), 10).ToArray();

			Assert.AreEqual(10, results.Length);
			CollectionAssert.AllItemsAreUnique(results);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns a subset when the count is less than the span length.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenRequestedCountIsPartial_ShouldReturnSubset_UsingSpan()
		{
			int[] buffer = Enumerable.Range(1, 20).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield<int>(buffer.AsSpan(), new XorShiftRandom(), 15).ToArray();

			Assert.AreEqual(15, results.Length);
			CollectionAssert.AllItemsAreUnique(results);
		}

		/*

		/// <summary>
		/// Verifies that ShuffleAndYield produces a randomized subset using a cryptographic random generator.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingCryptoRandom_ShouldReturnShuffledSubset_UsingArray()
		{
			using var rng = new CryptoRandom();
			int[] buffer = Enumerable.NextWhile(1, 20).ToArray();

			var results = ShuffleHelpers.ShuffleAndYield(buffer, rng, 10).ToArray();

			Assert.AreEqual(10, results.Length);
			CollectionAssert.AllItemsAreUnique(results);
		}
		*/

		/// <summary>
		/// Verifies that ShuffleAndYield returns a subset of string values from a buffer containing duplicates.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingStringBuffer_ShouldReturnSubset_UsingArray()
		{
			string[] buffer = new[] { "apple", "banana", "cherry", "apple", "date" };
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new SystemRandomAdapter(), 3).ToArray();

			Assert.AreEqual(3, results.Length);
			CollectionAssert.IsSubsetOf(results, buffer);
		}

		/// <summary> Verifies that ShuffleAndYield correctly returns a randomized subset when used with Memory<T> and SystemRandom. </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingSystemRandomAndPartialCount_ShouldReturnExpectedResult_UsingMemory()
		{
			int[] buffer = Enumerable.Range(1, 10).ToArray();
			var results = ShuffleHelpers.ShuffleAndYield(buffer.AsMemory(), new SystemRandomAdapter(), 5).ToArray();

			Assert.AreEqual(5, results.Length);
			CollectionAssert.AllItemsAreUnique(results);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns a randomized, unique subset of the specified count using an array input.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(ValidCounts), DynamicDataSourceType.Method)]
		public void ShuffleAndYield_WhenValidCount_ShouldReturnSubset_UsingArray(int[] buffer, int count)
		{
			var results = ShuffleHelpers.ShuffleAndYield(buffer, new XorShiftRandom(), count).ToArray();

			Assert.AreEqual(count, results.Length);
			CollectionAssert.AllItemsAreUnique(results);
			CollectionAssert.IsSubsetOf(results, buffer);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield does not enumerate the source until iteration begins.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingArrayInput_ShouldDeferExecution()
		{
			var input = new[] { 1, 2, 3 };
			AssertExecutionIsDeferred(
				methodName: "ShuffleAndYield",
				invokeExtensionMethod: source => ShuffleHelpers.ShuffleAndYield<int>(input, new XorShiftRandom(), 2),
				values: input);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield does not enumerate the source until iteration begins.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingEnumerableInput_ShouldDeferExecution()
		{
			IEnumerable<int> source = new[] { 1, 2, 3 };
			AssertExecutionIsDeferred(
				methodName: "ShuffleAndYield",
				invokeExtensionMethod: source => ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 2),
				values: source);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns a subset from an enumerable with duplicate values.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenSourceHasDuplicates_ShouldReturnExpected_UsingEnumerable()
		{
			IEnumerable<int> source = new[] { 5, 5, 5, 5, 5 };
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 3).ToArray();

			Assert.AreEqual(3, result.Length);
			CollectionAssert.AreEqual(new[] { 5, 5, 5 }, result);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws an ArgumentOutOfRangeException when the count exceeds the available items in an empty enumerable.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenSourceIsEmptyAndCountIsOne_ShouldThrow_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Empty<int>();
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 1).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns an empty result when source is empty and count is zero.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenSourceIsEmptyAndCountIsZero_ShouldReturnEmpty_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Empty<int>();
			var result = ShuffleHelpers.ShuffleAndYield(source, new SystemRandomAdapter(), 0).ToArray();
			Assert.AreEqual(0, result.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns all items in randomized order when count equals source length.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountEqualsLength_ShouldReturnAll_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Range(1, 5);
			var result = ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 5).ToArray();

			CollectionAssert.AreEquivalent(source.ToArray(), result);
			Assert.AreEqual(5, result.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws when count exceeds the number of items.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountExceedsLength_ShouldThrow_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Range(1, 5);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(source, new XorShiftRandom(), 6).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield throws when count is negative.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountIsNegative_ShouldThrow_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Range(1, 3);
			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
				ShuffleHelpers.ShuffleAndYield(source, new SystemRandomAdapter(), -1).ToArray());
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns an empty sequence when count is zero.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenCountIsZero_ShouldReturnEmpty_UsingEnumerable()
		{
			IEnumerable<int> source = Enumerable.Range(1, 3);
			var result = ShuffleHelpers.ShuffleAndYield(source, new SystemRandomAdapter(), 0).ToArray();
			Assert.AreEqual(0, result.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield returns the correct number of items for multiple valid counts.
		/// </summary>
		[DataTestMethod]
		[DataRow(0)]
		[DataRow(1)]
		[DataRow(5)]
		[DataRow(10)]
		public void ShuffleAndYield_WhenValidCountsProvided_ShouldReturnCorrectCount_UsingEnumerable(int count)
		{
			IEnumerable<int> source = Enumerable.Range(1, 10);
			var result = ShuffleHelpers.ShuffleAndYield(source, new SystemRandomAdapter(), count).ToArray();
			Assert.AreEqual(count, result.Length);
		}

		/// <summary>
		/// Verifies that ShuffleAndYield does not enumerate the source until iteration begins.
		/// </summary>
		[TestMethod]
		public void ShuffleAndYield_WhenUsingEnumerable_ShouldDeferExecution()
		{
			bool hasEnumerated = false;

			var tracked = new TrackingEnumerable<int>(
				source: new[] { 1, 2, 3 },
				onEnumerate: () => hasEnumerated = true
			);

			var result = ShuffleHelpers.ShuffleAndYield((IEnumerable<int>)tracked, new XorShiftRandom(), 2);

			Assert.IsFalse(hasEnumerated, "ShuffleAndYield should defer enumeration until iterated.");
			_ = result.First();
			Assert.IsTrue(hasEnumerated, "ShuffleAndYield should enumerate only on iteration.");
		}
	}
}