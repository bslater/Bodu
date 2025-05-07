using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Dequeue_WhenConcurrentEnqueueDequeue_ShouldPreserveFifo()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(100);
			var dequeued = new ConcurrentQueue<TestItem>();

			var enqueuer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					buffer.Enqueue(new TestItem(i));
					Thread.SpinWait(10);
				}
			});

			var dequeuer = Task.Run(() =>
			{
				int count = 0;
				while (count < 100)
				{
					if (buffer.TryDequeue(out var item))
					{
						dequeued.Enqueue(item);
						count++;
					}
					Thread.SpinWait(5);
				}
			});

			Task.WaitAll(enqueuer, dequeuer);

			var result = dequeued.Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(Enumerable.Range(0, 100).ToArray(), result);
		}

		[TestMethod]
		public void Dequeue_WhenNullWasEnqueued_ShouldReturnNull()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem?>(2);
			buffer.Enqueue(null);

			var result = buffer.Dequeue();
			Assert.IsNull(result);
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void Dequeue_WhenWraparoundOccurs_ShouldPreserveFifo()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));
			buffer.Dequeue(); // evict 1
			buffer.Enqueue(new TestItem(4));

			Assert.AreEqual(2, buffer.Dequeue().Value);
			Assert.AreEqual(3, buffer.Dequeue().Value);
			Assert.AreEqual(4, buffer.Dequeue().Value);
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void Dequeue_WhenDrainedConcurrently_ShouldReturnAllItems()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var source = Enumerable.Range(1, 5).Select(i => new TestItem(i)).ToArray();

			foreach (var item in source)
				buffer.Enqueue(item);

			var dequeued = new ConcurrentBag<TestItem>();

			Parallel.For(0, source.Length, _ =>
			{
				if (buffer.TryDequeue(out var item) && item != null)
					dequeued.Add(item);
			});

			Assert.AreEqual(source.Length, dequeued.Count);
			CollectionAssert.AreEquivalent(source.Select(i => i.Value).ToArray(), dequeued.Select(i => i.Value).ToArray());
		}

		[TestMethod]
		public void Dequeue_WhenBufferIsEmpty_ShouldThrowExactly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2);
			Assert.ThrowsException<InvalidOperationException>(() => buffer.Dequeue());
		}

		[TestMethod]
		public void Dequeue_WhenSlotIsReused_ShouldReturnExpectedValues()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2);
			buffer.Enqueue(new TestItem(10));
			buffer.Dequeue();       // Slot 0 freed
			buffer.Enqueue(new TestItem(20));
			buffer.Enqueue(new TestItem(30));     // Wraparound

			Assert.AreEqual(20, buffer.Dequeue().Value);
			Assert.AreEqual(30, buffer.Dequeue().Value);
			Assert.AreEqual(0, buffer.Count);
		}
	}
}