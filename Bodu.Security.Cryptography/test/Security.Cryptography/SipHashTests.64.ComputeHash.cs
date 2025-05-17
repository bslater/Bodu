// -----------------------------------------------------------------------
// <copyright file="Bernstein.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using Bodu.Infrastructure;

using System.Text;

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
				ExpectedHash_ForSimpleTextAsciiBytes = "E46F1FDC05612752",
				ExpectedHash_ForByteSequence0To255 = "D7BFA7D226059D99",
				ExpectedHash_ForInputPrefixes = new[]
				{
					"310E0EDD47DB6F72",// []
					"FD67DC93C539F874",	// [0]
					"5A4FA9D909806C0D",	// [0,1]
					"2D7EFBD796666785",	// [0,1,2]
					"B7877127E09427CF",	// [0,1,2,3]
					"8DA699CD64557618",	// [0,1,2,3,4]
					"CEE3FE586E46C9CB",	// [0,1,2,3,4,5]
					"37D1018BF50002AB",	// ...
					"6224939A79F5F593",
					"B0E4A90BDF82009E",
					"F3B9DD94C5BB5D7A",
					"A7AD6B22462FB3F4",
					"FBE50E86BC8F1E75",
					"903D84C02756EA14",
					"EEF27A8E90CA23F7",
					"E545BE4961CA29A1",
					"DB9BC2577FCC2A3F",
					"9447BE2CF5E99A69",
					"9CD38D96F0B3C14B",
					"BD6179A71DC96DBB",
					"98EEA21AF25CD6BE",
					"C7673B2EB0CBF2D0",
					"883EA3E395675393",
					"C8CE5CCD8C030CA8",
					"94AF49F6C650ADB8",
					"EAB8858ADE92E1BC",
					"F315BB5BB835D817",
					"ADCF6B0763612E2F",
					"A5C91DA7ACAA4DDE",
					"716595876650A2A6",
					"28EF495C53A387AD",
					"42C341D8FA92D832",
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