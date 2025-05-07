using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Count_WhenBufferIsEmpty_ShouldBeZero()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void Count_WhenItemsAreEnqueued_ShouldMatchItemCount()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(4);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			Assert.AreEqual(2, buffer.Count);
		}

		[TestMethod]
		public void Count_WhenItemsAreEnqueuedAndDequeued_ShouldUpdateCorrectly()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(4);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.TryDequeue(out _);
			Assert.AreEqual(1, buffer.Count);
		}

		[TestMethod]
		public void Count_WhenBufferIsCleared_ShouldBeZero()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Clear();
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void Count_WhenEnqueueingConcurrently_ShouldNotExceedCapacity()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			Parallel.For(0, 1000, i =>
			{
				try { buffer.TryEnqueue(new TestItem(i)); } catch { }
			});

			Assert.IsTrue(buffer.Count <= buffer.Capacity, "Count exceeded capacity.");
		}

		[TestMethod]
		public void Count_WhenDequeuingConcurrently_ShouldNeverBeNegative()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			Parallel.For(0, 10, i =>
			{
				buffer.TryDequeue(out _);
			});

			Assert.IsTrue(buffer.Count >= 0, "Count became negative.");
		}

		[TestMethod]
		public void Count_WhenMutatingConcurrently_ShouldAlwaysRemainInRange()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(8);
			for (int i = 0; i < 4; i++) buffer.Enqueue(new TestItem(i));

			var failed = false;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 1000; i++)
				{
					buffer.TryEnqueue(new TestItem(i));
					buffer.TryDequeue(out _);
					int count = buffer.Count;
					if (count < 0 || count > buffer.Capacity) failed = true;
				}
			});

			writer.Wait();
			Assert.IsFalse(failed, "Count went out of bounds during concurrent mutation.");
		}

		[TestMethod]
		public void Count_WhenClearedDuringMutation_ShouldEventuallyReset()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					buffer.Clear();
					Thread.SpinWait(100);
				}
			});

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 1000; i++)
				{
					buffer.TryEnqueue(new TestItem(i));
					buffer.TryDequeue(out _);
				}
			});

			Task.WaitAll(clearer, writer);
			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= buffer.Capacity);
		}
	}
}