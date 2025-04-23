using Bodu.Collections.Generic.Extensions;

namespace Bodu.Collections.Generic
{
	public partial class ShuffleHelpersTests
	{
		/// <summary>
		/// Verifies that shuffling a span with a single item results in no change.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenBufferHasOneItem_ShouldReturnSameItem_UsingSpan()
		{
			int[] buffer = new[] { 42 };
			var original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer.AsSpan(), new SystemRandomAdapter());

			CollectionAssert.AreEqual(original, buffer);
		}

		/// <summary>
		/// Verifies that shuffling a span with multiple items mutates the original order.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenCalled_ShouldMutateOriginalOrder_UsingSpan()
		{
			int[] buffer = Enumerable.Range(1, 20).ToArray();
			var original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer.AsSpan(), new SystemRandomAdapter());

			bool changed = !buffer.SequenceEqual(original);
			Assert.IsTrue(changed, "Expected order change, but got same result (may occasionally fail due to luck).");
		}

		/// <summary>
		/// Verifies that shuffling an empty span does not throw an exception.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenSpanIsEmpty_ShouldNotThrow_UsingSpan()
		{
			Span<int> span = Span<int>.Empty;
			ShuffleHelpers.Shuffle(span, new SystemRandomAdapter());
		}

		/// <summary>
		/// Verifies that shuffling a full array using SystemRandom changes the order of elements.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenSystemRandomUsed_ShouldChangeItemOrder_UsingArray()
		{
			int[] buffer = Enumerable.Range(1, 100).ToArray();
			int[] original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer, new SystemRandomAdapter());

			bool orderChanged = !buffer.SequenceEqual(original);
			Assert.IsTrue(orderChanged, "Shuffle did not change order (may occasionally fail).");
		}

		/// <summary>
		/// Verifies that shuffling a span using SystemRandom changes the order of elements.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenSystemRandomUsed_ShouldChangeItemOrder_UsingSpan()
		{
			int[] buffer = Enumerable.Range(1, 100).ToArray();
			int[] original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer.AsSpan(), new SystemRandomAdapter());

			bool orderChanged = !buffer.SequenceEqual(original);
			Assert.IsTrue(orderChanged, "Shuffle did not change order (may occasionally fail).");
		}

		/// <summary> Verifies that shuffling a Memory<T> retains all original elements. </summary>
		[TestMethod]
		public void Shuffle_WhenSystemRandomUsed_ShouldContainSameElements_UsingMemory()
		{
			int[] buffer = Enumerable.Range(1, 50).ToArray();
			int[] original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer.AsMemory(), new SystemRandomAdapter());

			CollectionAssert.AreEquivalent(original, buffer);
		}

		/// <summary>
		/// Verifies that shuffling a span with SystemRandom retains all original elements.
		/// </summary>
		[TestMethod]
		public void Shuffle_WhenSystemRandomUsed_ShouldContainSameElements_UsingSpan()
		{
			int[] buffer = Enumerable.Range(1, 100).ToArray();
			var original = buffer.ToArray();

			ShuffleHelpers.Shuffle(buffer.AsSpan(), new SystemRandomAdapter());

			CollectionAssert.AreEquivalent(original, buffer);
		}
	}
}