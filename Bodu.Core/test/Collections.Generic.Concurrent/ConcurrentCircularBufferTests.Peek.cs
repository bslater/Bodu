using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Peek_WhenBufferHasItems_ShouldReturnOldestWithoutRemoving()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(100));

			var peeked = buffer.Peek();
			Assert.AreEqual(100, peeked.Value);
			Assert.AreEqual(1, buffer.Count);
		}

		[TestMethod]
		public void Peek_WhenBufferHasWrapped_ShouldReturnOldestItem()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));
			buffer.Dequeue(); // Remove 1
			buffer.Enqueue(new TestItem(4)); // Wraparound

			var peeked = buffer.Peek();
			Assert.AreEqual(2, peeked.Value);
		}

		[TestMethod]
		public void Peek_WhenBufferIsEmpty_ShouldThrowExactly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);

			Assert.ThrowsException<InvalidOperationException>(() =>
			{
				_ = buffer.Peek();
			});
		}

		[TestMethod]
		public void Peek_WhenReadDuringConcurrentEnqueue_ShouldRemainSafe()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			var peekReader = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var item = buffer.Peek();
						Assert.IsNotNull(item);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
				}
			});

			var writer = Task.Run(() =>
			{
				for (int i = 10; i < 110; i++)
					buffer.Enqueue(new TestItem(i));
			});

			Task.WaitAll(peekReader, writer);
			Assert.AreEqual(0, exceptions.Count, "Peek threw during concurrent mutation.");
		}

		[TestMethod]
		public void Peek_WhenCalledDuringDraining_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);

			// Preload
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var success = 0;
			var peekFailures = 0;

			var peeker = Task.Run(() =>
			{
				for (int attempts = 0; attempts < 500 && Volatile.Read(ref success) == 0; attempts++)
				{
					try
					{
						var _ = buffer.Peek();
						Interlocked.Exchange(ref success, 1);
					}
					catch (InvalidOperationException)
					{
						Interlocked.Increment(ref peekFailures);
					}
					Thread.SpinWait(50);
				}
			});

			var consumer = Task.Run(() =>
			{
				while (buffer.TryDequeue(out _)) { }
			});

			Task.WaitAll(peeker, consumer);

			Assert.IsTrue(success == 1, "Peek never succeeded before buffer was emptied.");
		}
	}
}