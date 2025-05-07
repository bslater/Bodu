using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void TrimExcess_WhenCalled_ShouldPreserveItems()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 5; i++)
				buffer.Enqueue(new TestItem(i));

			buffer.TrimExcess();

			var snapshot = buffer.ToArray().Select(x => x?.Value).ToArray();
			CollectionAssert.AreEqual(new[] { 0, 1, 2, 3, 4 }, snapshot);
			Assert.AreEqual(5, buffer.Capacity);
		}

		[TestMethod]
		public void TrimExcess_WhenBufferIsEmpty_ShouldSetCapacityToMinimum()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(8);
			buffer.TrimExcess();

			Assert.AreEqual(1, buffer.Capacity); // Assuming minimum capacity of 1
			Assert.AreEqual(0, buffer.Count);
		}

		[TestMethod]
		public void TrimExcess_WhenCalledDuringEnqueue_ShouldRemainSafe()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(100);
			var failures = new ConcurrentBag<Exception>();

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					try { buffer.Enqueue(new TestItem(i)); }
					catch (Exception ex) { failures.Add(ex); }
				}
			});

			var trimmer = Task.Run(() =>
			{
				for (int i = 0; i < 5; i++)
				{
					try { buffer.TrimExcess(); }
					catch (Exception ex) { failures.Add(ex); }
					Thread.SpinWait(100);
				}
			});

			Task.WaitAll(writer, trimmer);

			Assert.AreEqual(0, failures.Count, "TrimExcess threw during concurrent usage.");
			Assert.IsTrue(buffer.Count > 0);
		}

		[TestMethod]
		public void TrimExcess_WhenCalledDuringDequeue_ShouldRemainSafe()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(50);
			for (int i = 0; i < 50; i++) buffer.Enqueue(new TestItem(i));

			var failures = new ConcurrentBag<Exception>();

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 25; i++)
				{
					try { buffer.TryDequeue(out _); }
					catch (Exception ex) { failures.Add(ex); }
				}
			});

			var trimmer = Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					try { buffer.TrimExcess(); }
					catch (Exception ex) { failures.Add(ex); }
					Thread.SpinWait(50);
				}
			});

			Task.WaitAll(reader, trimmer);

			Assert.AreEqual(0, failures.Count);
			Assert.IsTrue(buffer.Count <= buffer.Capacity);
		}
	}
}