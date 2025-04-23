// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Security.Cryptography;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
    public partial class Elf64Tests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Seed 0)",
				Factory = () => new Elf64 { Seed = 0 },
				ExpectedHash_ForEmptyByteArray = "0000000000000000",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC9912066B07",
				ExpectedHash_ForByteSequence0To255 = "0C794F30E5FBF7DF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"0000000000000000",
					"0000000000000000",
					"0000000000000001",
					"0000000000000012",
					"0000000000000123",
					"0000000000001234",
					"0000000000012345",
					"0000000000123456",
					"0000000001234567",
					"0000000012345678",
					"0000000123456789",
					"000000123456789A",
					"00000123456789AB",
					"0000123456789ABC",
					"000123456789ABCD",
					"00123456789ABCDE",
					"0123456789ABCDEF",
					"023456789ABCDF10",
					"03456789ABCDF131",
					"0456789ABCDF1312",
					"056789ABCDF13173",
					"06789ABCDF131714",
					"0789ABCDF1317135",
					"089ABCDF13171316",
					"09ABCDF1317131F7",
					"0ABCDF1317131F18",
					"0BCDF1317131F139",
					"0CDF1317131F131A",
					"0DF1317131F1317B",
					"0F1317131F13171C",
					"01317131F131712D",
					"0317131F131712FE",
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Seed 31)",
				Factory = () => new Elf64 { Seed = 31 },
				ExpectedHash_ForEmptyByteArray = "000000000000001F",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC9912066937",
				ExpectedHash_ForByteSequence0To255 = "0C794F30E530F7DF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"000000000000001F",
					"00000000000001F0",
					"0000000000001F01",
					"000000000001F012",
					"00000000001F0123",
					"0000000001F01234",
					"000000001F012345",
					"00000001F0123456",
					"0000001F01234567",
					"000001F012345678",
					"00001F0123456789",
					"0001F0123456789A",
					"001F0123456789AB",
					"01F0123456789ABC",
					"0F0123456789ABDD",
					"00123456789ABD2E",
					"0123456789ABD2EF",
					"023456789ABD2F10",
					"03456789ABD2F131",
					"0456789ABD2F1312",
					"056789ABD2F13173",
					"06789ABD2F131714",
					"0789ABD2F1317135",
					"089ABD2F13171316",
					"09ABD2F1317131F7",
					"0ABD2F1317131F18",
					"0BD2F1317131F139",
					"0D2F1317131F131A",
					"02F1317131F1316B",
					"0F1317131F1316EC",
					"01317131F1316E2D",
					"0317131F1316E2FE",
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Seed 131)",
				Factory = () => new Elf64 { Seed = 131 },
				ExpectedHash_ForEmptyByteArray = "0000000000000083",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC99120663F7",
				ExpectedHash_ForByteSequence0To255 = "0C794F30E6A4F7DF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"0000000000000083",
					"0000000000000830",
					"0000000000008301",
					"0000000000083012",
					"0000000000830123",
					"0000000008301234",
					"0000000083012345",
					"0000000830123456",
					"0000008301234567",
					"0000083012345678",
					"0000830123456789",
					"000830123456789A",
					"00830123456789AB",
					"0830123456789ABC",
					"030123456789AB4D",
					"00123456789AB4EE",
					"0123456789AB4EEF",
					"023456789AB4EF10",
					"03456789AB4EF131",
					"0456789AB4EF1312",
					"056789AB4EF13173",
					"06789AB4EF131714",
					"0789AB4EF1317135",
					"089AB4EF13171316",
					"09AB4EF1317131F7",
					"0AB4EF1317131F18",
					"0B4EF1317131F139",
					"04EF1317131F131A",
					"0EF1317131F131FB",
					"0F1317131F131F2C",
					"01317131F131F22D",
					"0317131F131F22FE",
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Seed 1313)",
				Factory = () => new Elf64 { Seed = 1313 },
				ExpectedHash_ForEmptyByteArray = "0000000000000521",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC991207F957",
				ExpectedHash_ForByteSequence0To255 = "0C794F30EEAAF7DF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"0000000000000521",
					"0000000000005210",
					"0000000000052101",
					"0000000000521012",
					"0000000005210123",
					"0000000052101234",
					"0000000521012345",
					"0000005210123456",
					"0000052101234567",
					"0000521012345678",
					"0005210123456789",
					"005210123456789A",
					"05210123456789AB",
					"0210123456789AEC",
					"010123456789AEED",
					"00123456789AEECE",
					"0123456789AEECEF",
					"023456789AEECF10",
					"03456789AEECF131",
					"0456789AEECF1312",
					"056789AEECF13173",
					"06789AEECF131714",
					"0789AEECF1317135",
					"089AEECF13171316",
					"09AEECF1317131F7",
					"0AEECF1317131F18",
					"0EECF1317131F139",
					"0ECF1317131F134A",
					"0CF1317131F1345B",
					"0F1317131F13450C",
					"01317131F134502D",
					"0317131F134502FE",
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Seed 13131)",
				Factory = () => new Elf64 { Seed = 13131 },
				ExpectedHash_ForEmptyByteArray = "000000000000334B",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC991204E077",
				ExpectedHash_ForByteSequence0To255 = "0C794F22B5BCF7DF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"000000000000334B",
					"00000000000334B0",
					"0000000000334B01",
					"000000000334B012",
					"00000000334B0123",
					"0000000334B01234",
					"000000334B012345",
					"00000334B0123456",
					"0000334B01234567",
					"000334B012345678",
					"00334B0123456789",
					"0334B0123456789A",
					"034B01234567899B",
					"04B012345678998C",
					"0B0123456789988D",
					"001234567899886E",
					"01234567899886EF",
					"0234567899886F10",
					"034567899886F131",
					"04567899886F1312",
					"0567899886F13173",
					"067899886F131714",
					"07899886F1317135",
					"0899886F13171316",
					"099886F1317131F7",
					"09886F1317131F18",
					"0886F1317131F109",
					"086F1317131F102A",
					"06F1317131F1023B",
					"0F1317131F1023AC",
					"01317131F1023A2D",
					"0317131F1023A2FE",
				}
			},
			new HashAlgorithmVariant
			{
				Name = "Elf64 (Max Seed)",
				Factory = () => new Elf64 { Seed = ulong.MaxValue },
				ExpectedHash_ForEmptyByteArray = "FFFFFFFFFFFFFFFF",
				ExpectedHash_ForSimpleTextAsciiBytes = "06CBBC9912066B37",
				ExpectedHash_ForByteSequence0To255 = "0D7C684A770B4AEF",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"FFFFFFFFFFFFFFFF",
					"0FFFFFFFFFFFFF00",
					"0FFFFFFFFFFFF0F1",
					"0FFFFFFFFFFF0FE2",
					"0FFFFFFFFFF0FED3",
					"0FFFFFFFFF0FEDC4",
					"0FFFFFFFF0FEDCB5",
					"0FFFFFFF0FEDCBA6",
					"0FFFFFF0FEDCBA97",
					"0FFFFF0FEDCBA988",
					"0FFFF0FEDCBA9879",
					"0FFF0FEDCBA9876A",
					"0FF0FEDCBA98765B",
					"0F0FEDCBA987654C",
					"00FEDCBA9876543D",
					"0FEDCBA9876543DE",
					"0EDCBA9876543D1F",
					"0DCBA9876543D2E0",
					"0CBA9876543D2EC1",
					"0BA9876543D2ECE2",
					"0A9876543D2ECE83",
					"09876543D2ECE8E4",
					"0876543D2ECE8EC5",
					"076543D2ECE8ECE6",
					"06543D2ECE8ECE07",
					"0543D2ECE8ECE0E8",
					"043D2ECE8ECE0EC9",
					"03D2ECE8ECE0ECEA",
					"0D2ECE8ECE0ECE8B",
					"02ECE8ECE0ECE81C",
					"0ECE8ECE0ECE81FD",
					"0CE8ECE0ECE81F0E",
				}
			}
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="Elf64Tests" /> class and yields
		/// test case data for each <see cref="HashAlgorithmVariant" /> defined in
		/// <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the
		/// <see cref="Elf64" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single
		/// <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new Elf64Tests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		/// <summary> Verifies that <see cref="Bernstein"> produces expected hash for an empty byte
		/// array. </summary>
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

		/// <summary> Verifies that hashing progressively longer prefixes using the <see
		/// cref="Bernstein"> hash produces expected results using <see
		/// cref="HashAlgorithm.ComputeHash(byte[], int, int)"/>. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_BytePrefixInput_ShouldMatchEachExpectedHash(HashAlgorithmVariant variant)
		{
			AssertHashMatchesInputPrefixes(variant);
		}
	}
}