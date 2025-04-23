using Bodu.Security.Cryptography;

namespace Bodu.Infrastructure
{
    [TestClass]
    public class Initialize
    {
        /// <summary>
        /// Verifies that Initialize resets the internal index.
        /// </summary>
        [TestMethod]
        public void WhenCalled_ShouldResetIndex()
        {
            var buffer = new ByteBuffer(3);
            buffer.Add(new byte[] { 1 }, 0, 1);
            buffer.Initialize();
            Assert.AreEqual(0, buffer.Count);
        }

        /// <summary>
        /// Verifies that Initialize(clear: false) does not clear buffer contents, but
        /// GetBytesZeroPadded() still zeroes them.
        /// </summary>
        [TestMethod]
        public void WhenClearFalse_ShouldNotPreserveBufferContentsInZeroPadded()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 9 }, 0, 1);
            buffer.Initialize(clear: false);
            var contents = buffer.GetBytesZeroPadded();
            Assert.AreEqual(0, contents[0]); // buffer was cleared during GetBytesZeroPadded
        }

        /// <summary>
        /// Verifies that Initialize(true) clears all buffer contents.
        /// </summary>
        [TestMethod]
        public void WhenClearTrue_ShouldClearBuffer()
        {
            var buffer = new ByteBuffer(2);
            buffer.Add(new byte[] { 7 }, 0, 1);
            buffer.Initialize(clear: true);
            var contents = buffer.GetBytesZeroPadded();
            CollectionAssert.AreEqual(new byte[] { 0, 0 }, contents);
        }
    }
}