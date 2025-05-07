using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void ItemEvicting_WhenOverwritingConcurrently_ShouldFireForEvictedItems()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5, allowOverwrite: true);
			var evictingItems = new ConcurrentBag<TestItem>();

			buffer.ItemEvicting += item =>
			{
				if (item != null)
					evictingItems.Add(item);
			};

			for (int i = 0; i < 5; i++)
				buffer.Enqueue(new TestItem(i)); // Pre-fill

			Parallel.For(5, 100, i =>
			{
				buffer.Enqueue(new TestItem(i)); // Should trigger evictions
			});

			Thread.Sleep(100);
			Assert.IsTrue(evictingItems.Count > 0);
			Assert.IsTrue(evictingItems.All(i => i != null));
			Assert.IsTrue(evictingItems.All(i => i.Value < 100));
		}

		[TestMethod]
		public void ItemEvicting_WhenMultipleHandlersAttached_ShouldAllBeInvoked()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			int count1 = 0;
			int count2 = 0;

			buffer.ItemEvicting += _ => Interlocked.Increment(ref count1);
			buffer.ItemEvicting += _ => Interlocked.Increment(ref count2);

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3)); // Should evict 1

			Assert.AreEqual(1, count1);
			Assert.AreEqual(1, count2);
		}

		[TestMethod]
		public void ItemEvicting_WhenOverwriteDisabled_ShouldNotFire()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: false);
			bool fired = false;

			buffer.ItemEvicting += _ => fired = true;

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.TryEnqueue(new TestItem(3)); // No overwrite

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void ItemEvicting_WhenBufferHasCapacity_ShouldNotFire()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);
			bool fired = false;

			buffer.ItemEvicting += _ => fired = true;

			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			Assert.IsFalse(fired);
		}

		[TestMethod]
		public void ItemEvicting_WhenHandlerThrows_ShouldPropagateException()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			buffer.ItemEvicting += _ => throw new InvalidOperationException("Simulated failure");

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				buffer.Enqueue(new TestItem(3)); // Triggers eviction
			});
		}

		[TestMethod]
		public void ItemEvicting_WhenTriggeredByParallelWriters_ShouldRemainStable()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);
			var captured = new ConcurrentBag<int>();

			buffer.ItemEvicting += item =>
			{
				if (item != null)
					captured.Add(item.Value);
			};

			Parallel.For(0, 50, i =>
			{
				buffer.Enqueue(new TestItem(i));
			});

			Thread.Sleep(100);
			Assert.IsTrue(captured.Count > 0);
			Assert.IsTrue(captured.All(v => v >= 0 && v < 50));
		}
	}
}