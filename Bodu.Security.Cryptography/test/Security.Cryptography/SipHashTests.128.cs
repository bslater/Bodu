// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32Tests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Infrastructure;
using System.Text;
using static Bodu.Security.Cryptography.SipHash128Tests;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Contains unit tests for the <see cref="SipHash128" /> hash algorithm.
	/// </summary>
	[TestClass]
	public partial class SipHash128Tests
		: Security.Cryptography.SipHashTests<SipHash128Tests, SipHash128>
	{
		/// <inheritdoc />

		/// <inheritdoc />
		protected override int ExpectedInputBlockSize => 8;

		/// <inheritdoc />
		protected override int ExpectedOutputBlockSize => 8;

		/// <inheritdoc />
		protected override int MaximumLegalKeyLength => ExpectedKeySize;

		/// <inheritdoc />
		protected override int MinimumLegalKeyLength => ExpectedKeySize;

		/// <inheritdoc />
		protected override IReadOnlyList<int> ValidKeyLengths => new[] { ExpectedKeySize };

		/// <inheritdoc />
		protected override SipHash128 CreateAlgorithm() => new SipHash128
		{
			Key = GetDeterministicKey(),
			CompressionRounds = 2,
			FinalizationRounds = 4,
		};

		protected override SipHash128 CreateAlgorithm(SipHashVariant variant) =>
			variant switch
			{
				SipHashVariant.SipHash_2_4 => this.CreateAlgorithm(),
				SipHashVariant.SipHash_4_8 => new SipHash128
				{
					Key = GetDeterministicKey(),
					CompressionRounds = 4,
					FinalizationRounds = 8,
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};

		private static readonly IReadOnlyDictionary<string, byte[]> CustomInputs = new Dictionary<string, byte[]>
		{
			["Hello"] = Encoding.UTF8.GetBytes("Hello")
		};

		protected override IEnumerable<KnownAnswerTest> GetTestVectors(SipHashVariant variant)
		{
			foreach (var vector in base.GetTestVectors(variant))
				yield return vector;

			var expected = GetExpectedHashesForNamedInputs(variant);
			foreach (var (name, input) in CustomInputs)
			{
				if (expected.TryGetValue(name, out var hex))
				{
					yield return new KnownAnswerTest
					{
						Name = name,
						Input = input,
						ExpectedOutput = Convert.FromHexString(hex)
					};
				}
			}
		}

		protected override IReadOnlyDictionary<string, string> GetExpectedHashesForNamedInputs(SipHashVariant variant) =>
			variant switch
			{
				SipHashVariant.SipHash_2_4 => new Dictionary<string, string>
				{
					["Empty"] = "A3817F04BA25A8E66DF67214C7550293",
					["ABC"] = "6EDFC93C6A8C85920C6D1BFE0413F575",
					["Zeros_16"] = "D60D3284A18EBD5AF3D0F02A078007CD",
					["QuickBrownFox"] = "7628C9301AA4412555E65227CD31964E",
					["Sequential_0_255"] = "1C9BB67528165F8E468248E3799B0EAB",
					["Hello"] = "C9E2FA57B43C46560D0F6C0657D05731",
				},
				SipHashVariant.SipHash_4_8 => new Dictionary<string, string>
				{
					["Empty"] = "1F64CE586DA904E9CFECE85483A70A6C",
					["ABC"] = "2A74871B2DB4FB6B7F7167F798A760BD",
					["Zeros_16"] = "2393F374C9F5E28B5CEC1E15B0D61114",
					["QuickBrownFox"] = "3DEDE5965E71E3A16C7231C2A12B244F",
					["Sequential_0_255"] = "C7BF2FFE16C9026C3FE93166ABD4D257",
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};

		protected override IReadOnlyList<string> GetExpectedHashesForIncrementalInput(SipHashVariant variant) =>
			variant switch
			{
				SipHashVariant.SipHash_2_4 => new[]
				{
					"A3817F04BA25A8E66DF67214C7550293",
					"DA87C1D86B99AF44347659119B22FC45",
					"8177228DA4A45DC7FCA38BDEF60AFFE4",
					"9C70B60C5267A94E5F33B6B02985ED51",
					"F88164C12D9C8FAF7D0F6E7C7BCD5579",
					"1368875980776F8854527A07690E9627",
					"14EECA338B208613485EA0308FD7A15E",
					"A1F1EBBED8DBC153C0B84AA61FF08239",
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
				SipHashVariant.SipHash_4_8 => new[]
				{
					"1F64CE586DA904E9CFECE85483A70A6C",
					"47345DA8EF4C79476AF27CA791C7A280",
					"E1495FA396CA2DC62273815F188221A4",
					"C7A273844AC54E835A9CB67F81057602",
					"541F52BBF43ECE4E2A95C8E01F656DEF",
					"17973BD40DF34815244F990CBF12BE5D",
					"6B0B360D563280CDB17D56C908E1F5FF",
					"ED00E13B184BF1C2726B8B54FFD2EEE0",
					"A7D946138FF9EDF5364A5A23AFCAE063",
					"9E7314B7545CECA38B9A5549E4FB0BE8",
					"586C62C68489D168AEE65B889AB91275",
					"E67152A64CA3D147C4AB841E2F2E7A99",
					"7F1C7AEA908DE52E3E9E0883EEA816AF",
					"DE827ABF92B733923F35330DB5EF4A34",
					"597563640F379AC537678EE2354C7DF9",
					"284D03303A453A593D78F7FADC9062CB",
					"914AC7A2597F63B7C0FDE5AB8D4EAD9C",
					"0D5115A44BA455EE3A453B95CE87C3CB",
					"549B939D0BF1D8948337885A84CE7914",
					"6C179769CD348AEBD2FB13578C72B46C",
					"AAD036C138C957E0682A00EE2F86408B",
					"21B1EEC42FB670BFEE9044FF4ED73A26",
					"0593A1D62997ED374653C917463F14EB",
					"113D31627719F91EA0F1FFC68657E24E",
					"B3394CF72DE06ADD0E7314F0C252C4D6",
					"922A98DA9D35C341E2456BE4CD6389D2",
					"596B6230F757B34AA2DCEA50CBB28D4D",
					"C24EE497D55B7E800684DF756559EE48",
					"5E9CB6A136681ED45E2B9DE4DC018177",
					"BFFA39CA8656D3047933EDFE9D8178B2",
					"18229418A1D0795A357A803A8134AEA3",
					"4A3E96FF53474E2E737B69571A77B06E",
					"FED5F0F9D03772842E2F572F63F19450",
					"39335886C1F94263C40C6629C6BC446F",
					"EEA5F93BB38710B08B2C4697198BBF9F",
					"806EC7B6704F720E374312066166D43A",
					"6E69ED9DF0C939B49DAFEEAE6047B2A2",
					"93C77BF298B6F9C794A230177F2FD738",
					"FFAD9CD98C2AA875DAFF3A2A4CE60CE6",
					"4D992FFDF94A93CDCD64EF7657F510E3",
					"3270624E24E0A11EA186E096BE1BCE9B",
					"31E8BBE0CB4EFF511FFFC7C409343177",
					"CBE17D05879AD907648A12A07016AB5B",
					"8848D44370E98BE2D5D28B46366A0AFC",
					"B7FFD1B2421076A90CB5CF6554095E0C",
					"6A6B666CD523A8F6BBD884FE1FD1050C",
					"A8FE8A8350FBF5C805F18CBD30136224",
					"CCE7117AEE8236F2EB3A9694D57E62B5",
					"3A25F0E4FC28B70C6B3090BAFEF69F04",
					"3F05E626749FC48B8106F8E44431DD4A",
					"766879F97672165C0AFFD5FADC77345B",
					"4371A05AB66C598BC9C28494A1DD2F0E",
					"65F85BD3A2A5F1BA1F22B6EFD6E00266",
					"76CF61DAE54B22EFCA6A9F228AAF6611",
					"6CDCC2E39FDBA29F885390AB9DA484DA",
					"E1EEACEACC3B67B2D8E4E2617B2FAA5A",
					"0BD29F6F4CE10F1778D6B02ED5AB5A6D",
					"AD189F156A52267CE08745835B65A607",
					"0F6B9971722566D43DEC6B99E31C218F",
					"A1A4C8FA4F3DF466D3F39C6F3D9E1A74",
					"3B1A3DB88CF0C21FC1A6D8A72D9EF91D",
					"D1486802EFC0002856C3635A8A692EE5",
					"EEA15F8F7CAE1999FD564931C22C1C3C",
					"63F5AE6328C4DB93207961EE906BD4A5",
				},
				_ => throw new ArgumentOutOfRangeException(nameof(variant))
			};
	}
}