// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Blowfish.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

using Bodu.Extensions;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Implementation of the <see cref="Bodu.Security.Cryptography.Blowfish" /> tweakable large block cipher. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This algorithm is a symmetric-key tweakable large block cipher that supports key lengths of 256, 512, or 1024 bits; defaulting to
	/// 256 bites. The block size is the same size as the key, and the tweak value is 128-bits for all block sizes.
	/// </para>
	/// <para>
	/// The <see cref="Bodu.Security.Cryptography.Blowfish" /> algorithm was designed by Bruce Schneier, Niels Ferguson, Stefan Lucks, Doug
	/// Whiting, Mihir Bellare, Tadayoshi Kohno, Jon Callas, and Jesse Walker. For further details see <a href="https://en.wikipedia.org/wiki/Blowfish">https://en.wikipedia.org/wiki/Blowfish</a>.
	/// </para>
	/// </remarks>
	public sealed class Blowfish
		: System.Security.Cryptography.SymmetricAlgorithm
	{
		private const int CipherBlockSize = 0x40; // 64 bits
		private const int CipherKeySize = 0x1C0; // 448 bits
		private static readonly KeySizes[] BlockSizesValue = new[] { new KeySizes(0x40, 0x40, 0x00) };
		private static readonly KeySizes[] KeySizesValue = new[] { new KeySizes(0x20, Blowfish.CipherKeySize, 0x01) };

		/// <summary>
		/// Initializes a new instance of the <see cref="Bodu.Security.Cryptography.Blowfish" /> class.
		/// </summary>
#if FEATURE_CRYPTO

       /// <exception cref="System.InvalidOperationException">
       /// This implementation is not part of the Windows Platform FIPS-validated cryptographic algorithms.
       /// </exception>
#endif // FEATURE_CRYPTO

		public Blowfish()
		{
#if FEATURE_CRYPTO
         if (CryptoConfig.AllowOnlyFipsAlgorithms) throw new InvalidOperationException(Resources.Cryptography_NonCompliantFIPSAlgorithm);
#endif // FEATURE_CRYPTO

			this.BlockSizeValue = Blowfish.CipherBlockSize;
			this.LegalBlockSizesValue = Blowfish.BlockSizesValue;
			this.KeySizeValue = Blowfish.CipherKeySize;
			this.LegalKeySizesValue = Blowfish.KeySizesValue;
			this.FeedbackSizeValue = Blowfish.CipherBlockSize;
		}

		/// <summary>
		/// Gets or sets the mode for operation of the cryptographic algorithm.
		/// </summary>
		/// <returns>One of the enumeration values that specifies the block cipher mode to use for encryption. The default is <see cref="System.Security.Cryptography.CipherMode.CBC" />.</returns>
		/// <remarks>
		/// <para>See <see cref="System.Security.Cryptography.CipherMode" /> enumeration for a description of specific modes.</para>
		/// </remarks>
		/// <exception cref="System.Security.Cryptography.CryptographicException">
		/// <see cref="Bodu.Security.Cryptography.Blowfish.Mode" /> is set to <see cref="System.Security.Cryptography.CipherMode.CTS" />.
		/// </exception>
		public override CipherMode Mode
		{
			get => this.ModeValue;

			set
			{
				if (value.Equals(CipherMode.CTS)) throw new CryptographicException(Resources.Cryptography_Invalid_Value(nameof(System.Security.Cryptography.CipherMode).Humanize()));

				this.ModeValue = value;
			}
		}

		/// <summary>
		/// Creates a cryptographic object to perform the <see cref="Bodu.Security.Cryptography.Blowfish" /> algorithm.
		/// </summary>
		/// <returns>A cryptographic object.</returns>
#if FEATURE_CRYPTO

      /// <exception cref="System.Reflection.TargetInvocationException">
      /// The algorithm was used with Federal Information Processing Standards (FIPS) mode enabled, but is not FIPS compatible.
      /// </exception>
#endif // FEATURE_CRYPTO

		public new static Blowfish Create()
		{
			return Create(typeof(Blowfish).FullName);
		}

		/// <summary>
		/// Creates a cryptographic object to perform the specified implementation of the <see cref="Bodu.Security.Cryptography.Blowfish" /> algorithm.
		/// </summary>
		/// <param name="algName">The name of the specific implementation of <see cref="Bodu.Security.Cryptography.Blowfish" /> to create.</param>
		/// <returns>A cryptographic object.</returns>
#if FEATURE_CRYPTO

       /// <exception cref="System.Reflection.TargetInvocationException">
       /// The algorithm described by the <paramref name="algName" /> parameter was used with Federal Information Processing Standards
       /// (FIPS) mode enabled, but is not FIPS compatible.
       /// </exception>
#endif // FEATURE_CRYPTO

		public new static Blowfish Create(string algName)
		{
			return (Blowfish)CryptoConfig.CreateFromName(algName);
		}

		/// <summary>
		/// Creates a symmetric <see cref="Bodu.Security.Cryptography.Blowfish" /> decryptor object with the specified
		/// <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key" /> and initialization vector ( <see cref="System.Security.Cryptography.SymmetricAlgorithm.IV" />).
		/// </summary>
		/// <param name="rgbKey">The secret key to be used for the symmetric algorithm.</param>
		/// <param name="rgbIV">The IV to be used for the symmetric algorithm.</param>
		/// <returns>A symmetric <see cref="Bodu.Security.Cryptography.Blowfish" /> decryptor object.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="rgbKey" /> or <paramref name="rgbIV" /> is <see langword="null" /> (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="System.Security.Cryptography.CryptographicException">
		/// The value of the <see cref="P:System.Security.Cryptography.SymmetricAlgorithm.Mode" /> property is not
		/// <see cref="System.Security.Cryptography.CipherMode.ECB" />, <see cref="System.Security.Cryptography.CipherMode.CBC" />, or <see cref="System.Security.Cryptography.CipherMode.CFB" />.
		/// </exception>
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
			=> this.NewEncryptor(this.ModeValue, rgbKey, rgbIV, this.FeedbackSizeValue, TransformMode.Decrypt);

		/// <summary>
		/// Creates a symmetric <see cref="Bodu.Security.Cryptography.Blowfish" /> encryptor object with the specified
		/// <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key" /> and initialization vector ( <see cref="System.Security.Cryptography.SymmetricAlgorithm.IV" />).
		/// </summary>
		/// <param name="rgbKey">The secret key to be used for the symmetric algorithm.</param>
		/// <param name="rgbIV">The IV to be used for the symmetric algorithm.</param>
		/// <returns>A symmetric <see cref="System.Security.Cryptography.Rijndael" /> encryptor object.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// <paramref name="rgbKey" /> or <paramref name="rgbIV" /> is <see langword="null" /> (Nothing in Visual Basic).
		/// </exception>
		/// <exception cref="System.Security.Cryptography.CryptographicException">
		/// The value of the <see cref="System.Security.Cryptography.SymmetricAlgorithm.Mode" /> property is not
		/// <see cref="System.Security.Cryptography.CipherMode.ECB" />, <see cref="System.Security.Cryptography.CipherMode.CBC" />, or <see cref="System.Security.Cryptography.CipherMode.CFB" />.
		/// </exception>
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
			=> this.NewEncryptor(this.ModeValue, rgbKey, rgbIV, this.FeedbackSizeValue, TransformMode.Encrypt);

		/// <summary>
		/// Generates a random initialization vector ( <see cref="System.Security.Cryptography.SymmetricAlgorithm.IV" />) to be used for the algorithm.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In general, there is no reason to use this method, because the
		/// <see cref="O:Bodu.Security.Cryptography.Blowfish.CreateEncryptor" /> overload methods will automatically generates an
		/// initialization vector, key and tweak. However, you may want to use the
		/// <see cref="Bodu.Security.Cryptography.Blowfish.GenerateIV" /> method to reuse a symmetric algorithm instance with a different
		/// initialization vector.
		/// </para>
		/// </remarks>
		public override void GenerateIV()
			=> this.IVValue = CryptoUtilities.GetRandomNonZeroBytes(this.BlockSizeValue / 8);

		/// <summary>
		/// Generates a random <see cref="System.Security.Cryptography.SymmetricAlgorithm.Key" /> to be used for the algorithm.
		/// </summary>
		/// <remarks>
		/// <para>
		/// In general, there is no reason to use this method, because the
		/// <see cref="O:Bodu.Security.Cryptography.Blowfish.CreateEncryptor" /> overload methods will automatically generates an
		/// initialization vector, key and tweak. However, you may want to use the
		/// <see cref="Bodu.Security.Cryptography.Blowfish.GenerateKey" /> method to reuse a symmetric algorithm instance with a different key.
		/// </para>
		/// </remarks>
		public override void GenerateKey()
			=> this.KeyValue = CryptoUtilities.GetRandomNonZeroBytes(this.KeySizeValue / 8);

		private ICryptoTransform NewEncryptor(CipherMode cipherMode, byte[] rgbKey, byte[] rgbIV, int feedbackSize, TransformMode encryptMode)
		{
			if (rgbKey == null) rgbKey = CryptoUtilities.GetRandomNonZeroBytes(this.KeySizeValue / 8);
			if (rgbIV == null) rgbIV = CryptoUtilities.GetRandomNonZeroBytes(this.BlockSizeValue / 8);

			return new BlowfishTransform(rgbKey, rgbIV, cipherMode, this.PaddingValue, encryptMode);
		}
	}
}