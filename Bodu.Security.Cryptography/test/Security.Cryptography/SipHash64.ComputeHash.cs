// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Text;
using Bodu.Infrastructure;

namespace Bodu.Security.Cryptography
{
	public partial class SipHash64Tests
	{
		/// <inheritdoc />
		public override IEnumerable<HashAlgorithmVariant> GetVariants() => new[]
		{
			new HashAlgorithmVariant
			{
				Name = "SipHash64 (default)",
				Factory = () => this.CreateAlgorithm(),
				ExpectedHash_ForEmptyByteArray = "310E0EDD47DB6F72",
				ExpectedHash_ForSimpleTextAsciiBytes = "0B75CE46D5178CD7",
				ExpectedHash_ForByteSequence0To255 = "D7BFA7D226059D99",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"A0DA70C05CEB4C3C",// []
					"8DC5FB49AA0B5A8B",	// [0]
					"256C37276F50C362",	// [0,1]
					"E9DF7F0E9FA70F68",	// [0,1,2]
					"C8BE4AE23EDF7BEF",	// [0,1,2,3]
					"5B78394C87CEF168",	// [0,1,2,3,4]
					"AF93D7D717E8011F",	// [0,1,2,3,4,5]
					"0B48112CAF7ED6B3",	// ...
					"38792FFC241C2BC7",
					"220BA6ADB67A0E61",
					"123412AD1DD39A78",
					"79D75EF69AC46DCA",
					"F0AACE2734317611",
					"35F262DD70196F3C",
					"AB1C121581046ADA",
					"6313894ED47C56D0",
					"7F898FD82E6302C9",
					"F2EEDF0E80245AE0",
					"B6F87F8EBA75BAF2",
					"DF6087D34361DA74",
					"E596B229CADB4B4D",
					"BF163B608C3BA8F0",
					"57717BD1EB96E771",
					"E4AA0DA6E1D5E527",
					"2B142FEC6ED44EC2",
					"09465390767AFE59",
					"B430E506F644B094",
					"2297EF8D325FBDEF",
					"88EC7E85CB476F8E",
					"16A2F319A093766C",
					"64376C3C15998DDA",
					"D97C4426BD3D52A4",
				},
				ExpectedHash_ForHashTestVectors= new []
				{
					// https://131002.net/siphash/siphash.pdf 15-byte string: 000102···0c0d0e 16-byte key: 000102···0d0e0f expected hash: a129ca6149be45e5
					new HashAlgorithmTestCase{
						InputHex = Encoding.ASCII.GetString(Enumerable.Range(0, 15).Select(i => (byte)i).ToArray()),
						ExpectedHex = Convert.ToHexString(Convert.FromHexString("A129CA6149BE45E5").Reverse().ToArray())
					},
					new HashAlgorithmTestCase { InputHex = "Hello", ExpectedHex = "8CBB0B17D23FD563" },
				}
			},
		};

		/// <summary>
		/// Provides test case data for parameterized tests using the <c>DynamicData</c> attribute.
		/// </summary>
		/// <remarks>
		/// This method creates a new instance of the <see cref="SipHash64Tests" /> class and yields test case data for each
		/// <see cref="HashAlgorithmVariant" /> defined in <see cref="GetVariants" />. It is intended to be used as a data source for
		/// <c>[DataTestMethod]</c> tests that validate different configurations or behaviors of the <see cref="SipHash64" /> hash algorithm.
		/// </remarks>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> of object arrays, each containing a single <see cref="HashAlgorithmVariant" /> instance.
		/// </returns>
		public static IEnumerable<object[]> GetTestVariants()
		{
			var instance = new SipHash64Tests();
			foreach (var variant in instance.GetVariants())
				yield return new object[] { variant };
		}

		public static IEnumerable<object[]> GetValidKeyLengths()
		{
			var instance = new SipHash64Tests();
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

		/// <summary> Verifies that <see cref="SipHash64"> produces expected hash for an empty byte array. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsEmpty_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.EmptyByteArray, variant.ExpectedHash_ForEmptyByteArray);
		}

		/// <summary> Verifies <see cref="SipHash64"> hash for simple ASCII text. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsAsciiText_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.SimpleTextAsciiBytes, variant.ExpectedHash_ForSimpleTextAsciiBytes);
		}

		/// <summary> Verifies <see cref="SipHash64"> hash for byte sequence 0-255. </summary>
		[DataTestMethod]
		[DynamicData(nameof(GetTestVariants), DynamicDataSourceType.Method)]
		public void ComputeHash_WhenInputIsByteSequence0To255_ShouldMatchExpected(HashAlgorithmVariant variant)
		{
			AssertHashMatches(variant, CryptoTestUtilities.ByteSequence0To255, variant.ExpectedHash_ForByteSequence0To255);
		}

		/// <summary> Verifies that hashing progressively longer prefixes using the <see cref="SipHash64"> hash produces expected results
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