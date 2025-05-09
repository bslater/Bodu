﻿// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
    public partial class SipHash128Tests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "SipHash128 (default)",
				Factory = () => this.CreateAlgorithm(),
				ExpectedHash_ForEmptyByteArray = "A3817F04BA25A8E66DF67214C7550293",
				ExpectedHash_ForSimpleTextAsciiBytes = "7628C9301AA4412555E65227CD31964E",
				ExpectedHash_ForByteSequence0To255 = "67A00304D3834C4612EAEE7B5F579ACB",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"A3817F04BA25A8E66DF67214C7550293",// []
					"DA87C1D86B99AF44347659119B22FC45",	// [0]
					"8177228DA4A45DC7FCA38BDEF60AFFE4",	// [0,1]
					"9C70B60C5267A94E5F33B6B02985ED51",	// [0,1,2]
					"F88164C12D9C8FAF7D0F6E7C7BCD5579",	// [0,1,2,3]
					"1368875980776F8854527A07690E9627",	// [0,1,2,3,4]
					"14EECA338B208613485EA0308FD7A15E",	// [0,1,2,3,4,5]
					"A1F1EBBED8DBC153C0B84AA61FF08239",	// ...
					"3B62A9BA6258F5610F83E264F31497B4",
					"264499060AD9BAABC47F8B02BB6D71ED",
					"00110DC378146956C95447D3F3D0FBBA",
					"0151C568386B6677A2B4DC6F81E5DC18",
					"D626B266905EF35882634DF68532C125",
					"9869E247E9C08B10D029934FC4B952F7",
					"31FCEFAC66D7DE9C7EC7485FE4494902",
					"5493E99933B0A8117E08EC0F97CFC3D9",
					"6EE2A4CA67B054BBFD3315BF85230577",
					"473D06E8738DB89854C066C47AE47740",
					"A426E5E423BF4885294DA481FEAEF723",
					"78017731CF65FAB074D5208952512EB1",
					"9E25FC833F2290733E9344A5E83839EB",
					"568E495ABE525A218A2214CD3E071D12",
					"4A29B54552D16B9A469C10528EFF0AAE",
					"C9D184DDD5A9F5E0CF8CE29A9ABF691C",
					"2DB479AE78BD50D8882A8A178A6132AD",
					"8ECE5F042D5E447B5051B9EACB8D8F6F",
					"9C0B53B4B3C307E87EAEE08678141F66",
					"ABF248AF69A6EAE4BFD3EB2F129EEB94",
					"0664DA1668574B88B935F3027358AEF4",
					"AA4B9DC4BF337DE90CD4FD3C467C6AB7",
					"EA5C7F471FAF6BDE2B1AD7D4686D2287",
					"2939B0183223FAFC1723DE4F52C43D35",
					"7C3956CA5EEAFC3E363E9D556546EB68",
					"77C6077146F01C32B6B69D5F4EA9FFCF",
					"37A6986CB8847EDF0925F0F1309B54DE",
					"A705F0E69DA9A8F907241A2E923C8CC8",
					"3DC47D1F29C448461E9E76ED904F6711",
					"0D62BF01E6FC0E1A0D3C4751C5D3692B",
					"8C03468BCA7C669EE4FD5E084BBEE7B5",
					"528A5BB93BAF2C9C4473CCE5D0D22BD9",
					"DF6A301E95C95DAD97AE0CC8C6913BD8",
					"801189902C857F39E73591285E70B6DB",
					"E617346AC9C231BB3650AE34CCCA0C5B",
					"27D93437EFB721AA401821DCEC5ADF89",
					"89237D9DED9C5E78D8B1C9B166CC7342",
					"4A6D8091BF5E7D651189FA94A250B14C",
					"0E33F96055E7AE893FFC0E3DCF492902",
					"E61C432B720B19D18EC8D84BDC63151B",
					"F7E5AEF549F782CF379055A608269B16",
					"438D030FD0B7A54FA837F2AD201A6403",
					"A590D3EE4FBF04E3247E0D27F286423F",
					"5FE2C1A172FE93C4B15CD37CAEF9F538",
					"2C97325CBD06B36EB2133DD08B3A017C",
					"92C814227A6BCA949FF0659F002AD39E",
					"DCE850110BD8328CFBD50841D6911D87",
					"67F14984C7DA791248E32BB5922583DA",
					"1938F2CF72D54EE97E94166FA91D2A36",
					"74481E9646ED49FE0F6224301604698E",
					"57FCA5DE98A9D6D8006438D0583D8A1D",
					"9FECDE1CEFDC1CBED4763674D9575359",
					"E3040C00EB28F15366CA73CBD872E740",
					"7697009A6A831DFECCA91C5993670F7A",
					"5853542321F567A005D547A4F04759BD",
					"5150D1772F50834A503E069A973FBD7C",
				},
				ExpectedHash_ForHashTestVectors= new []
				{
					new HashAlgorithmTestCase{ InputHex = "Hello", ExpectedHex = "C9E2FA57B43C46560D0F6C0657D05731" }
				}
			},
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="SipHash128Tests" /> class and yields test case data for each
		/// <see cref="HashAlgorithmVariant" /> defined in <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the <see cref="SipHash128" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new SipHash128Tests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		public static IEnumerable<object[]> GetValidKeyLengths()
		{
			var instance = new SipHash128Tests();
			foreach (var length in instance.ValidKeyLengths)
				yield return new object[] { length };
		}

		/// <summary>
		/// Verifies that assigning a key of a valid length does not throw and is accepted by the algorithm.
		/// </summary>
		/// <param name="length">The valid key length to test.</param>
		[DataTestMethod]
		[DynamicData(nameof(GetValidKeyLengths), DynamicDataSourceType.Method)]
		public void Key_WhenSetToValidLength_ShouldBeAccepted(int length)
		{
			AssertValidKeyLength(this.CreateAlgorithm(), length);
		}

		/// <summary> Verifies that <see cref="SipHash128"> produces expected hash for an empty byte array. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsEmpty_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary> Verifies <see cref="SipHash128"> hash for simple ASCII text. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsAsciiText_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary> Verifies <see cref="SipHash128"> hash for byte sequence 0-255. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsByteSequence0To255_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="SipHash128"> hash produces expected results
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