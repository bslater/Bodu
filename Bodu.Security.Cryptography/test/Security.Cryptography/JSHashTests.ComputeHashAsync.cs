// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public partial class JSHashTests
	{
		/// <summary>
		/// Verifies that hashing an empty byte array using a stream returns the expected result.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public async Task ComputeHashAsync_WhenInputIsEmptyByteArray_ShouldReturnExpectedHash(HashAlgorithmVariant variant)
		{
			await this.AssertStreamHashMatchesAsync(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary>
		/// Verifies that hashing an ASCII string using a stream returns the expected result.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public async Task ComputeHashAsync_WhenInputIsSimpleTextAsciiBytes_ShouldReturnExpectedHash(HashAlgorithmVariant variant)
		{
			await this.AssertStreamHashMatchesAsync(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary>
		/// Verifies that hashing the byte sequence 0 to 255 using a stream returns the expected result.
		/// </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public async Task ComputeHashAsync_WhenInputIsByteSequence0To255_ShouldReturnExpectedHash(HashAlgorithmVariant variant)
		{
			await this.AssertStreamHashMatchesAsync(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="JSHash"> hash produces expected results
		/// using. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public async Task ComputeHashAsync_BytePrefixInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			await this.AssertHashMatchesInputPrefixesAsync(variant);
		}
	}
}