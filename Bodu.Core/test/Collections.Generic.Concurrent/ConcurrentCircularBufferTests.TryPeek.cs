using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void TryPeek_WhenCalledDuringConcurrentEnqueue_ShouldEventuallySucceed()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			var peeked = 0;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
					buffer.Enqueue(new TestItem(i));
			});

			var reader = Task.Run(() =>
			{
				while (Volatile.Read(ref peeked) == 0)
				{
					if (buffer.TryPeek(out var item) && item != null)
					{
						Interlocked.Exchange(ref peeked, 1);
					}
				}
			});

			Task.WaitAll(writer, reader);
			Assert.AreEqual(1, peeked, "TryPeek never succeeded during enqueuing.");
		}

		[TestMethod]
		public void TryPeek_WhenCalledDuringConcurrentDequeue_ShouldRemainSafe()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			var failures = 0;

			var peeker = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try { buffer.TryPeek(out _); }
					catch { Interlocked.Increment(ref failures); }
				}
			});

			var consumer = Task.Run(() =>
			{
				while (buffer.TryDequeue(out _)) { }
			});

			Task.WaitAll(peeker, consumer);
			Assert.AreEqual(0, failures, "TryPeek threw during concurrent dequeue.");
		}

		[TestMethod]
		public void TryPeek_WhenCalledDuringRapidEnqueueDequeue_ShouldYieldConsistentOldestItem()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			int observed = 0;

			var enqueuer = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					buffer.Enqueue(new TestItem(i));
					Thread.Sleep(1);
				}
			});

			var dequeuer = Task.Run(() =>
			{
				while (true)
				{
					buffer.TryDequeue(out _);
					Thread.Sleep(1);
					if (Volatile.Read(ref observed) >= 10)
						break;
				}
			});

			var peeker = Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					if (buffer.TryPeek(out var item) && item != null)
						Interlocked.Increment(ref observed);
					Thread.Sleep(1);
				}
			});

			Task.WaitAll(enqueuer, dequeuer, peeker);
			Assert.IsTrue(observed > 0, "TryPeek did not observe any items.");
		}

		[TestMethod]
		public void TryPeek_WhenBufferContainsNullsConcurrently_ShouldSafelyYieldNull()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem?>(5);
			int nullSeen = 0;

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 20; i++)
				{
					buffer.Enqueue(i % 3 == 0 ? null : new TestItem(i));
				}
			});

			var peeker = Task.Run(() =>
			{
				for (int i = 0; i < 50; i++)
				{
					if (buffer.TryPeek(out var item) && item == null)
						Interlocked.Increment(ref nullSeen);
				}
			});

			Task.WaitAll(writer, peeker);
			Assert.IsTrue(nullSeen > 0, "Expected TryPeek to observe null items.");
		}

		[TestMethod]
		public void TryPeek_WhenMultipleThreadsReadConcurrently_ShouldNotThrowOrCorrupt()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(10);
			for (int i = 0; i < 10; i++)
				buffer.Enqueue(new TestItem(i));

			int totalAttempts = 0;
			int failures = 0;

			var tasks = Enumerable.Range(0, 4).Select(_ => Task.Run(() =>
			{
				for (int i = 0; i < 100; i++)
				{
					try
					{
						buffer.TryPeek(out TestItem _);
						Interlocked.Increment(ref totalAttempts);
					}
					catch
					{
						Interlocked.Increment(ref failures);
					}
				}
			})).ToArray();

			Task.WaitAll(tasks);
			Assert.AreEqual(0, failures, "TryPeek threw exceptions during concurrent reads.");
			Assert.IsTrue(totalAttempts > 0, "No TryPeek operations were attempted.");
		}
	}
}