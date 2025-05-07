using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void IEnumerable_WhenBufferIsEnumerated_ShouldYieldAllItemsInOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));

			var values = buffer.Select(x => x?.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 1, 2, 3 }, values);
		}

		[TestMethod]
		public void IEnumerable_WhenBufferContainsNull_ShouldIncludeNull()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem?>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(null);
			buffer.Enqueue(new TestItem(3));

			var items = buffer.ToArray();
			Assert.AreEqual(3, items.Length);
			Assert.IsTrue(items.Any(i => i is null));
		}

		[TestMethod]
		public void IEnumerable_WhenBufferIsEmpty_ShouldYieldNothing()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			var result = buffer.ToArray();
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void IEnumerable_WhenEnumeratedDuringEnqueue_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(50);
			var snapshotLengths = new ConcurrentBag<int>();

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
					buffer.Enqueue(new TestItem(i));
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					var snapshot = buffer.ToArray();
					snapshotLengths.Add(snapshot.Length);
					Thread.SpinWait(10);
				}
			});

			Task.WaitAll(writer, reader);
			Assert.IsTrue(snapshotLengths.All(len => len <= buffer.Capacity));
		}

		[TestMethod]
		public void IEnumerable_WhenEnumeratedDuringDequeue_ShouldYieldStableSnapshot()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var exceptionThrown = false;

			var dequeuer = Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
					buffer.TryDequeue(out _);
			});

			var enumerator = Task.Run(() =>
			{
				try
				{
					var snapshot = buffer.ToArray();
					Assert.IsTrue(snapshot.Length <= buffer.Capacity);
				}
				catch
				{
					exceptionThrown = true;
				}
			});

			Task.WaitAll(dequeuer, enumerator);
			Assert.IsFalse(exceptionThrown, "Enumeration threw during concurrent dequeue.");
		}
	}
}