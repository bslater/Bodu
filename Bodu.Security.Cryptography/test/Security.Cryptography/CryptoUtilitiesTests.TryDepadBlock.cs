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
		/// Verifies that TryDepadBlock returns <c>true</c> for valid input input across modes.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), DynamicDataSourceType.Method)]
		public void TryDepadBlock_WhenValidPadding_ShouldReturnTrue(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			Span<byte> destination = new byte[input.Length];

			bool result = CryptoUtilities.TryDepadBlock(padding, input.Length, input, destination, out int written);

			Assert.IsTrue(result);
		}

		/// <summary>
		/// Verifies that TryDepadBlock returns the correct uninput byte count for valid input.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), DynamicDataSourceType.Method)]
		public void TryDepadBlock_WhenValidPadding_ShouldReturnExpectedLength(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			byte[] expected = Convert.FromHexString(expectedHex);
			var expectedLength = expected.Length;
			Span<byte> destination = new byte[input.Length];

			CryptoUtilities.TryDepadBlock(padding, input.Length, input, destination, out int written);

			Assert.AreEqual(expectedLength, written);
		}

		/// <summary>
		/// Verifies that TryDepadBlock returns the correct uninput content for valid input.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.ValidDepaddingCases), DynamicDataSourceType.Method)]
		public void TryDepadBlock_WhenValidPadding_ShouldReturnExpectedResult(
			PaddingMode padding, string inputHex, string expectedHex)
		{
			byte[] input = Convert.FromHexString(inputHex);
			byte[] expected = Convert.FromHexString(expectedHex);
			var expectedLength = expected.Length;
			Span<byte> destination = new byte[input.Length];

			CryptoUtilities.TryDepadBlock(padding, input.Length, input, destination, out int written);

			CollectionAssert.AreEqual(input.Take(expectedLength).ToArray(), destination.Slice(0, written).ToArray());
		}

		/// <summary>
		/// Verifies that TryDepadBlock returns <c>false</c> for invalid input input across supported modes.
		/// </summary>
		[TestMethod]
		[DynamicData(nameof(CryptoUtilitiesTests.InvalidDepaddingCases), DynamicDataSourceType.Method)]
		public void TryDepadBlock_WhenInvalidPadding_ShouldReturnFalse(PaddingMode padding, string inputHex, int blockSizeBytes, Type exceptionType)
		{
			byte[] input = Convert.FromHexString(inputHex);
			Span<byte> destination = new byte[input.Length];

			bool result = CryptoUtilities.TryDepadBlock(padding, blockSizeBytes, input, destination, out int written);

			Assert.IsFalse(result);
			Assert.AreEqual(0, written);
		}
	}
}