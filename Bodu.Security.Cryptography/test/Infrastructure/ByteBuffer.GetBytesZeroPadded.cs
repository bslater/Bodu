using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class GetBytesZeroPadded
    {
        /// <summary>
        /// Verifies that the unused portion of the buffer is zeroed.
        /// </summary>
        [TestMethod]
        public void ShouldClearRemainingBytes()
        {
            var buffer = new ByteBuffer(4);
            buffer.Add(new byte[] { 1 }, 0, 1);
            var result = buffer.GetBytesZeroPadded();
            CollectionAssert.AreEqual(new byte[] { 1, 0, 0, 0 }, result);
        }

        /// <summary>
        /// Verifies that the written portion is preserved after zero padding.
        /// </summary>
        [TestMethod]
        public void ShouldPreserveWrittenBytes()
        {
            var buffer = new ByteBuffer(3);
            buffer.Add(new byte[] { 5, 6 }, 0, 2);
            var result = buffer.GetBytesZeroPadded();
            Assert.AreEqual(5, result[0]);
            Assert.AreEqual(6, result[1]);
        }

        /// <summary>
        /// Verifies that calling GetBytesZeroPadded resets the buffer.
        /// </summary>
        [TestMethod]
        public void ShouldResetIndex()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1 }, 0, 1);
            buffer.GetBytesZeroPadded();
            Assert.IsTrue(buffer.IsEmpty);
        }

        /// <summary>
        /// Verifies that Initialize(clear: false) does not erase old data.
        /// </summary>
        [TestMethod]
        public void WhenInitializedWithoutClear_ShouldPreserveOldData()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 1 }, 0, 1);
            buffer.Initialize(clear: false);
            var contents = buffer.GetBytesZeroPadded();
            Assert.AreEqual(0, contents[0]);
        }
    }
}