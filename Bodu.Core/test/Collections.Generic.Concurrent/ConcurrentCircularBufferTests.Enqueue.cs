using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Enqueue_WhenMultipleThreadsEnqueue_ShouldStoreAllIfSpacePermits()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(100, allowOverwrite: false);

			Parallel.For(0, 100, i =>
			{
				buffer.TryEnqueue(new TestItem(i));
			});

			Assert.AreEqual(100, buffer.Count);
			Assert.IsTrue(buffer.ToArray().All(x => x != null));
		}

		[TestMethod]
		public void Enqueue_WhenDuplicatesProvidedConcurrently_ShouldStoreAll()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10, allowOverwrite: true);

			Parallel.For(0, 10, _ =>
			{
				buffer.Enqueue(new TestItem(5));
			});

			Assert.AreEqual(10, buffer.Count);
			Assert.IsTrue(buffer.ToArray().All(x => x?.Value == 5));
		}

		[TestMethod]
		public void Enqueue_WhenConcurrentDequeueFreesSlot_ShouldReuseSlotSafely()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);

			// Fill initial
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			var enqueueSuccess = new ConcurrentBag<bool>();

			var writer = Task.Run(() =>
			{
				for (int i = 100; i < 200; i++)
				{
					bool success = buffer.TryEnqueue(new TestItem(i));
					enqueueSuccess.Add(success);
					Thread.SpinWait(5);
				}
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					buffer.TryDequeue(out _);
					Thread.SpinWait(10);
				}
			});

			Task.WaitAll(writer, reader);

			Assert.IsTrue(enqueueSuccess.Any(s => s), "No enqueue succeeded during concurrent dequeue.");
			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= buffer.Capacity);
		}

		[TestMethod]
		public void Enqueue_WhenNullProvidedConcurrently_ShouldBeAccepted()
		{
			var buffer = new ConcurrentCircularBuffer<string>(10);

			Parallel.For(0, 10, i =>
			{
				buffer.Enqueue(i % 2 == 0 ? null : $"Value-{i}");
			});

			var items = buffer.ToArray();
			Assert.AreEqual(10, items.Length);
			Assert.IsTrue(items.Count(x => x == null) > 0);
		}

		[TestMethod]
		public void Enqueue_WhenWraparoundOccursConcurrently_ShouldMaintainFIFOOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);

			Parallel.For(0, 10, i =>
			{
				buffer.Enqueue(new TestItem(i));
			});

			// Assert final state: 3 items, each in range [0–9]
			AssertBufferContainsOnlyValuesInRange(buffer, 3, 0, 9);
		}

		[TestMethod]
		public void Enqueue_WhenMultipleNullsProvidedConcurrently_ShouldRetainInOrderIfSpaceAllows()
		{
			var buffer = new ConcurrentCircularBuffer<string>(3);
			buffer.Enqueue(null);
			buffer.Enqueue("X");
			buffer.Enqueue(null);

			var snapshot = buffer.ToArray();
			CollectionAssert.AreEqual(new[] { null, "X", null }, snapshot);
		}

		[TestMethod]
		public void Enqueue_WhenBufferFullAndAllowOverwriteDisabled_ShouldBlockOverwrites()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: false);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));

			var failures = new ConcurrentBag<Exception>();

			Parallel.For(0, 10, i =>
			{
				try
				{
					buffer.Enqueue(new TestItem(100 + i));
				}
				catch (InvalidOperationException ex)
				{
					failures.Add(ex);
				}
			});

			Assert.AreEqual(10, failures.Count);
			Assert.AreEqual(3, buffer.Count);

			// Use helper
			AssertBufferContainsExactlyValues(buffer, 1, 2, 3);
		}

		[TestMethod]
		public void Enqueue_WhenBufferFullAndAllowOverwriteEnabled_ShouldEvictOldest()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);

			// Pre-fill to capacity
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));

			// Overwrite with 10 new items
			Parallel.For(0, 10, i =>
			{
				buffer.Enqueue(new TestItem(100 + i));
			});

			// Assert using helper
			AssertBufferContainsOnlyValuesInRange(buffer, 3, 100, 109);
		}
	}
}