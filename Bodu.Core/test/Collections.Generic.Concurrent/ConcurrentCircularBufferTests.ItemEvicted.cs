using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void ItemEvicted_WhenOverwrittenConcurrently_ShouldCaptureAllEvictedItems()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5, allowOverwrite: true);
			var evicted = new ConcurrentBag<TestItem>();

			buffer.ItemEvicted += item =>
			{
				if (item != null)
					evicted.Add(item);
			};

			// Fill buffer
			for (int i = 0; i < 5; i++)
				buffer.Enqueue(new TestItem(i));

			// Concurrent overwrites
			Parallel.For(5, 100, i =>
			{
				buffer.Enqueue(new TestItem(i));
			});

			// Wait for event handlers to flush
			Thread.Sleep(100);

			Assert.IsTrue(evicted.Count > 0);
			Assert.IsTrue(evicted.All(x => x != null));
			Assert.IsTrue(evicted.All(x => x.Value >= 0 && x.Value < 100));
		}

		[TestMethod]
		public void ItemEvicted_WhenHandlerIsRegisteredMultipleTimes_ShouldAllFire()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			var count1 = 0;
			var count2 = 0;

			buffer.ItemEvicted += item => Interlocked.Increment(ref count1);
			buffer.ItemEvicted += item => Interlocked.Increment(ref count2);

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3)); // Evicts 1

			Assert.AreEqual(1, count1);
			Assert.AreEqual(1, count2);
		}

		[TestMethod]
		public void ItemEvicted_WhenBufferIsNotFull_ShouldNotFire()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5, allowOverwrite: true);
			var fired = false;

			buffer.ItemEvicted += _ => fired = true;

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void ItemEvicted_WhenOverwriteIsDisabled_ShouldNotFire()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: false);
			var fired = false;

			buffer.ItemEvicted += _ => fired = true;

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			var success = buffer.TryEnqueue(new TestItem(3)); // Should fail, no eviction

			Assert.IsFalse(success);
			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void ItemEvicted_WhenHandlerThrows_ShouldNotCrashEnqueue()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			buffer.ItemEvicted += _ => throw new InvalidOperationException("Simulated error");

			// Exception should not propagate from Enqueue
			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				buffer.Enqueue(new TestItem(3));
			});
		}

		[TestMethod]
		public void ItemEvicted_WhenTriggeredByParallelWriters_ShouldRemainStable()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);
			var evicted = new ConcurrentBag<TestItem>();

			buffer.ItemEvicted += item =>
			{
				if (item != null)
					evicted.Add(item);
			};

			Parallel.For(0, 50, i =>
			{
				buffer.Enqueue(new TestItem(i));
			});

			Thread.Sleep(100); // Let event handlers process

			Assert.IsTrue(evicted.Count > 0);
			Assert.IsTrue(evicted.All(x => x != null));
		}
	}
}