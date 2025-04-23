namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that <see cref="CircularBuffer{T}.TryEnqueue" /> returns false when the buffer is full and overwriting is disabled.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenBufferIsFullAndOverwriteIsDisabled_ShouldReturnFalse()
		{
			var buffer = new CircularBuffer<string>(capacity: 2, allowOverwrite: false);
			buffer.Enqueue("A");
			buffer.Enqueue("B");

			bool success = buffer.TryEnqueue("C");

			Assert.IsFalse(success);
			Assert.AreEqual(2, buffer.Count);
		}

		/// <summary>
		/// Verifies that TryEnqueue does not modify the buffer when it is full and overwrite is disabled.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenFullAndOverwriteDisabled_ShouldNotModifyBuffer()
		{
			var buffer = new CircularBuffer<int>(2, allowOverwrite: false);
			buffer.TryEnqueue(1);
			buffer.TryEnqueue(2);

			var result = buffer.TryEnqueue(3);

			Assert.IsFalse(result);
			CollectionAssert.AreEqual(new[] { 1, 2 }, buffer.ToArray());
		}

		/// <summary>
		/// Verifies that TryEnqueue returns true and increases the buffer count when space is available.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenBufferHasSpace_ShouldReturnTrueAndIncreaseCount()
		{
			var buffer = new CircularBuffer<int>(3);

			var result = buffer.TryEnqueue(42);

			Assert.IsTrue(result);
			Assert.AreEqual(1, buffer.Count);
			Assert.AreEqual(42, buffer.Peek());
		}

		/// <summary>
		/// Verifies that TryEnqueue returns false when buffer is full and overwrite is disabled.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenFullAndOverwriteDisabled_ShouldReturnFalse()
		{
			var buffer = new CircularBuffer<int>(2, allowOverwrite: false);
			buffer.TryEnqueue(1);
			buffer.TryEnqueue(2);

			var result = buffer.TryEnqueue(3);

			Assert.IsFalse(result);
			Assert.AreEqual(2, buffer.Count);
			CollectionAssert.AreEqual(new[] { 1, 2 }, buffer.ToArray());
		}

		/// <summary>
		/// Verifies that TryEnqueue overwrites the oldest item and returns true when full and overwrite is enabled.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenFullAndOverwriteEnabled_ShouldOverwriteAndReturnTrue()
		{
			var buffer = new CircularBuffer<int>(2, allowOverwrite: true);
			buffer.TryEnqueue(1);
			buffer.TryEnqueue(2);

			var result = buffer.TryEnqueue(3);

			Assert.IsTrue(result);
			Assert.AreEqual(2, buffer.Count);
			CollectionAssert.AreEqual(new[] { 2, 3 }, buffer.ToArray());
		}

		/// <summary>
		/// Verifies that TryEnqueue triggers ItemEvicting and ItemEvicted events in the correct order when overwriting.
		/// </summary>
		[TestMethod]
		public void TryEnqueue_WhenFullAndOverwriteEnabled_ShouldTriggerEvictionEventsInCorrectOrder()
		{
			var events = new List<string>();
			var buffer = new CircularBuffer<string>(2, allowOverwrite: true);

			buffer.TryEnqueue("A");
			buffer.TryEnqueue("B");

			buffer.ItemEvicting += item => events.Add("Evicting:" + item);
			buffer.ItemEvicted += item => events.Add("Evicted:" + item);

			var result = buffer.TryEnqueue("C"); // should evict "A"

			Assert.IsTrue(result);
			CollectionAssert.AreEqual(new[] { "Evicting:A", "Evicted:A" }, events);
			CollectionAssert.AreEqual(new[] { "B", "C" }, buffer.ToArray());
		}
	}
}