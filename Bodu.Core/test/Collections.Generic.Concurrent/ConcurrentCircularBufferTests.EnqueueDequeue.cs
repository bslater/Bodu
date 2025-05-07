using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void EnqueueDequeue_WhenInterleavedConcurrently_ShouldNotCorruptState()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var output = new ConcurrentBag<int>();

			var task = Task.Run(() =>
			{
				for (int i = 0; i < 1000; i++)
				{
					buffer.TryEnqueue(new TestItem(i));
					if (buffer.TryDequeue(out var item) && item != null)
						output.Add(item.Value);
				}
			});

			task.Wait();

			Assert.IsTrue(output.All(v => v >= 0 && v < 1000));
			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= buffer.Capacity);
		}

		[TestMethod]
		public void EnqueueDequeue_WhenWraparoundStress_ShouldMaintainCorrectCount()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);

			Parallel.For(0, 1000, i =>
			{
				buffer.TryEnqueue(new TestItem(i));
				buffer.TryDequeue(out _);
			});

			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= 3);
		}

		[TestMethod]
		public void EnqueueDequeue_WhenConcurrent_ShouldRetainNullsSafely()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem?>(10);
			var nullCount = 0;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					buffer.Enqueue(i % 2 == 0 ? null : new TestItem(i)); // 50% nulls
				}
			});

			var reader = Task.Run(() =>
			{
				int attempts = 0;
				while (attempts < 150)
				{
					if (buffer.TryDequeue(out var item))
					{
						if (item == null)
							Interlocked.Increment(ref nullCount);
					}
					attempts++;
				}
			});

			Task.WaitAll(writer, reader);

			if (nullCount == 0)
			{
				// Retry by checking snapshot (in case nulls were still in buffer)
				nullCount = buffer.ToArray().Count(x => x is null);
			}

			Assert.IsTrue(nullCount > 0, "Expected to observe null entries.");
			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= buffer.Capacity);
		}

		[TestMethod]
		public void EnqueueDequeue_WhenRepeatedWraparound_ShouldNotLeakOrStall()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(4);

			for (int i = 0; i < 100; i++)
			{
				buffer.Enqueue(new TestItem(i));
				buffer.Dequeue();
				buffer.Enqueue(new TestItem(i + 100));
				buffer.Dequeue();
			}

			Assert.AreEqual(0, buffer.Count);
		}
	}
}