using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void MultipleInstances_WhenUsedConcurrently_ShouldMaintainSeparateState()
		{
			var buffer1 = new ConcurrentCircularBuffer<TestItem>(5);
			var buffer2 = new ConcurrentCircularBuffer<TestItem>(5);

			Parallel.Invoke(
				() =>
				{
					for (int i = 0; i < 50; i++)
						buffer1.Enqueue(new TestItem(i));
				},
				() =>
				{
					for (int i = 100; i < 150; i++)
						buffer2.Enqueue(new TestItem(i));
				});

			var values1 = buffer1.ToArray().Select(x => x.Value).ToArray();
			var values2 = buffer2.ToArray().Select(x => x.Value).ToArray();

			Assert.IsTrue(values1.All(v => v < 100));
			Assert.IsTrue(values2.All(v => v >= 100));
		}

		[TestMethod]
		public void MultipleInstances_WhenUsingEvents_ShouldMaintainEventIsolation()
		{
			var buffer1Events = new ConcurrentBag<string>();
			var buffer2Events = new ConcurrentBag<string>();

			var buffer1 = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);
			var buffer2 = new ConcurrentCircularBuffer<TestItem>(2, allowOverwrite: true);

			buffer1.ItemEvicted += item => buffer1Events.Add("B1:" + item?.Value);
			buffer2.ItemEvicted += item => buffer2Events.Add("B2:" + item?.Value);

			buffer1.Enqueue(new TestItem(1));
			buffer1.Enqueue(new TestItem(2));
			buffer1.Enqueue(new TestItem(3)); // Evict 1

			buffer2.Enqueue(new TestItem(100));
			buffer2.Enqueue(new TestItem(200));
			buffer2.Enqueue(new TestItem(300)); // Evict 100

			Assert.IsTrue(buffer1Events.Contains("B1:1"));
			Assert.IsTrue(buffer2Events.Contains("B2:100"));
			Assert.IsFalse(buffer1Events.Any(e => e.StartsWith("B2:")));
			Assert.IsFalse(buffer2Events.Any(e => e.StartsWith("B1:")));
		}

		[TestMethod]
		public void MultipleInstances_WhenAccessedInParallel_ShouldRemainThreadSafe()
		{
			var buffers = Enumerable.Range(0, 5)
				.Select(_ => new ConcurrentCircularBuffer<TestItem>(20, allowOverwrite: true))
				.ToArray();

			Parallel.ForEach(buffers, buffer =>
			{
				for (int i = 0; i < 100; i++)
					buffer.Enqueue(new TestItem(i));

				var snapshot = buffer.ToArray();
				Assert.IsTrue(snapshot.Length <= 20);
				Assert.IsTrue(snapshot.All(x => x is not null));
			});
		}
	}
}