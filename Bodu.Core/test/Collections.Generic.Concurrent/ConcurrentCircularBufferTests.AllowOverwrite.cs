using System.Collections.Concurrent;

namespace Bodu.Collections.Generic.Concurrent
{
	public partial class ConcurrentCircularBufferTests
	{
		[TestMethod]
		public void AllowOverwrite_WhenToggledConcurrently_ShouldRemainConsistent()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(5, false);

			Parallel.For(0, 1000, i =>
			{
				if (i % 2 == 0)
					buffer.AllowOverwrite = true;
				else
					buffer.AllowOverwrite = false;

				_ = buffer.AllowOverwrite;
			});

			Assert.IsTrue(buffer.AllowOverwrite == true || buffer.AllowOverwrite == false);
		}

		[TestMethod]
		public void AllowOverwrite_WhenToggledDuringConcurrency_ShouldAffectBehaviorImmediately()
		{
			var buffer = new ConcurrentCircularBuffer<TestItem>(3, allowOverwrite: true);
			buffer.Enqueue(new TestItem(1));
			buffer.Enqueue(new TestItem(2));
			buffer.Enqueue(new TestItem(3));

			var exceptions = new ConcurrentBag<Exception>();

			var writer = Task.Run(() =>
			{
				for (int i = 0; i < 200; i++) // more iterations
				{
					try
					{
						buffer.Enqueue(new TestItem(100 + i));
					}
					catch (Exception ex)
					{
						exceptions.Add(ex);
					}

					// Introduce random delay to vary interleaving
					Thread.SpinWait(1000 + (i % 5) * 100);
				}
			});

			var toggler = Task.Run(() =>
			{
				for (int i = 0; i < 200; i++)
				{
					buffer.AllowOverwrite = (i % 2 == 0);
					Thread.SpinWait(2000); // longer delay to keep setting stable
				}
			});

			Task.WaitAll(writer, toggler);

			Assert.IsTrue(exceptions.Count > 0, "At least one enqueue should have failed when AllowOverwrite was false.");
			Assert.IsTrue(exceptions.All(e => e is InvalidOperationException));
		}
	}
}