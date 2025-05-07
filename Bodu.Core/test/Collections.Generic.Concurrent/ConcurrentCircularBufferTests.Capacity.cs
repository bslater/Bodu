using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Capacity_WhenConstructed_ShouldReturnDefinedSize()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			Assert.AreEqual(10, buffer.Capacity);
		}

		[TestMethod]
		public void Capacity_WhenAccessedConcurrently_ShouldAlwaysReturnConsistentValue()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(32);
			var results = new int[1000];

			Parallel.For(0, 1000, i =>
			{
				results[i] = buffer.Capacity;
			});

			Assert.IsTrue(results.All(r => r == 32), "All reads of Capacity should return 32.");
		}

		[TestMethod]
		public void Capacity_WhenFilledAndCleared_ShouldRemainUnchanged()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);

			for (int i = 0; i < 5; i++)
				buffer.Enqueue(new TestItem(i));

			buffer.Clear();

			Assert.AreEqual(5, buffer.Capacity);
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void Capacity_WhenConcurrentEnqueueDequeue_ShouldRemainStable()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(8);

			Parallel.Invoke(
				() =>
				{
					for (int i = 0; i < 100; i++)
					{
						buffer.TryEnqueue(new TestItem(i));
						Thread.SpinWait(100);
					}
				},
				() =>
				{
					for (int i = 0; i < 100; i++)
					{
						buffer.TryDequeue(out _);
						Thread.SpinWait(100);
					}
				}
			);

			Assert.AreEqual(8, buffer.Capacity, "Capacity must remain fixed despite concurrent mutation.");
		}

		[TestMethod]
		public void Capacity_WhenTrimExcessCalled_ShouldMatchElementCount()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			buffer.TrimExcess();

			Assert.AreEqual(2, buffer.Capacity);
			Assert.AreEqual(2, buffer.Count);
		}

		[TestMethod]
		public void Capacity_WhenTrimExcessOnEmpty_ShouldSetMinimum()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			buffer.Clear();

			buffer.TrimExcess();

			Assert.AreEqual(1, buffer.Capacity, "Capacity should be reduced to at least 1.");
		}
	}
}