namespace Bodu.Collections.Generic
{
	public partial class CircularBufferTests
	{
		/// <summary>
		/// Verifies that Contains returns true when the specified item exists in the buffer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenItemExists_ShouldReturnTrue()
		{
			var buffer = new CircularBuffer<string>(3);
			buffer.Enqueue("x");
			Assert.IsTrue(buffer.Contains("x"));
		}

		/// <summary>
		/// Verifies that Contains and IndexOf use default equality comparison for value types.
		/// </summary>
		[TestMethod]
		public void Contains_WhenUsingValueTypes_ShouldUseDefaultEquality()
		{
			var buffer = new CircularBuffer<int>(3);
			buffer.Enqueue(10);
			buffer.Enqueue(20);

			Assert.IsTrue(buffer.Contains(10));
			Assert.IsFalse(buffer.Contains(30));
		}

		/// <summary>
		/// Verifies that Contains returns false when the specified item does not exist in the buffer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenItemDoesNotExist_ShouldReturnFalse()
		{
			var buffer = new CircularBuffer<int>(2);
			buffer.Enqueue(1);
			Assert.IsFalse(buffer.Contains(99));
		}

		/// <summary>
		/// Verifies that Contains returns true when null is present in the buffer.
		/// </summary>
		[TestMethod]
		public void Contains_WhenBufferContainsNull_ShouldReturnTrue()
		{
			var buffer = new CircularBuffer<object>(2);
			buffer.Enqueue(null);
			Assert.IsTrue(buffer.Contains(null));
		}
	}
}