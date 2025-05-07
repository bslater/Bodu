using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void Contains_WhenItemExistsConcurrently_ShouldEventuallyReturnTrue()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var target = new TestItem(999);
			var found = false;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					buffer.TryEnqueue(i == 25 ? target : new TestItem(i));
					Thread.SpinWait(100);
				}
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 200; i++)
				{
					if (buffer.Contains(target))
					{
						found = true;
						break;
					}
					Thread.SpinWait(50);
				}
			});

			Task.WaitAll(writer, reader);
			Assert.IsTrue(found, "Target item was never detected by Contains.");
		}

		[TestMethod]
		public void Contains_WhenItemDoesNotExist_ShouldAlwaysReturnFalse()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var missing = new TestItem(999);
			var result = Enumerable.Range(0, 1000).AsParallel().All(_ => !buffer.Contains(missing));

			Assert.IsTrue(result, "Contains returned true for missing item.");
		}

		[TestMethod]
		public void Contains_WhenItemMayBeEnqueuedOrCleared_ShouldNotThrow()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			var target = new TestItem(42);
			var exceptions = new ConcurrentBag<Exception>();

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					buffer.TryEnqueue(i % 2 == 0 ? target : new TestItem(i));
					if (i % 10 == 0) buffer.Clear();
					Thread.SpinWait(20);
				}
			});

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 200; i++)
				{
					try
					{
						_ = buffer.Contains(target);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
					Thread.SpinWait(10);
				}
			});

			Task.WaitAll(writer, reader);

			Assert.AreEqual(0, exceptions.Count, "Contains threw during concurrent mutation.");
		}

		[TestMethod]
		public void Contains_WhenBufferContainsNull_ShouldReturnTrue()
		{
			var buffer = new ConcurrentCircularBuffer<object>(5);
			buffer.Enqueue(null);
			Assert.IsTrue(buffer.Contains(null));
		}

		[TestMethod]
		public void Contains_WhenUsedConcurrently_ShouldHonorDefaultEquality()
		{
			var buffer = new ConcurrentCircularBuffer<string>(10);
			for (int i = 0; i < 5; i++) buffer.Enqueue("Test" + i);

			var result = Enumerable.Range(0, 100).AsParallel().All(_ =>
				buffer.Contains("Test2") && !buffer.Contains("Missing"));

			Assert.IsTrue(result, "Contains failed to honor equality under concurrency.");
		}
	}
}