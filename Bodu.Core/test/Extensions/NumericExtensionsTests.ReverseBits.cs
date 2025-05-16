using System.Diagnostics;

namespace Bodu.Extensions
{
	public partial class NumericExtensionsTests
	{
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a byte value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow((byte)0x00, (byte)0x00)] // all bits zero
		[DataRow((byte)0x01, (byte)0x80)] // only LSB set
		[DataRow((byte)0x80, (byte)0x01)] // only MSB set
		[DataRow((byte)0xFF, (byte)0xFF)] // all bits set
		[DataRow((byte)0xAA, (byte)0x55)] // alternating bits: 1010...
		[DataRow((byte)0x55, (byte)0xAA)] // alternating bits: 0101...
		[DataRow((byte)0x0F, (byte)0xF0)] // 4-bit alternating patterns
		[DataRow((byte)0xF0, (byte)0x0F)] // inverse 4-bit pattern
		[DataRow((byte)0x12, (byte)0x48)] // sequential pattern
		[DataRow((byte)0x3F, (byte)0xFC)] // 6-bit pattern set
		[DataRow((byte)0xC0, (byte)0x03)] // 2 bits in high byte
		public void ReverseBits_Byte_ShouldReverseBits(byte value, byte expected)
		{
			byte actual = value.ReverseBits();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X2")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X2")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X2")}");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a ushort value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow((ushort)0x0000U, (ushort)0x0000U)] // all bits zero
		[DataRow((ushort)0x0001U, (ushort)0x8000U)] // only LSB set
		[DataRow((ushort)0x8000U, (ushort)0x0001U)] // only MSB set
		[DataRow((ushort)0xFFFFU, (ushort)0xFFFFU)] // all bits set
		[DataRow((ushort)0xAAAAU, (ushort)0x5555U)] // alternating bits: 1010...
		[DataRow((ushort)0x5555U, (ushort)0xAAAAU)] // alternating bits: 0101...
		[DataRow((ushort)0x0F0FU, (ushort)0xF0F0U)] // 4-bit alternating patterns
		[DataRow((ushort)0xF0F0U, (ushort)0x0F0FU)] // inverse 4-bit pattern
		[DataRow((ushort)0x1234U, (ushort)0x2C48U)] // sequential pattern
		[DataRow((ushort)0x00FFU, (ushort)0xFF00U)] // lower 8 bits set
		[DataRow((ushort)0x0100U, (ushort)0x0080U)] // one bit in high byte
		public void ReverseBits_UShort_ShouldReverseBits(ushort value, ushort expected)
		{
			ushort actual = value.ReverseBits();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X4")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X4")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X4")}");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a uint value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow(0x00000000U, 0x00000000U)] // all bits zero
		[DataRow(0x00000001U, 0x80000000U)] // only LSB set
		[DataRow(0x80000000U, 0x00000001U)] // only MSB set
		[DataRow(0xFFFFFFFFU, 0xFFFFFFFFU)] // all bits set
		[DataRow(0xAAAAAAAAU, 0x55555555U)] // alternating bits: 1010...
		[DataRow(0x55555555U, 0xAAAAAAAAU)] // alternating bits: 0101...
		[DataRow(0x0F0F0F0FU, 0xF0F0F0F0U)] // 4-bit alternating patterns
		[DataRow(0xF0F0F0F0U, 0x0F0F0F0FU)] // inverse 4-bit pattern
		[DataRow(0x01234567U, 0xE6A2C480U)] // sequential pattern
		[DataRow(0x0000FFFFU, 0xFFFF0000U)] // two halves
		[DataRow(0x0000FFFFU, 0xFFFF0000U)] // lower 16 bits set
		[DataRow(0x000000FFU, 0xFF000000U)] // lower 8 bits set
		[DataRow(0x00000100U, 0x00800000U)] // one bit in high byte
		public void ReverseBits_UInt_ShouldReverseBits(uint value, uint expected)
		{
			uint actual = value.ReverseBits();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X8")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X8")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X8")}");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a ulong value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow(0x0000000000000000UL, 0x0000000000000000UL)] // all bits zero
		[DataRow(0x0000000000000001UL, 0x8000000000000000UL)] // only LSB set
		[DataRow(0x8000000000000000UL, 0x0000000000000001UL)] // only MSB set
		[DataRow(0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL)] // all bits set
		[DataRow(0xAAAAAAAAAAAAAAAAUL, 0x5555555555555555UL)] // alternating bits: 1010...
		[DataRow(0x5555555555555555UL, 0xAAAAAAAAAAAAAAAAUL)] // alternating bits: 0101...
		[DataRow(0x0F0F0F0F0F0F0F0FUL, 0xF0F0F0F0F0F0F0F0UL)] // 4-bit alternating patterns
		[DataRow(0xF0F0F0F0F0F0F0F0UL, 0x0F0F0F0F0F0F0F0FUL)] // inverse 4-bit pattern
		[DataRow(0x0123456789ABCDEFUL, 0xF7B3D591E6A2C480UL)] // sequential pattern
		[DataRow(0x0000FFFF0000FFFFUL, 0xFFFF0000FFFF0000UL)] // two halves
		[DataRow(0x00000000FFFFFFFFUL, 0xFFFFFFFF00000000UL)] // lower 32 bits set
		[DataRow(0xFFFFFFFF00000000UL, 0x00000000FFFFFFFFUL)] // upper 32 bits set
		[DataRow(0x000000000000FFFFUL, 0xFFFF000000000000UL)] // lower 16 bits set
		[DataRow(0x00000000FFFF0000UL, 0x0000FFFF00000000UL)] // middle bits
		[DataRow(0x0000000100000000UL, 0x0000000080000000UL)] // one bit in high byte
		public void ReverseBits_ULong_ShouldReverseBits(ulong value, ulong expected)
		{
			ulong actual = value.ReverseBits();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X16")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X16")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X16")}");

			Assert.AreEqual(expected, actual);
		}

		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of each byte in a byte array.
		/// </summary>
		[DataTestMethod]
		[DataRow(null, new byte[0])] // Null array input should return an empty array
		[DataRow(new byte[] { 0x00 }, new byte[] { 0x00 })] // Single byte, all bits zero
		[DataRow(new byte[] { 0x01 }, new byte[] { 0x80 })] // Single byte, LSB set
		[DataRow(new byte[] { 0x80 }, new byte[] { 0x01 })] // Single byte, MSB set
		[DataRow(new byte[] { 0xFF }, new byte[] { 0xFF })] // Single byte, all bits set
		[DataRow(new byte[] { 0xAA }, new byte[] { 0x55 })] // Single byte, alternating bits 1010...
		[DataRow(new byte[] { 0x55 }, new byte[] { 0xAA })] // Single byte, alternating bits 0101...
		[DataRow(new byte[] { 0x0F, 0xF0 }, new byte[] { 0xF0, 0x0F })] // Two bytes, 4-bit alternating patterns
		[DataRow(new byte[] { 0xF0, 0x0F }, new byte[] { 0x0F, 0xF0 })] // Inverse of 4-bit alternating patterns
		[DataRow(new byte[] { 0x12, 0x34 }, new byte[] { 0x48, 0x2C })] // Two bytes, sequential pattern
		[DataRow(new byte[] { 0x3F, 0xC0 }, new byte[] { 0xFC, 0x03 })] // Two bytes, 6-bit pattern set
		[DataRow(new byte[] { 0x00, 0x00, 0x00 }, new byte[] { 0x00, 0x00, 0x00 })] // Three bytes, all zeroes
		[DataRow(new byte[] { 0x01, 0x80 }, new byte[] { 0x80, 0x01 })] // Two bytes, LSB and MSB set
		[DataRow(new byte[] { 0x00, 0x00, 0x00, 0x00 }, new byte[] { 0x00, 0x00, 0x00, 0x00 })] // Four bytes, all zeroes
		[DataRow(new byte[] { 0x01, 0x01, 0x01, 0x01 }, new byte[] { 0x80, 0x80, 0x80, 0x80 })] // Four bytes, LSB set
		[DataRow(new byte[] { 0xFF, 0xFF, 0xFF, 0xFF }, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF })] // Four bytes, all 1s
		[DataRow(new byte[] { 0xAA, 0xAA, 0xAA }, new byte[] { 0x55, 0x55, 0x55 })] // Three bytes, alternating bits 1010...
		[DataRow(new byte[] { 0x55, 0x55, 0x55 }, new byte[] { 0xAA, 0xAA, 0xAA })] // Three bytes, alternating bits 0101...
		[DataRow(new byte[] { 0x00, 0xFF, 0x00, 0xFF }, new byte[] { 0x00, 0xFF, 0x00, 0xFF })] // Four bytes, alternating 00000000 and 11111111
		public void ReverseBits_ByteArray_ShouldReverseBits(byte[] bytes, byte[] expected)
		{
			byte[] actual = bytes.ReverseBits();

			// Trace for debugging
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Input bytes: {(bytes == null ? "" : BitConverter.ToString(bytes))}");
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Expected bytes: {BitConverter.ToString(expected)}");
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Actual bytes: {BitConverter.ToString(actual)}");

			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i]);
			}
		}

		public static readonly byte[] ByteReverseLookup = new byte[]
		{
			0x00, 0x80, 0x40, 0xC0, 0x20, 0xA0, 0x60, 0xE0, 0x10, 0x90, 0x50, 0xD0, 0x30, 0xB0, 0x70, 0xF0,
			0x08, 0x88, 0x48, 0xC8, 0x28, 0xA8, 0x68, 0xE8, 0x18, 0x98, 0x58, 0xD8, 0x38, 0xB8, 0x78, 0xF8,
			0x04, 0x84, 0x44, 0xC4, 0x24, 0xA4, 0x64, 0xE4, 0x14, 0x94, 0x54, 0xD4, 0x34, 0xB4, 0x74, 0xF4,
			0x0C, 0x8C, 0x4C, 0xCC, 0x2C, 0xAC, 0x6C, 0xEC, 0x1C, 0x9C, 0x5C, 0xDC, 0x3C, 0xBC, 0x7C, 0xFC,
			0x02, 0x82, 0x42, 0xC2, 0x22, 0xA2, 0x62, 0xE2, 0x12, 0x92, 0x52, 0xD2, 0x32, 0xB2, 0x72, 0xF2,
			0x0A, 0x8A, 0x4A, 0xCA, 0x2A, 0xAA, 0x6A, 0xEA, 0x1A, 0x9A, 0x5A, 0xDA, 0x3A, 0xBA, 0x7A, 0xFA,
			0x06, 0x86, 0x46, 0xC6, 0x26, 0xA6, 0x66, 0xE6, 0x16, 0x96, 0x56, 0xD6, 0x36, 0xB6, 0x76, 0xF6,
			0x0E, 0x8E, 0x4E, 0xCE, 0x2E, 0xAE, 0x6E, 0xEE, 0x1E, 0x9E, 0x5E, 0xDE, 0x3E, 0xBE, 0x7E, 0xFE,
			0x01, 0x81, 0x41, 0xC1, 0x21, 0xA1, 0x61, 0xE1, 0x11, 0x91, 0x51, 0xD1, 0x31, 0xB1, 0x71, 0xF1,
			0x09, 0x89, 0x49, 0xC9, 0x29, 0xA9, 0x69, 0xE9, 0x19, 0x99, 0x59, 0xD9, 0x39, 0xB9, 0x79, 0xF9,
			0x05, 0x85, 0x45, 0xC5, 0x25, 0xA5, 0x65, 0xE5, 0x15, 0x95, 0x55, 0xD5, 0x35, 0xB5, 0x75, 0xF5,
			0x0D, 0x8D, 0x4D, 0xCD, 0x2D, 0xAD, 0x6D, 0xED, 0x1D, 0x9D, 0x5D, 0xDD, 0x3D, 0xBD, 0x7D, 0xFD,
			0x03, 0x83, 0x43, 0xC3, 0x23, 0xA3, 0x63, 0xE3, 0x13, 0x93, 0x53, 0xD3, 0x33, 0xB3, 0x73, 0xF3,
			0x0B, 0x8B, 0x4B, 0xCB, 0x2B, 0xAB, 0x6B, 0xEB, 0x1B, 0x9B, 0x5B, 0xDB, 0x3B, 0xBB, 0x7B, 0xFB,
			0x07, 0x87, 0x47, 0xC7, 0x27, 0xA7, 0x67, 0xE7, 0x17, 0x97, 0x57, 0xD7, 0x37, 0xB7, 0x77, 0xF7,
			0x0F, 0x8F, 0x4F, 0xCF, 0x2F, 0xAF, 0x6F, 0xEF, 0x1F, 0x9F, 0x5F, 0xDF, 0x3F, 0xBF, 0x7F, 0xFF
		};

		public static IEnumerable<object[]> ByteArrayReversalData()
		{
			for (int len = 1; len <= 256; len++)
			{
				byte[] input = Enumerable.Range(1, len).Select(i => (byte)i).ToArray();

				// Use the precomputed lookup table for expected values
				byte[] expected = input.Select(b => ByteReverseLookup[b]).ToArray();

				yield return new object[] { input, expected };
			}
		}

		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of each byte in an array.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(ByteArrayReversalData), DynamicDataSourceType.Method)]
		public void ReverseBits_WhenIncrementalArray_ShouldReverseBits(byte[] bytes, byte[] expected)
		{
			byte[] actual = bytes?.ReverseBits() ?? new byte[0];

			// Trace for debugging
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Input    : {BitConverter.ToString(bytes ?? new byte[0])}");
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Expected : {BitConverter.ToString(expected)}");
			Trace.WriteLineIf(!actual.SequenceEqual(expected), $"Actual   : {BitConverter.ToString(actual)}");

			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], $"Mismatch at index {i}");
			}
		}
	}
}