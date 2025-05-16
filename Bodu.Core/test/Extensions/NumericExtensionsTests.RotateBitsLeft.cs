using System.Diagnostics;

namespace Bodu.Extensions
{
	public partial class NumericExtensionsTests
	{
		[DataTestMethod]
		[DataRow((byte)0x00, 0, (byte)0x00)] // All bits zero, rotate by 0
		[DataRow((byte)0x01, 1, (byte)0x02)] // Single bit set (rotate left by 1)
		[DataRow((byte)0x01, 7, (byte)0x80)] // Single bit set (rotate left by 7)
		[DataRow((byte)0x80, 1, (byte)0x01)] // Only MSB set (rotate left by 1)
		[DataRow((byte)0xFF, 1, (byte)0xFF)] // All bits set (rotate left by 1)
		[DataRow((byte)0xAA, 3, (byte)0x55)] // Alternating bits (10101010 -> 01010101)
		[DataRow((byte)0x55, 5, (byte)0xAA)] // Alternating bits (01010101 -> 10101010)
		[DataRow((byte)0xFF, 8, (byte)0xFF)] // All bits set (rotate left by 8, full rotation)
		[DataRow((byte)0xAA, 8, (byte)0xAA)] // Alternating bits, rotate by 8 (full rotation)
		public void RotateBitsLeft_Byte_ShouldRotateBits(byte value, int count, byte expected)
		{
			byte actual = value.RotateBitsLeft(count);
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DataRow((ushort)0x0000, 0, (ushort)0x0000)] // All bits zero, rotate by 0
		[DataRow((ushort)0x0001, 1, (ushort)0x0002)] // Single bit set (rotate left by 1)
		[DataRow((ushort)0x8000, 1, (ushort)0x0001)] // Only MSB set (rotate left by 1)
		[DataRow((ushort)0xFFFF, 1, (ushort)0xFFFF)] // All bits set (rotate left by 1)
		[DataRow((ushort)0xAAAA, 7, (ushort)0x5555)] // Alternating bits (10101010 -> 01010101)
		[DataRow((ushort)0x5555, 9, (ushort)0xAAAA)] // Alternating bits (01010101 -> 10101010)
		[DataRow((ushort)0xFFFF, 16, (ushort)0xFFFF)] // All bits set (rotate left by 16)
		[DataRow((ushort)0xAAAA, 16, (ushort)0xAAAA)] // Alternating bits, rotate by 16 (full rotation)
		public void RotateBitsLeft_UShort_ShouldRotateBits(ushort value, int count, ushort expected)
		{
			ushort actual = value.RotateBitsLeft(count);
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DataRow(0x00000000U, 0, 0x00000000U)] // All bits zero, rotate by 0
		[DataRow(0x00000001U, 1, 0x00000002U)] // Single bit set (rotate left by 1)
		[DataRow(0x80000000U, 1, 0x00000001U)] // Only MSB set (rotate left by 1)
		[DataRow(0xFFFFFFFFU, 1, 0xFFFFFFFFU)] // All bits set (rotate left by 1)
		[DataRow(0xAAAAAAAAU, 15, 0x55555555U)] // Alternating bits (10101010 -> 01010101)
		[DataRow(0x55555555U, 17, 0xAAAAAAAAU)] // Alternating bits (01010101 -> 10101010)
		[DataRow(0x00000001U, 31, 0x80000000U)] // Single bit set (rotate left by 31)
		[DataRow(0xFFFFFFFFU, 32, 0xFFFFFFFFU)] // All bits set (rotate left by 32)
		public void RotateBitsLeft_UInt_ShouldRotateBits(uint value, int count, uint expected)
		{
			uint actual = value.RotateBitsLeft(count);
			Assert.AreEqual(expected, actual);
		}

		[DataTestMethod]
		[DataRow(0x0000000000000000UL, 0, 0x0000000000000000UL)] // All bits zero, rotate by 0
		[DataRow(0x0000000000000001UL, 1, 0x0000000000000002UL)] // Single bit set (rotate left by 1)
		[DataRow(0x8000000000000000UL, 1, 0x0000000000000001UL)] // Only MSB set (rotate left by 1)
		[DataRow(0xFFFFFFFFFFFFFFFFUL, 1, 0xFFFFFFFFFFFFFFFFUL)] // All bits set (rotate left by 1)
		[DataRow(0xAAAAAAAAAAAAAAAAUL, 31, 0x5555555555555555UL)] // Alternating bits (10101010 -> 01010101)
		[DataRow(0x5555555555555555UL, 33, 0xAAAAAAAAAAAAAAAAUL)] // Alternating bits (01010101 -> 10101010)
		[DataRow(0x0000000000000001UL, 63, 0x8000000000000000UL)] // Single bit set (rotate left by 63)
		[DataRow(0xFFFFFFFFFFFFFFFFUL, 64, 0xFFFFFFFFFFFFFFFFUL)] // All bits set (rotate left by 64)
		public void RotateBitsLeft_ULong_ShouldRotateBits(ulong value, int count, ulong expected)
		{
			ulong actual = value.RotateBitsLeft(count);
			Assert.AreEqual(expected, actual);
		}
	}
}