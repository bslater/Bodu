using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void TryEnqueue_WhenBufferHasSpace_ShouldReturnTrue()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: false);
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(1)));
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(2)));
		}

		[TestMethod]
		public void TryEnqueue_WhenBufferFullAndOverwriteDisabled_ShouldReturnFalse()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: false);
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(1)));
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(2)));
			Assert.IsFalse(buffer.TryEnqueue(new TestItem(3)));
		}

		[TestMethod]
		public void TryEnqueue_WhenBufferFullAndOverwriteEnabled_ShouldReturnTrue()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(1)));
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(2)));
			Assert.IsTrue(buffer.TryEnqueue(new TestItem(3))); // Overwrites 1
		}

		[TestMethod]
		public void TryEnqueue_WhenCalledConcurrently_ShouldMaintainIntegrity()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(100, allowOverwrite: false);
			var successes = 0;

			Parallel.For(0, 150, i =>
			{
				if (buffer.TryEnqueue(new TestItem(i)))
					Interlocked.Increment(ref successes);
			});

			Assert.AreEqual(100, successes);
			Assert.AreEqual(100, buffer.Count);
		}

		[TestMethod]
		public void TryEnqueue_WhenOverwriteEnabledAndCalledConcurrently_ShouldAllowMoreThanCapacity()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10, allowOverwrite: true);

			Parallel.For(0, 50, i =>
			{
				buffer.TryEnqueue(new TestItem(i));
			});

			Assert.IsTrue(buffer.Count <= buffer.Capacity, $"Expected Count <= {buffer.Capacity}, got {buffer.Count}.");

			var snapshot = buffer.ToArray();

			Assert.IsTrue(snapshot.Length <= buffer.Capacity, $"ToArray exceeded capacity: {snapshot.Length}");
			Assert.IsTrue(snapshot.All(x => x is not null), "All items in buffer should be non-null.");
		}

		[TestMethod]
		public void TryEnqueue_WhenMultipleThreads_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(25);
			var exceptions = new ConcurrentBag<Exception>();

			var tasks = Enumerable.Range(0, 5).Select(t => Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					try
					{
						buffer.TryEnqueue(new TestItem(i + t * 10));
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
				}
			})).ToArray();

			Task.WaitAll(tasks);

			Assert.AreEqual(0, exceptions.Count);
			Assert.IsTrue(buffer.Count <= buffer.Capacity);
		}
	}
}