namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that <see cref="CircularBuffer{T}.GetSegments" /> returns a single contiguous segment when data is not wrapped.
		/// </summary>
		[TestMethod]
		public void GetSegments_ShouldReturnSingleSegment_WhenDataIsContiguous()
		{
			var buffer = new CircularBuffer<int>(4);
			buffer.Enqueue(1);
			buffer.Enqueue(2);

			var segments = buffer.GetSegments();
			var combined = segments.FirstSegment.Concat(segments.SecondSegment).ToArray();

			CollectionAssert.AreEqual(new[] { 1, 2 }, combined);
		}

		/// <summary>
		/// Verifies that <see cref="CircularBuffer{T}.GetSegments" /> returns two wrapped segments when data has wrapped around.
		/// </summary>
		[TestMethod]
		public void GetSegments_ShouldReturnTwoSegments_WhenDataIsWrapped()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Dequeue();       // removes 1
			buffer.Enqueue(4);      // wraps around

			var segments = buffer.GetSegments();
			var combined = segments.FirstSegment.Concat(segments.SecondSegment).ToArray();

			CollectionAssert.AreEqual(new[] { 2, 3, 4 }, combined);
		}

		/// <summary>
		/// Verifies that <see cref="CircularBuffer{T}.GetSegments" /> returns empty segments when the buffer is empty.
		/// </summary>
		[TestMethod]
		public void GetSegments_ShouldReturnEmptySegments_WhenBufferIsEmpty()
		{
			var buffer = new CircularBuffer<string>(2);
			var segments = buffer.GetSegments();

			Assert.AreEqual(0, segments.FirstSegment.Count);
			Assert.AreEqual(0, segments.SecondSegment.Count);
		}

		/// <summary>
		/// Verifies that GetSegments returns two separate segments when the buffer is wrapped around (head &gt; tail).
		/// </summary>
		[TestMethod]
		public void GetSegments_WhenWrapped_ShouldReturnTwoSegments()
		{
			var buffer = new CircularBuffer<int>(5, allowOverwrite: true);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);
			buffer.Enqueue(4);
			buffer.Enqueue(5);
			buffer.Dequeue(); // Advance head
			buffer.Enqueue(6); // Wrap

			var (first, second) = buffer.GetSegments();

			Assert.IsTrue(first.Count + second.Count == buffer.Count);
			Assert.IsTrue(first.Count > 0);
			Assert.IsTrue(second.Count > 0);
		}

		/// <summary> Verifies that GetSegments returns a single segment when the buffer has not wrapped (head < tail). </summary>
		[TestMethod]
		public void GetSegments_WhenContiguous_ShouldReturnSingleSegment()
		{
			var buffer = new CircularBuffer<int>(5);
			buffer.Enqueue(1);
			buffer.Enqueue(2);
			buffer.Enqueue(3);

			var (first, second) = buffer.GetSegments();

			Assert.AreEqual(buffer.Count, first.Count + second.Count);
			Assert.IsTrue(second.Count == 0);
		}
	}
}