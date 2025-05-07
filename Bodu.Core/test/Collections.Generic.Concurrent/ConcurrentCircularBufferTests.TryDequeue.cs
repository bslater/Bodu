using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void TryDequeue_WhenSingleThreaded_ShouldReturnInFifoOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 3; i++)
				buffer.Enqueue(new TestItem(i));

			var results = new TestItem[3];
			for (int i = 0; i < 3; i++)
				Assert.IsTrue(buffer.TryDequeue(out results[i]));

			CollectionAssert.AreEqual(new[] { 0, 1, 2 }, results.Select(r => r.Value).ToArray());
		}

		[TestMethod]
		public void TryDequeue_WhenBufferIsEmpty_ShouldReturnFalse()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			Assert.IsFalse(buffer.TryDequeue(out _));
		}

		[TestMethod]
		public void TryDequeue_WhenCalledConcurrently_ShouldDrainBufferCorrectly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(20);
			for (int i = 0; i < 20; i++)
				buffer.Enqueue(new TestItem(i));

			var dequeued = new ConcurrentBag<int>();

			Parallel.For(0, 20, _ =>
			{
				if (buffer.TryDequeue(out var item) && item != null)
					dequeued.Add(item.Value);
			});

			Assert.AreEqual(20, dequeued.Count);
			var ordered = dequeued.OrderBy(x => x).ToArray();
			CollectionAssert.AreEquivalent(Enumerable.Range(0, 20).ToArray(), ordered);
		}

		[TestMethod]
		public void TryDequeue_WhenCalledWhileEnqueueing_ShouldRemainSafe()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var dequeuedCount = 0;
			var success = 0;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 20; i++)
				{
					buffer.Enqueue(new TestItem(i));
					Thread.Sleep(1); // Give room for interleaving
				}
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					if (buffer.TryDequeue(out var item) && item != null)
					{
						Interlocked.Increment(ref dequeuedCount);
						Interlocked.Exchange(ref success, 1);
					}
					Thread.Sleep(1);
				}
			});

			Task.WaitAll(writer, reader);

			Assert.IsTrue(success == 1, "TryDequeue never succeeded during concurrent enqueue.");
			Assert.IsTrue(dequeuedCount > 0, "Expected to dequeue at least one item.");
		}

		[TestMethod]
		public void TryDequeue_WhenMultipleThreadsRead_ShouldRemainConsistent()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(30);
			for (int i = 0; i < 30; i++) buffer.Enqueue(new TestItem(i));

			var dequeued = new ConcurrentBag<int>();
			var tasks = Enumerable.Range(0, 5).Select(_ => Task.Run(() =>
			{
				while (buffer.TryDequeue(out var item))
				{
					if (item != null)
						dequeued.Add(item.Value);
				}
			})).ToArray();

			Task.WaitAll(tasks);
			Assert.AreEqual(30, dequeued.Count);
			CollectionAssert.AreEquivalent(Enumerable.Range(0, 30).ToArray(), dequeued.OrderBy(x => x).ToArray());
		}
	}
}