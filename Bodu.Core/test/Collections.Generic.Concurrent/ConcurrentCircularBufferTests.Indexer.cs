using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Indexer_WhenAccessed_ShouldReturnExpectedValues()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(10));
			buffer.Enqueue(new TestItem(20));
			buffer.Enqueue(new TestItem(30));

			Assert.AreEqual(10, buffer[0].Value);
			Assert.AreEqual(20, buffer[1].Value);
			Assert.AreEqual(30, buffer[2].Value);
		}

		[TestMethod]
		public void Indexer_WhenBufferIsAccessedConcurrently_ShouldRemainStable()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			Parallel.For(0, 10, i =>
			{
				try
				{
					var item = buffer[i];
					Assert.IsNotNull(item);
					Assert.AreEqual(i, item.Value);
				}
				catch (Exception ex)
				{
					exceptions.Add(ex);
				}
			});

			Assert.AreEqual(0, exceptions.Count);
		}

		[TestMethod]
		public void Indexer_WhenReadDuringEnqueue_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						_ = buffer[0];
						Thread.SpinWait(5);
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
				{
					buffer.TryEnqueue(new TestItem(i));
					Thread.SpinWait(5);
				}
			});

			Task.WaitAll(reader, writer);
			Assert.AreEqual(0, exceptions.Count, "Indexer threw during concurrent enqueue.");
		}

		[DataTestMethod]
		[DataRow(-1)]
		[DataRow(1)]
		public void Indexer_WhenAccessingInvalidIndex_ShouldThrow(int index)
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));

			Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
			{
				var _ = buffer[index];
			});
		}

		[TestMethod]
		public void Indexer_WhenAccessingNull_ShouldReturnNull()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem?>(2);
			buffer.Enqueue(null);
			buffer.Enqueue(new TestItem(1));

			Assert.IsNull(buffer[0]);
			Assert.AreEqual(1, buffer[1]!.Value);
		}
	}
}