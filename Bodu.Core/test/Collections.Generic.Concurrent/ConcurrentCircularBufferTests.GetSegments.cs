using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void GetSegments_WhenBufferIsEmpty_ShouldReturnTwoEmptySegments()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			var (first, second) = buffer.GetSegments();

			Assert.AreEqual(0, first.Count);
			Assert.AreEqual(0, second.Count);
		}

		[TestMethod]
		public void GetSegments_WhenBufferIsContiguous_ShouldReturnSingleSegment()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));

			var (first, second) = buffer.GetSegments();
			Assert.AreEqual(buffer.Count, first.Count + second.Count);
			Assert.AreEqual(2, first.Count);
			Assert.AreEqual(0, second.Count);
		}

		[TestMethod]
		public void GetSegments_WhenBufferIsWrapped_ShouldReturnTwoSegments()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(4, allowOverwrite: true);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));
			buffer.Dequeue(); // Head++
			buffer.Enqueue(new TestItem(4));
			buffer.Enqueue(new TestItem(5)); // Wrap

			var (first, second) = buffer.GetSegments();

			Assert.IsTrue(first.Count > 0);
			Assert.IsTrue(second.Count > 0);
			Assert.AreEqual(buffer.Count, first.Count + second.Count);
		}

		[TestMethod]
		public void GetSegments_WhenCalledDuringConcurrentEnqueue_ShouldYieldValidSnapshot()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var snapshotCount = 0;

			var enqueuer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
					buffer.TryEnqueue(new TestItem(i));
			});

			var segmentReader = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					var (first, second) = buffer.GetSegments();
					Assert.IsTrue(first.Count + second.Count <= buffer.Capacity);
					snapshotCount++;
					Thread.SpinWait(10);
				}
			});

			Task.WaitAll(enqueuer, segmentReader);
			Assert.IsTrue(snapshotCount > 0);
		}

		[TestMethod]
		public void GetSegments_WhenCalledDuringConcurrentDequeue_ShouldNotThrowOrCorrupt()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var errors = new ConcurrentBag<Exception>();

			var dequeuer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
					buffer.TryDequeue(out _);
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					try
					{
						var (first, second) = buffer.GetSegments();
						Assert.IsTrue(first.Count + second.Count <= buffer.Capacity);
					}
					catch (Exception ex)
					{
						errors.Add(ex);
					}
				}
			});

			Task.WaitAll(dequeuer, reader);
			Assert.AreEqual(0, errors.Count);
		}
	}
}