using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void ToArray_WhenBufferHasElements_ShouldReturnInOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(10));
			buffer.Enqueue(new TestItem(20));

			var result = buffer.ToArray().Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 10, 20 }, result);
		}

		[TestMethod]
		public void ToArray_WhenBufferIsEmpty_ShouldReturnEmptyArray()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			var result = buffer.ToArray();
			Assert.AreEqual(0, result.Length);
		}

		[TestMethod]
		public void ToArray_WhenWrapped_ShouldReturnFifoOrder()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));
			buffer.Dequeue();
			buffer.Enqueue(new TestItem(4)); // Wrap

			var result = buffer.ToArray().Select(x => x.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, result);
		}

		[TestMethod]
		public void ToArray_WhenReadDuringEnqueue_ShouldNotThrowAndYieldSnapshot()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var failures = new ConcurrentBag<Exception>();

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						var snapshot = buffer.ToArray();
						Assert.IsTrue(snapshot.Length <= buffer.Capacity);
					}
					catch (Exception ex)
					{
						failures.Add(ex);
					}
				}
			});

			var writer = Task.Run(() =>
			{
				for (int i = 10; i < 110; i++)
					buffer.Enqueue(new TestItem(i));
			});

			Task.WaitAll(reader, writer);
			Assert.AreEqual(0, failures.Count, "ToArray threw during concurrent enqueue.");
		}

		[TestMethod]
		public void ToArray_WhenReadDuringDequeue_ShouldRemainConsistent()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 5; i++)
				buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 20; i++)
				{
					try
					{
						var snapshot = buffer.ToArray();
						Assert.IsTrue(snapshot.Length <= buffer.Capacity);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
				}
			});

			var dequeuer = Task.Run(() =>
			{
				for (int i = 0; i < 5; i++)
					buffer.TryDequeue(out _);
			});

			Task.WaitAll(reader, dequeuer);
			Assert.AreEqual(0, exceptions.Count);
		}
	}
}