namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that ItemEvicted is raised with the correct item when an element is overwritten.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenOverwriteOccurs_ShouldContainCorrectItem()
		{
			var evictedItems = new List<string>();
			var buffer = new CircularBuffer<string>(2, allowOverwrite: true);
			buffer.ItemEvicted += item => evictedItems.Add(item);

			buffer.Enqueue("X");
			buffer.Enqueue("Y");
			buffer.Enqueue("Z"); // Should evict "X"

			CollectionAssert.AreEqual(new[] { "X" }, evictedItems);
		}

		/// <summary>
		/// Verifies that ItemEvicted is not raised when overwrite is not allowed.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenOverwriteIsDisabled_ShouldNotFire()
		{
			bool anyEventFired = false;
			var buffer = new CircularBuffer<int>(2, allowOverwrite: false);
			buffer.ItemEvicted += _ => anyEventFired = true;

			buffer.Enqueue(1);
			buffer.Enqueue(2);
			var success = buffer.TryEnqueue(3); // Should not overwrite or fire event

			Assert.IsFalse(success);
			Assert.IsFalse(anyEventFired);
		}

		/// <summary>
		/// Verifies that ItemEvicted is not raised when the buffer has capacity available.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenBufferHasCapacity_ShouldNotFire()
		{
			bool anyEventFired = false;
			var buffer = new CircularBuffer<int>(3, allowOverwrite: true);
			buffer.ItemEvicted += _ => anyEventFired = true;

			buffer.Enqueue(1);
			buffer.Enqueue(2); // Buffer not full

			Assert.IsFalse(anyEventFired);
		}

		/// <summary>
		/// Verifies that ItemEvicted handlers are invoked without exception under high concurrency.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenConcurrentOverwrite_ShouldNotThrow()
		{
			var buffer = new CircularBuffer<int>(10, allowOverwrite: true);
			var fired = 0;

			buffer.ItemEvicted += _ => Interlocked.Increment(ref fired);

			Parallel.For(0, 1000, i => buffer.Enqueue(i));

			Assert.IsTrue(fired > 0);
		}

		/// <summary>
		/// Verifies that an exception thrown in the ItemEvicted handler is propagated by the buffer.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenHandlerThrows_ShouldPropagateException()
		{
			var buffer = new CircularBuffer<string>(2, allowOverwrite: true);
			buffer.Enqueue("A");
			buffer.Enqueue("B");

			buffer.ItemEvicted += _ => throw new InvalidOperationException("Simulated error");

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				buffer.Enqueue("C"); // Overwrites "A" → triggers ItemEvicted
			});
		}

		/// <summary>
		/// Verifies that all registered ItemEvicted handlers are invoked.
		/// </summary>
		[TestMethod]
		public void ItemEvicted_WhenMultipleHandlers_ShouldInvokeAll()
		{
			var log = new List<string>();
			var buffer = new CircularBuffer<string>(2, allowOverwrite: true);

			buffer.ItemEvicted += item => log.Add("Handler1:" + item);
			buffer.ItemEvicted += item => log.Add("Handler2:" + item);

			buffer.Enqueue("X");
			buffer.Enqueue("Y");
			buffer.Enqueue("Z"); // Overwrite X

			CollectionAssert.AreEqual(new[] { "Handler1:X", "Handler2:X" }, log);
		}
	}
}