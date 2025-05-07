using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		/// <summary>
		/// Verifies that Clear removes all items even if concurrently reading Count and ToArray.
		/// </summary>
		[TestMethod]
		public void Clear_WhenInvokedDuringRead_ShouldProduceConsistentEmptyState()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++) buffer.Enqueue(new TestItem(i));

			var consistent = true;

			var reader = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					int count = buffer.Count;
					var snapshot = buffer.ToArray();

					// Accept temporary inconsistency during concurrent clear
					if (snapshot.Length > count)
					{
						consistent = false;
						break;
					}

					Thread.SpinWait(50);
				}
			});

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 10; i++)
				{
					buffer.Clear();
					Thread.SpinWait(100);
				}
			});

			Task.WaitAll(reader, clearer);

			Assert.IsTrue(consistent, "ToArray() returned more elements than reported by Count during concurrent Clear.");
			Assert.AreEqual(0, buffer.Count);
			CollectionAssert.AreEqual(Array.Empty<TestItem>(), buffer.ToArray());
		}

		/// <summary>
		/// Verifies that Clear does not interfere with concurrent Enqueue and Count is never negative.
		/// </summary>
		[TestMethod]
		public void Clear_WhenInvokedDuringEnqueue_ShouldNotCorruptState()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(8);
			var failed = false;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 1000; i++)
				{
					buffer.TryEnqueue(new TestItem(i));
					Thread.SpinWait(10);
				}
			});

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					buffer.Clear();
					if (buffer.Count < 0 || buffer.Count > buffer.Capacity)
						failed = true;

					Thread.SpinWait(100);
				}
			});

			Task.WaitAll(writer, clearer);

			Assert.IsFalse(failed, "Buffer state became invalid (Count out of range) during Clear.");
		}

		/// <summary>
		/// Verifies that Clear does not interfere with concurrent Dequeue and leaves valid state.
		/// </summary>
		[TestMethod]
		public void Clear_WhenInvokedDuringDequeue_ShouldNotCauseInvalidOperation()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5);
			for (int i = 0; i < 5; i++) buffer.Enqueue(new TestItem(i));

			var exceptions = new ConcurrentBag<Exception>();

			var dequeuer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						buffer.TryDequeue(out _);
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}
					Thread.SpinWait(20);
				}
			});

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 20; i++)
				{
					buffer.Clear();
					Thread.SpinWait(50);
				}
			});

			Task.WaitAll(dequeuer, clearer);

			Assert.IsTrue(exceptions.All(e => e is not InvalidOperationException), "Clear should not corrupt internal state during Dequeue.");
		}

		/// <summary>
		/// Verifies that Clear is idempotent and safe when called repeatedly during concurrent activity.
		/// </summary>
		[TestMethod]
		public void Clear_WhenCalledRepeatedly_ShouldRemainStable()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(4);
			var failed = false;

			var toggler = Task.Run(() =>
			{
				for (int i = 0; i < 1000; i++)
				{
					if (!buffer.TryEnqueue(new TestItem(i)))
						buffer.TryDequeue(out _);
				}
			});

			var clearer = Task.Run(() =>
			{
				for (int i = 0; i < 200; i++)
				{
					buffer.Clear();
					if (buffer.Count < 0 || buffer.Count > buffer.Capacity)
						failed = true;

					Thread.SpinWait(20);
				}
			});

			Task.WaitAll(toggler, clearer);

			Assert.IsFalse(failed, "Repeated Clear caused buffer state inconsistency.");
			Assert.IsTrue(buffer.Count >= 0 && buffer.Count <= buffer.Capacity);
		}
	}
}