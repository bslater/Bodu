using System.Security.Cryptography;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bodu.Security.Cryptography
{
	public enum SipHashVariant
	{
		SipHash_2_4,
		SipHash_4_8
	}

	public abstract partial class SipHashTests<TTest, TAlgorithm>
		: KeyedHashAlgorithmTests<TTest, TAlgorithm, SipHashVariant>
		where TTest : HashAlgorithmTests<TTest, TAlgorithm, SipHashVariant>, new()
		where TAlgorithm : SipHash, new()
	{
		protected const int ExpectedKeySize = 16;

		/// <inheritdoc />
		protected override byte[] GenerateUniqueKey()
		{
			byte[] key = new byte[SipHash.KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(key);
			return key;
		}

		public override IEnumerable<SipHashVariant> GetHashAlgorithmVariants() => new[]
		{
			SipHashVariant.SipHash_2_4,
			SipHashVariant.SipHash_4_8
		};
	}
}