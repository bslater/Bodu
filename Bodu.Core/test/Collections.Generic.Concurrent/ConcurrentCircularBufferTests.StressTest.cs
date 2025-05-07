using System.Diagnostics;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[DataTestMethod]
		[DataRow(10, true)]
		[DataRow(50, true)]
		[DataRow(10, false)]
		[DataRow(50, false)]
		[TestCategory("Stress")]
		public void ConcurrentStressTest_WhenAccessingBuffer_ShouldNotCorruptInternalState(int capacity, bool allowOverwrite)
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(capacity, allowOverwrite);
			using (var cts = new CancellationTokenSource())
			{
				var startGate = new ManualResetEventSlim(false);

				int enqueued = 0, dequeued = 0, faults = 0;
				const int threadCount = 2;
				const int durationMs = 2000;

				var readers = Enumerable.Range(0, threadCount).Select(threadId =>
				{
					int localId = threadId;
					return Task.Run(() =>
					{
						startGate.Wait();
						while (!cts.Token.IsCancellationRequested)
						{
							if (buffer.TryDequeue(out var item) && item != null)
							{
								Interlocked.Increment(ref dequeued);
								item.Value *= -1; // Mutate after dequeue
							}
						}
					});
				});

				var writers = Enumerable.Range(0, threadCount).Select(threadId =>
				{
					int localId = threadId;
					return Task.Run(() =>
					{
						startGate.Wait();
						while (!cts.Token.IsCancellationRequested)
						{
							var value = Interlocked.Increment(ref enqueued);
							var item = value % 2 == 0 ? new TestItem(value) : new TestItem(value * -1); // Some mutation
							buffer.TryEnqueue(item);
						}
					});
				});

				var inspectors = Enumerable.Range(0, threadCount).Select(threadId =>
				{
					int localId = threadId;
					return Task.Run(() =>
					{
						startGate.Wait();
						while (!cts.Token.IsCancellationRequested)
						{
							try
							{
								int count = buffer.Count;
								int cap = buffer.Capacity;

								if (count > 0)
								{
									var _ = buffer[0];
									buffer.Contains(buffer[0]);
									buffer.TryPeek(out _);
									buffer.Peek();
								}
							}
							catch
							{
								Interlocked.Increment(ref faults);
							}
						}
					});
				});

				// Start all tasks simultaneously
				var allTasks = writers.Concat(readers).Concat(inspectors).ToArray();
				startGate.Set();
				Thread.Sleep(durationMs);
				cts.Cancel();

				Task.WaitAll(allTasks);
				TestContext.WriteLine($"Count={buffer.Count}, Capacity={buffer.Capacity}, Enq={enqueued}, Deq={dequeued}, Faults={faults}");
				TestItem[]? snapshot = null;
				try
				{
					snapshot = buffer.ToArray();
				}
				catch (Exception ex)
				{
					TestContext.WriteLine($"Snapshot failed: {ex}");
				}

				if (snapshot != null)
				{
					TestContext.WriteLine($"[Snapshot] Items: {string.Join(", ", snapshot.Select(x => x?.Value.ToString() ?? "null"))}");
				}

				Assert.AreEqual(0, faults, "Unexpected exception occurred during concurrent access.");
				Assert.IsTrue(buffer.Count <= buffer.Capacity, "Buffer count exceeded capacity.");
			}
		}
	}
}