// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public partial class Fletcher64Tests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "Fletcher64",
				Factory = () => new Fletcher64(),
				ExpectedHash_ForEmptyByteArray = "0000000000000000",
				ExpectedHash_ForSimpleTextAsciiBytes = "7CA0BCD01F153C78",
				ExpectedHash_ForByteSequence0To255 = "8D855D355F20E09F",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"0000000000000000", // []
					"0000000000000000",	// [0]
					"0000010000000100",	// [0,1]
					"0002010000020100",	// [0,1,2]
					"0302010003020100",	// [0,1,2,3]
					"0604020403020104",	// [0,1,2,3,4]
					"0604070403020604",	// [0,1,2,3,4,5]
					"060A070403080604",	// ...
					"0D0A07040A080604",
					"17120D100A08060C",
					"171216100A080F0C",
					"171C16100A120F0C",
					"221C161015120F0C",
					"372E252815120F18",
					"372E322815121C18",
					"373C322815201C18",
					"463C322824201C18",
					"6A5C4E5024201C28",
					"6A5C5F5024202D28",
					"6A6E5F5024322D28",
					"7D6E5F5037322D28",
					"B4A08C8C37322D3C",
					"B4A0A18C3732423C",
					"B4B6A18C3748423C",
					"CBB6A18C4E48423C",
					"19FFE3E04E484254",
					"19FFFCE04E485B54",
					"1919FDE04E625B54",
					"3419FDE069625B54",
					"9E7B585169625B70",
					"9E7B755169627870",
					"9E99755169807870",
				},
				ExpectedHash_ForHashTestVectors= new []
				{
					new HashAlgorithmTestCase{ InputHex="abcde", ExpectedHex="C9C6C427646362C6" },
					new HashAlgorithmTestCase{ InputHex="abcdef", ExpectedHex="C9C62A286463C8C6" },
					new HashAlgorithmTestCase{ InputHex="abcdefgh", ExpectedHex="312E2B28CCCAC8C6" },
				}
			},
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="Fletcher64Tests" /> class and yields test case data for each
		/// <see cref="HashAlgorithmVariant" /> defined in <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the <see cref="Fletcher64" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new Fletcher64Tests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		/// <summary> Verifies that <see cref="Fletcher64"> produces expected hash for an empty byte array. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsEmpty_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary> Verifies <see cref="Fletcher64"> hash for simple ASCII text. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsAsciiText_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary> Verifies <see cref="Fletcher64"> hash for byte sequence 0-255. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsByteSequence0To255_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="Fletcher64"> hash produces expected results
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