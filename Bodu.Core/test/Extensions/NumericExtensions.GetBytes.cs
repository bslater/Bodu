namespace Bodu.Extensions
{
	[TestClass]
	public partial class NumericExtensions
	{
		/// <summary>
		/// Verifies that the byte array returned for a known integer value matches the expected byte order based on system endianness.
		/// </summary>
		[TestMethod]
		[DataRow(0, new byte[] { 0x00 })]  // Zero value
		[DataRow(1, new byte[] { 0x01 })]  // Least significant bit set
		[DataRow(255, new byte[] { 0xFF })]  // All bits set for byte
		[DataRow(int.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0x7F })]  // max value for int
		[DataRow(int.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x80 })]  // min value for int
		[DataRow(long.MaxValue, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0x7F })]  // max value for long
		[DataRow(long.MinValue, new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x80 })]  // min value for long
		[DataRow(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]  // Two's complement for -1 (int)
		[DataRow(-int.MaxValue, new byte[] { 0x80, 0x00, 0x00, 0x00 })]  // Two's complement for -int.MaxValue
		[DataRow(-long.MaxValue, new byte[] { 0x80, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 })]  // Two's complement for -long.MaxValue
		public void GetBytes_WhenCalled_ShouldReturnExpectedByteArrayForVariousValues(long input, byte[] expected)
		{
			var bytes = input.GetBytes();
			CollectionAssert.AreEqual(expected, bytes);
		}

		/// <summary>
		/// Verifies that the byte output of GetBytes matches BitConverter.GetBytes in safe mode.
		/// </summary>
		[TestMethod]
		[DataRow(42, new byte[] { 0x2A, 0x00, 0x00, 0x00 })] // Basic check
		public void GetBytes_WhenCalled_ShouldMatchBitConverter_ForSafeMode(int value, byte[] expected)
		{
			var actual = value.GetBytes();
			CollectionAssert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that the byte array for zero is filled entirely with zeros.
		/// </summary>
		[TestMethod]
		[DataRow(0, new byte[] { 0x00, 0x00, 0x00, 0x00 })]
		public void GetBytes_WhenValueIsZero_ShouldReturnAllZeros(int value, byte[] expected)
		{
			var bytes = value.GetBytes();
			CollectionAssert.AreEqual(expected, bytes);
		}

		/// <summary>
		/// Verifies that GetBytes returns a byte array of the expected length for various value types.
		/// </summary>
		[TestMethod]
		[DataRow((short)1, 2)] // 2 bytes for short
		[DataRow((ushort)1, 2)] // 2 bytes for ushort
		[DataRow(1, 4)] // 4 bytes for int
		[DataRow(1U, 4)] // 4 bytes for uint
		[DataRow(1L, 8)] // 8 bytes for long
		[DataRow(1UL, 8)] // 8 bytes for ulong
		public void GetBytes_WhenCalled_ShouldReturnExpectedLengthForVariousTypes<T>(T value, int expectedLength)
		{
			var length = value.GetBytes().Length;
			Assert.AreEqual(expectedLength, length);
		}

		/// <summary>
		/// Verifies that the first byte of the output corresponds to the system endianness.
		/// </summary>
		[TestMethod]
		[DataRow((ushort)0x1234, new byte[] { 0x34, 0x12 })]  // Checking endianness for ushort
		[DataRow(0x12345678, new byte[] { 0x78, 0x56, 0x34, 0x12 })]  // Checking endianness for int
		public void GetBytes_WhenCalled_ShouldRespectSystemEndianness<T>(T value, byte[] expected)
		{
			var bytes = value.GetBytes();
			CollectionAssert.AreEqual(expected, bytes);
		}

		/// <summary>
		/// Verifies that the byte array returned for a known integer value matches the expected byte order based on system endianness.
		/// </summary>
		[TestMethod]
		[DataRow(0x12345678, new byte[] { 0x78, 0x56, 0x34, 0x12 })]  // little-endian test
		[DataRow(0x12345678, new byte[] { 0x12, 0x34, 0x56, 0x78 })]  // big-endian test
		public void GetBytes_WhenCalled_ShouldReturnCorrectBytes_ForKnownIntegerValues(int value, byte[] expected)
		{
			var bytes = value.GetBytes();
			CollectionAssert.AreEqual(expected, bytes);
		}

		/// <summary>
		/// Verifies that two's complement representation is preserved for negative values.
		/// </summary>
		[TestMethod]
		[DataRow(-1, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })]  // Two's complement for -1 (int)
		[DataRow(-int.MaxValue, new byte[] { 0x80, 0x00, 0x00, 0x00 })]  // Two's complement for -int.MaxValue
		public void GetBytes_WhenNegativeValue_ShouldPreserveTwoComplement(int value, byte[] expected)
		{
			var bytes = value.GetBytes();
			CollectionAssert.AreEqual(expected, bytes);
		}
	}
}