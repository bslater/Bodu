using System;
using System.Linq;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides a managed implementation of the Threefish-256 symmetric block cipher that integrates with the .NET cryptographic framework.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class supports a variety of custom cipher block modes (e.g., CBC, CFB, OFB, CTR) through the <see cref="BlockMode" /> property.
	/// The base class <see cref="SymmetricAlgorithm.Mode" /> is not used and will throw if set.
	/// </para>
	/// </remarks>
	public sealed class Threefish256 : SymmetricAlgorithm
	{
		private const int BlockSizeBits = 256;
		private const int BlockSizeBytes = BlockSizeBits / 8;
		private const int KeySizeBits = 256;
		private const int KeySizeBytes = KeySizeBits / 8;
		private const int TweakSizeBytes = 16;

		/// <summary>
		/// Initializes a new instance of the <see cref="Threefish256" /> class with default settings.
		/// </summary>
		public Threefish256()
		{
			BlockSizeValue = BlockSizeBits;
			KeySizeValue = KeySizeBits;
			FeedbackSizeValue = 8;

			LegalBlockSizesValue = new[] { new KeySizes(BlockSizeBits, BlockSizeBits, 0) };
			LegalKeySizesValue = new[] { new KeySizes(KeySizeBits, KeySizeBits, 0) };

			// Satisfy framework expectations with defaults
			ModeValue = CipherMode.CBC;
			Padding = PaddingMode.PKCS7;

			GenerateKey();
			GenerateIV();
		}

		/// <summary>
		/// Gets or sets the cipher mode used by this algorithm.
		/// </summary>
		/// <remarks>
		/// This property replaces the standard <see cref="SymmetricAlgorithm.Mode" /> to support extended block modes like CTR or XTS.
		/// </remarks>
		public CipherBlockMode BlockMode { get; set; } = CipherBlockMode.CBC;

		/// <inheritdoc />
		public override CipherMode Mode
		{
			get => ModeValue;
			set => throw new NotSupportedException("The standard CipherMode is not supported. Use the BlockMode property instead.");
		}

		/// <summary>
		/// Gets or sets the 128-bit tweak value used in Threefish cipher operations.
		/// </summary>
		public byte[] Tweak { get; set; } = new byte[TweakSizeBytes];

		/// <inheritdoc />
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
		{
			ValidateKeyAndIv(rgbKey, rgbIV);
			var engine = new Threefish256Engine(rgbKey, Tweak);

			return new ThreefishTransform(
				engine,
				BlockMode,
				Padding,
				rgbIV,
				encrypt: false);
		}

		/// <inheritdoc />
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
		{
			ValidateKeyAndIv(rgbKey, rgbIV);
			var engine = new Threefish256Engine(rgbKey, Tweak);

			return new ThreefishTransform(
				engine,
				BlockMode,
				Padding,
				rgbIV,
				encrypt: true);
		}

		/// <inheritdoc />
		public override void GenerateIV() =>
			IVValue = RandomBytes(BlockSizeBytes);

		/// <inheritdoc />
		public override void GenerateKey() =>
			KeyValue = RandomBytes(KeySizeBytes);

		private static byte[] RandomBytes(int length)
		{
			var buffer = new byte[length];
			RandomNumberGenerator.Fill(buffer);
			return buffer;
		}

		private static void ValidateKeyAndIv(byte[] key, byte[] iv)
		{
			if (key is null || key.Length != KeySizeBytes)
				throw new CryptographicException($"Key must be {KeySizeBytes} bytes (256 bits).");

			if (iv is null || iv.Length != BlockSizeBytes)
				throw new CryptographicException($"IV must be {BlockSizeBytes} bytes (256 bits).");
		}
	}
}