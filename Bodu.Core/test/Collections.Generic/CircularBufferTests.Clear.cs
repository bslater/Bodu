namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that Clear removes all items from the buffer and resets its state.
		/// </summary>
		[TestMethod]
		public void Clear_ShouldClearAllItems()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Clear();

			Assert.AreEqual(0, buffer.Count);

			Assert.ThrowsExactly<InvalidOperationException>(() =>
			{
				_ = buffer.Dequeue(); // should throw
			});
		}

		/// <summary>
		/// Verifies that Clear resets buffer state even after wraparound.
		/// </summary>
		[TestMethod]
		public void Clear_WhenCalledAfterWraparound_ShouldResetInternalState()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Dequeue();
			buffer.Enqueue(4); // wrap
			buffer.Clear();

			Assert.AreEqual(0, buffer.Count);
			Assert.AreEqual(3, buffer.Capacity);
			CollectionAssert.AreEqual(Array.Empty<int>(), buffer.ToArray());
		}

		/// <summary> Verifies that Clear removes all elements when the buffer is not wrapped (head < tail). </summary>
		[TestMethod]
		public void Clear_WhenNotWrapped_ShouldRemoveAllItems()
		{
			var buffer = new CircularBuffer<string>(5);
			buffer.Enqueue("A");
			buffer.Enqueue("B");
			buffer.Enqueue("C");

			buffer.Clear();

			Assert.AreEqual(0, buffer.Count);
			Assert.IsTrue(buffer.ToArray().Length == 0);
		}

		/// <summary>
		/// Verifies that Clear zeroes both array segments when the buffer is wrapped (head &gt;;= tail).
		/// </summary>
		[TestMethod]
		public void Clear_WhenWrapped_ShouldZeroAllSegments()
		{
			var buffer = new CircularBuffer<int>(5);

			// Fill and wrap the buffer
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Enqueue(4);
			buffer.Enqueue(5);
			buffer.Dequeue(); // move head forward
			buffer.Enqueue(6); // causes wrap

			Assert.IsTrue(buffer.Count > 0);
			Assert.IsTrue(buffer.GetSegments().FirstSegment.Count > 0 || buffer.GetSegments().SecondSegment.Count > 0);

			buffer.Clear();

			Assert.AreEqual(0, buffer.Count);
			CollectionAssert.AreEqual(Array.Empty<int>(), buffer.ToArray());
		}

		/// <summary>
		/// Verifies that Clear is a no-op when the buffer is already empty.
		/// </summary>
		[TestMethod]
		public void Clear_WhenBufferIsAlreadyEmpty_ShouldDoNothing()
		{
			var buffer = new CircularBuffer<string>(3);
			buffer.Clear(); // no items

			Assert.AreEqual(0, buffer.Count);
			Assert.IsTrue(buffer.ToArray().Length == 0);
		}
	}
}