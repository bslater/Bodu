using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class AddSpan
    {
        /// <summary>
        /// Verifies that a valid span is added successfully.
        /// </summary>
        [TestMethod]
        public void WhenValidSpanAdded_ShouldReturnCorrectly()
        {
            var buffer = new ByteBuffer(3);
            var span = new byte[] { 1, 2 };
            var result = buffer.Add(span);
            Assert.IsFalse(result);
            Assert.AreEqual(2, buffer.Count);
        }

        /// <summary>
        /// Verifies that adding a span that exceeds capacity throws ArgumentOutOfRangeException.
        /// </summary>
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void WhenSpanExceedsRemainingCapacity_ShouldThrowExactly()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1 }, 0, 1);
            var span = new byte[] { 2, 3 };
            buffer.Add(span);
        }

        /// <summary>
        /// Verifies that sliced spans are correctly added to the buffer.
        /// </summary>
        [TestMethod]
        public void WhenAddingSpanSlice_ShouldAddCorrectBytes()
        {
            var data = new byte[] { 1, 2, 3, 4 };
            var buffer = new ByteBuffer(2);
            buffer.Add(data.AsSpan(1, 2)); // adds 2 and 3
            var result = buffer.GetBytesZeroPadded();
            CollectionAssert.AreEqual(new byte[] { 2, 3 }, result);
        }
    }
}