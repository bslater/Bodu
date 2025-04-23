namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that TryDequeue returns true and dequeues the oldest item when the buffer is
		/// not empty.
		/// </summary>
		[TestMethod]
		public void TryDequeue_WhenBufferIsNotEmpty_ShouldReturnTrueAndDequeueOldestItem()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(100);
			buffer.Enqueue(200);

			var success = buffer.TryDequeue(out int result);

			Assert.IsTrue(success);
			Assert.AreEqual(100, result);
			Assert.AreEqual(1, buffer.Count);
		}

		/// <summary>
		/// Verifies that TryDequeue returns the correct value after an overwrite has occurred.
		/// </summary>
		[TestMethod]
		public void TryDequeue_AfterOverwrite_ShouldReturnCorrectItem()
		{
			var buffer = new CircularBuffer<int>(2, allowOverwrite: true);
			buffer.TryEnqueue(1);
			buffer.TryEnqueue(2);
			buffer.TryEnqueue(3); // Overwrites 1

			var success = buffer.TryDequeue(out int value);

			Assert.IsTrue(success);
			Assert.AreEqual(2, value);
		}

		/// <summary>
		/// Verifies that TryDequeue returns false and outputs default value when the buffer is empty.
		/// </summary>
		[TestMethod]
		public void TryDequeue_WhenBufferIsEmpty_ShouldReturnFalseAndDefaultValue()
		{
			var buffer = new CircularBuffer<object>(1);

			var success = buffer.TryDequeue(out object result);

			Assert.IsFalse(success);
			Assert.IsNull(result);
		}

		/// <summary>
		/// Verifies that TryDequeue returns true and outputs null when null was enqueued.
		/// </summary>
		[TestMethod]
		public void TryDequeue_WhenNullWasEnqueued_ShouldReturnTrueAndOutputNull()
		{
			var buffer = new CircularBuffer<object>(1);
			buffer.Enqueue(null);

			var success = buffer.TryDequeue(out var result);

			Assert.IsTrue(success);
			Assert.IsNull(result);
		}

		/// <summary>
		/// Verifies that TryDequeue does not trigger any eviction events.
		/// </summary>
		[TestMethod]
		public void TryDequeue_WhenCalled_ShouldNotTriggerEvictionEvents()
		{
			var events = new List<string>();
			var buffer = new CircularBuffer<string>(2);

			buffer.TryEnqueue("A");
			buffer.TryEnqueue("B");

			buffer.ItemEvicting += item => events.Add("Evicting:" + item);
			buffer.ItemEvicted += item => events.Add("Evicted:" + item);

			var result = buffer.TryDequeue(out string item);

			Assert.IsTrue(result);
			Assert.AreEqual("A", item);
			CollectionAssert.AreEqual(Array.Empty<string>(), events);
			CollectionAssert.AreEqual(new[] { "B" }, buffer.ToArray());
		}
	}
}