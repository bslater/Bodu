using System.Diagnostics;

namespace Bodu.Extensions
{
	[TestClass]
	public class NumberExtensionsTests
	{
		// Byte tests
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a byte value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow((byte)0b_00000001, (byte)0b_10000000)] // single bit set (least significant bit)
		public void ReverseBits_Byte_ShouldReverseBits(byte value, byte expected)
		{
			byte reversed = value.ReverseBits();
			Trace.WriteLineIf(reversed != expected, $"value   : {(Convert.ToString((byte)value, 2)).PadLeft(sizeof(byte) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"expected: {(Convert.ToString((byte)expected, 2)).PadLeft(sizeof(byte) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"result  : {(Convert.ToString((byte)reversed, 2)).PadLeft(sizeof(byte) * 8, '0')}");

			Assert.AreEqual(expected, reversed, "Byte ReverseBits did not produce the correct result.");
		}

		// UShort tests
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a ushort value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow((ushort)0b_0000000000000001, (ushort)0b_1000000000000000)] // single bit set (least significant bit)
		public void ReverseBits_UShort_ShouldReverseBits(ushort value, ushort expected)
		{
			ushort reversed = value.ReverseBits();
			Trace.WriteLineIf(reversed != expected, $"value   : {(Convert.ToString((short)value, 2)).PadLeft(sizeof(short) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"expected: {(Convert.ToString((short)expected, 2)).PadLeft(sizeof(short) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"result  : {(Convert.ToString((short)reversed, 2)).PadLeft(sizeof(short) * 8, '0')}");

			Assert.AreEqual(expected, reversed, "UShort ReverseBits did not produce the correct result.");
		}

		// Int tests
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of an int value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow(0b_00000000000000000000000000000001, 0b_10000000000000000000000000000000)] // single bit set (least significant bit)
		public void ReverseBits_Int_ShouldReverseBits(int value, uint expected)
		{
			int reversed = value.ReverseBits();
			Trace.WriteLineIf(reversed != expected, $"value   : {(Convert.ToString((int)value, 2)).PadLeft(sizeof(int) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"expected: {(Convert.ToString((int)expected, 2)).PadLeft(sizeof(int) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"result  : {(Convert.ToString((int)reversed, 2)).PadLeft(sizeof(int) * 8, '0')}");

			Assert.AreEqual((int)expected, reversed);
		}

		// Long tests
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a long value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow(0b_0000000000000000000000000000000101010101010101, 0b_10101010101010101010101010101010000000000000000)] // single bit set (least significant bit)
		public void ReverseBits_Long_ShouldReverseBits(long value, long expected)
		{
			long reversed = value.ReverseBits();
			Trace.WriteLineIf(reversed != expected, $"value   : {(Convert.ToString((long)value, 2)).PadLeft(sizeof(long) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"expected: {(Convert.ToString((long)expected, 2)).PadLeft(sizeof(long) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"result  : {(Convert.ToString((long)reversed, 2)).PadLeft(sizeof(long) * 8, '0')}");

			Assert.AreEqual(expected, reversed, "Long ReverseBits did not produce the correct result.");
		}

		// ULong tests
		/// <summary>
		/// Verifies that the ReverseBits method correctly reverses the bits of a ulong value. Tests include single bit set, power of 2,
		/// alternating bits, all 1s, all 0s, and other common cases.
		/// </summary>
		[DataTestMethod]
		[DataRow(0b_0000000000000000000000000000000101010101010101_1010101010101010UL, 0b_0101010101010101010101010101010100000000000000000000000000000000UL)] // single bit set (least significant bit)
		public void ReverseBits_Ulong_ShouldReverseBits(ulong value, ulong expected)
		{
			ulong reversed = value.ReverseBits();
			Trace.WriteLineIf(reversed != expected, $"value   : {(Convert.ToString((long)value, 2)).PadLeft(sizeof(long) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"expected: {(Convert.ToString((long)expected, 2)).PadLeft(sizeof(long) * 8, '0')}");
			Trace.WriteLineIf(reversed != expected, $"result  : {(Convert.ToString((long)reversed, 2)).PadLeft(sizeof(long) * 8, '0')}");

			Assert.AreEqual(expected, reversed, "Ulong ReverseBits did not produce the correct result.");
		}
	}
}