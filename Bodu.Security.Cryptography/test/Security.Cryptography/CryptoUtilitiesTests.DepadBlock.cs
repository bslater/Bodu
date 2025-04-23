﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	public partial class CryptoUtilitiesTests
	{
		/// <summary>
		/// Provides valid input inputs for depadding tests, with expected unpadded outputs.
		/// </summary>
		public static IEnumerable<object[]> ValidDepaddingCases => new[]
		{
			// Typical valid depad scenarios
			new object[] { PaddingMode.PKCS7, "1020300505050505", "102030" },
			new object[] { PaddingMode.PKCS7, "0102030404040404", "01020304" },
			new object[] { PaddingMode.PKCS7, "0808080808080808", "" },
			new object[] { PaddingMode.PKCS7, "1020304050607001", "10203040506070" },
			new object[] { PaddingMode.ANSIX923, "1020300000000005", "102030" },
			new object[] { PaddingMode.ANSIX923, "0000000000000008", "" },
			new object[] { PaddingMode.ANSIX923, "1020304050607001", "10203040506070" },
			new object[] { PaddingMode.Zeros, "1020300000000000", "1020300000000000" },
			new object[] { PaddingMode.None, "1020304050607080", "1020304050607080" },

			// Full block input that was input
			new object[] { PaddingMode.PKCS7, "11223344556677880808080808080808", "1122334455667788" },
			new object[] { PaddingMode.ANSIX923, "11223344556677880000000000000008", "1122334455667788" },

			// Aligned input with no padding required
			new object[] { PaddingMode.Zeros, "0001020304050607", "0001020304050607" },
			new object[] { PaddingMode.None, "0001020304050607", "0001020304050607" },

			// Empty input with single-byte block (PKCS7)
			new object[] { PaddingMode.PKCS7, "01", "" },

			// Non-power-of-two block size
			new object[] { PaddingMode.PKCS7, "01060606060606", "01" },

			// Empty input input
			new object[] { PaddingMode.PKCS7, "0808080808080808", "" }
		};

		/// <summary>
		/// Provides invalid input blocks for DepadBlock that should raise padding validation exceptions.
		/// </summary>
		public static IEnumerable<object[]> InvalidDepaddingCases => new[]
		{
			// PKCS7: inconsistent padding values
			new object[] { PaddingMode.PKCS7, "1020304050607000", 8, typeof(CryptographicException) }, // Pad count = 00 → invalid
			new object[] { PaddingMode.PKCS7, "1020304005050505", 8, typeof(CryptographicException) }, // Pad count = 05 → last 5 bytes must be 05 05 05 05 05
			new object[] { PaddingMode.PKCS7, "0102030405060709", 8, typeof(CryptographicException) }, // Pad count = 09 → greater than block size (8) → invalid

			// ANSIX923: non-zero padding bytes before final byte
			new object[] { PaddingMode.ANSIX923, "1020304050607000", 8, typeof(CryptographicException) }, // Pad count = 00 → invalid
			new object[] { PaddingMode.ANSIX923, "4142FF000004", 6, typeof(CryptographicException) },  // Pad count = 04 → last 4 bytes must be 00 00 00 04
			new object[] { PaddingMode.ANSIX923, "41420000FF04", 6, typeof(CryptographicException) },  // Pad count = 04 → last 4 bytes must be 00 00 00 04

			// ISO10126: technically passes on randomness but pad count is invalid
			new object[] { PaddingMode.ISO10126, "1020304050607000", 8, typeof(CryptographicException) }, // Pad count = 00 → invalid
			new object[] { PaddingMode.ISO10126, "ABCD05060709FF11", 8, typeof(CryptographicException) }, // Pad count = 11 (decimal 17) → exceeds block size (8)

			// Zeros: source block not aligned
			new object[] { PaddingMode.Zeros, "4142000000", 8, typeof(ArgumentException) }, // InputHex length is 5 → not a multiple of 8 → not aligned

			// None: source block not aligned
			new object[] { PaddingMode.None, "1122334455", 8, typeof(ArgumentException) }, // InputHex length is 5 → not a multiple of 8 → not aligned

			// General: block size is 0 or negative
			new object[] { PaddingMode.PKCS7, "010203", 0, typeof(ArgumentOutOfRangeException) }, // Block Size <= 0
			new object[] { PaddingMode.PKCS7, "010203", -4, typeof(ArgumentOutOfRangeException) }, // Block Size <= 0

			// General: unsupported mode
			new object[] { (PaddingMode)999, "0102030405060708", 8, typeof(CryptographicException) }, // Padding mode not defined in PaddingMode enum

			// General: empty blocks
			new object[] { PaddingMode.None, string.Empty, 8, typeof(ArgumentException) }, // Empty input
			new object[] { PaddingMode.PKCS7, string.Empty, 8, typeof(ArgumentException) },
			new object[] { PaddingMode.Zeros, string.Empty, 8, typeof(ArgumentException) },
			new object[] { PaddingMode.ISO10126, string.Empty, 8, typeof(ArgumentException) },
		};

		/// <summary>
		/// Verifies that DepadBlock removes padding and returns the expected unpadded bytes using the byte[] overload.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), typeof(CryptoUtilitiesTests))]
		public void DepadBlock_WhenValidInput_ShouldReturnOriginalBytes_UsingByteArray(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			byte[] expected = Convert.FromHexString(expectedHex);

			var result = CryptoUtilities.DepadBlock(padding, input.Length, input, 0, input.Length);

			CollectionAssert.AreEqual(expected, result);
		}

		/// <summary>
		/// Verifies that DepadBlock removes padding and returns the expected byte count using the Span overload.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), typeof(CryptoUtilitiesTests))]
		public void DepadBlock_WhenValidInput_ShouldReturnOriginalBytes_UsingSpan(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			byte[] expected = Convert.FromHexString(expectedHex);

			Span<byte> destination = input.Length <= 128
				? stackalloc byte[input.Length]
				: new byte[input.Length];

			_ = CryptoUtilities.DepadBlock(padding, input.Length, input, destination);

			Assert.IsTrue(destination.Slice(0, expected.Length).SequenceEqual(expected));
		}

		/// <summary>
		/// Verifies that DepadBlock removes padding and returns the expected byte count using the Span overload.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), typeof(CryptoUtilitiesTests))]
		public void DepadBlock_WhenValidInput_ShouldReturnExpectedLength_UsingSpan(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			byte[] expected = Convert.FromHexString(expectedHex);

			Span<byte> destination = input.Length <= 128
				? stackalloc byte[input.Length]
				: new byte[input.Length];

			var result = CryptoUtilities.DepadBlock(padding, input.Length, input, destination);

			Assert.AreEqual(expected.Length, result);
		}

		/// <summary>
		/// Verifies that DepadBlock throws appropriate exceptions for invalid input across padding modes.
		/// </summary>
		/// <param name="padding">The padding mode being tested.</param>
		/// <param name="inputHex">The input hex string expected to fail.</param>
		/// <param name="blockSize">The block size in bytes.</param>
		/// <param name="exceptionType">The expected exception type.</param>
		[TestMethod]
		[DynamicData(nameof(InvalidDepaddingCases), typeof(CryptoUtilitiesTests))]
		public void DepadBlock_WhenInvalidInput_ShouldThrowExpectedException_UsingByteArray(
			PaddingMode padding, string inputHex, int blockSize, Type exceptionType)
		{
			byte[] input = Convert.FromHexString(inputHex);

			try
			{
				_ = CryptoUtilities.DepadBlock(padding, blockSize, input, 0, input.Length);
				Assert.Fail($"Expected {exceptionType.Name} was not thrown.");
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOfType(ex, exceptionType);
			}
		}
	}
}