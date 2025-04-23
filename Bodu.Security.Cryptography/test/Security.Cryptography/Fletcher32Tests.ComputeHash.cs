// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public partial class Fletcher32Tests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "Fletcher32",
				Factory = () => new Fletcher32(),
				ExpectedHash_ForEmptyByteArray = "00000000",
				ExpectedHash_ForSimpleTextAsciiBytes = "53CD5B8D",
				ExpectedHash_ForByteSequence0To255 = "75553FC0",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"00000000", // []
					"00000000",	// [0]
					"01000100",	// [0,1]
					"02020102",	// [0,1,2]
					"05020402",	// [0,1,2,3]
					"09080406",	// [0,1,2,3,4]
					"0E080906",	// [0,1,2,3,4,5]
					"1714090C",	// ...
					"1E14100C",
					"2E281014",
					"37281914",
					"5046191E",
					"5B46241E",
					"7F70242A",
					"8C70312A",
					"BDA83138",
				},
				ExpectedHash_ForHashTestVectors= new []
				{
					new HashAlgorithmTestCase{ InputHex="abcde", ExpectedHex="F04FC729" },
					new HashAlgorithmTestCase{ InputHex="abcdef", ExpectedHex="56502D2A" },
					new HashAlgorithmTestCase{ InputHex="abcdefgh", ExpectedHex="EBE19591" },
				}
			},
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="Fletcher32Tests" /> class and yields test case data for each
		/// <see cref="HashAlgorithmVariant" /> defined in <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the <see cref="Fletcher32" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new Fletcher32Tests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		/// <summary> Verifies that <see cref="Fletcher32"> produces expected hash for an empty byte array. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsEmpty_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary> Verifies <see cref="Fletcher32"> hash for simple ASCII text. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsAsciiText_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary> Verifies <see cref="Fletcher32"> hash for byte sequence 0-255. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsByteSequence0To255_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="Fletcher32"> hash produces expected results
		/// using. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_BytePrefixInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			AssertHashMatchesInputPrefixes(variant);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="Fletcher16"> hash produces expected results
		/// using. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_TestVectorInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			AssertHashMatchesTestVector(variant);
		}
	}
}