// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
    public partial class BernsteinTests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "Bernstein (Original)",
				Factory = () => new Bernstein { UseModifiedAlgorithm = false },
				ExpectedHash_ForEmptyByteArray = "00001505",
				ExpectedHash_ForSimpleTextAsciiBytes = "34CC38DE",
				ExpectedHash_ForByteSequence0To255 = "9A5B9485",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"00001505", // []
					"0002B5A5", // [0]
					"00596A46", // [0,1]
					"0B86B308", // [0,1,2]
					"7C5D140B",	// [0,1,2,3]
					"07FF956F", // [0,1,2,3,4]
					"07F24354", // [0,1,2,3,4,5]
					"063AADDA", // ...
					"CD906921",
					"7F9D8D49",
					"734F3672",
					"DD3604BC",
					"83F69C47",
					"02CA2533",
					"5C0ECBA0",
					"DDE83FAE"
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Bernstein (Modified)",
				Factory = () => new Bernstein { UseModifiedAlgorithm = true },
				ExpectedHash_ForEmptyByteArray = "00001505",
				ExpectedHash_ForSimpleTextAsciiBytes = "B679B80A",
				ExpectedHash_ForByteSequence0To255 = "E63A4D05",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"00001505", // []
					"0002B5A5", // [0]
					"00596A44", // [0,1]
					"0B86B2C6", // [0,1,2]
					"7C5D0B85", // [0,1,2,3]
					"07FE7C21", // [0,1,2,3,4]
					"07CE0044", // [0,1,2,3,4,5]
					"018E08C2", // ...
					"334F2105",
					"9D3341AD",
					"439B7744",
					"B70A5FCE",
					"98565985",
					"A3218A29",
					"0752CF44",
					"F1ACB7CA",
				}
			}
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="BernsteinTests" /> class and yields test case data for each
		/// <see cref="HashAlgorithmVariant" /> defined in <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the <see cref="Bernstein" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new BernsteinTests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		/// <summary> Verifies that <see cref="Bernstein"> produces expected hash for an empty byte array. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsEmpty_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary> Verifies <see cref="Bernstein"> hash for simple ASCII text. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsAsciiText_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary> Verifies <see cref="Bernstein"> hash for byte sequence 0-255. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsByteSequence0To255_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="Bernstein"> hash produces expected results
		/// using. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_BytePrefixInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			AssertHashMatchesInputPrefixes(variant);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="Bernstein"> hash produces expected results
		/// using. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_TestVectorInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			AssertHashMatchesTestVector(variant);
		}
	}
}