using System.Diagnostics;

namespace Bodu.Extensions
{
	public partial class NumericExtensionsTests
	{
		/// <summary>
		/// Verifies that the ReverseBytes method correctly reverses the bytes of a ushort value.
		/// </summary>
		[DataTestMethod]
		[DataRow((ushort)0xABCDU, (ushort)0xCDABU, "Reversed byte order (ABCD -> CDAB)")]
		[DataRow((ushort)0x1234U, (ushort)0x3412U, "Reversed byte order (1234 -> 3412)")]
		[DataRow((ushort)0x0102U, (ushort)0x0201U, "Reversed byte order (0102 -> 0201)")]
		[DataRow((ushort)0xFFFFU, (ushort)0xFFFFU, "All bits set, no change")]
		[DataRow((ushort)0x0000U, (ushort)0x0000U, "All bits zero, no change")]
		[DataRow((ushort)0xAAAAU, (ushort)0xAAAAU, "Alternating pattern with symmetry")]
		[DataRow((ushort)0x1234U, (ushort)0x3412U, "Arbitrary non-palindromic value")]
		public void ReverseBytes_UShort_ShouldReverseBytes(ushort value, ushort expected, string test)
		{
			ushort actual = value.ReverseBytes();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X4")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X4")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X4")}");

			Assert.AreEqual(expected, actual, test);
		}

		/// <summary>
		/// Verifies that the ReverseBytes method correctly reverses the bytes of a uint value.
		/// </summary>
		[DataTestMethod]
		[DataRow(0xABCD1234U, 0x3412CDABU, "Reversed byte order (ABCD1234 -> 3412CDAB)")]
		[DataRow(0x12345678U, 0x78563412U, "Reversed byte order (12345678 -> 78563412)")]
		[DataRow(0x01020304U, 0x04030201U, "Reversed byte order (01020304 -> 04030201)")]
		[DataRow(0xFFFFFFFFU, 0xFFFFFFFFU, "All bits set, no change")]
		[DataRow(0x00000000U, 0x00000000U, "All bits zero, no change")]
		[DataRow(0x55555555U, 0x55555555U, "Alternating pattern, no change")]
		[DataRow(0x12345678U, 0x78563412U, "Arbitrary non-palindromic value")]
		public void ReverseBytes_UInt_ShouldReverseBytes(uint value, uint expected, string test)
		{
			uint actual = value.ReverseBytes();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X8")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X8")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X8")}");

			Assert.AreEqual(expected, actual, test);
		}

		/// <summary>
		/// Verifies that the ReverseBytes method correctly reverses the bytes of a ulong value.
		/// </summary>
		[DataTestMethod]
		[DataRow(0xABCD1234567890ABUL, 0xAB9078563412CDABUL, "Reversed byte order (ABCD1234567890AB -> AB9078563412CDAB)")]
		[DataRow(0x0102030405060708UL, 0x0807060504030201UL, "Reversed byte order (0102030405060708 -> 0807060504030201)")]
		[DataRow(0x1234567890ABCDEFUL, 0xEFCDAB9078563412UL, "Reversed byte order (1234567890ABCDEF -> EFCDAB9078563412)")]
		[DataRow(0x0000000000000000UL, 0x0000000000000000UL, "All bits zero, no change")]
		[DataRow(0xFFFFFFFFFFFFFFFFUL, 0xFFFFFFFFFFFFFFFFUL, "All bits set, no change")]
		[DataRow(0x5555555555555555UL, 0x5555555555555555UL, "Alternating pattern, no change")]
		[DataRow(0x1234567890ABCDEFUL, 0xEFCDAB9078563412UL, "Arbitrary non-palindromic value")]
		public void ReverseBytes_ULong_ShouldReverseBytes(ulong value, ulong expected, string test)
		{
			ulong actual = value.ReverseBytes();

			Trace.WriteLineIf(actual != expected, $"value   : {value.ToString("X16")}");
			Trace.WriteLineIf(actual != expected, $"expected: {expected.ToString("X16")}");
			Trace.WriteLineIf(actual != expected, $"actual  : {actual.ToString("X16")}");

			Assert.AreEqual(expected, actual, test);
		}
	}
}