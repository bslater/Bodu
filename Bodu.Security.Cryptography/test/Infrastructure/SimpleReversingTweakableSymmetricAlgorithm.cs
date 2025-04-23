using Bodu.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Infrastructure
{
	/// <summary>
	/// A test implementation of <see cref="TweakableSymmetricAlgorithm" /> that produces a simple reversing crypto transform, with optional
	/// tweak input for controlled transformation variation.
	/// </summary>
	/// <remarks>This class is used for validating tweakable crypto flows in test scenarios. It is not cryptographically secure.</remarks>
	public sealed class SimpleReversingTweakableSymmetricAlgorithm
		: Security.Cryptography.TweakableSymmetricAlgorithm
	{
		public const int DefaultBlockSize = 0x100;
		public const int DefaultTweakSize = 0x100;

		public static readonly KeySizes[] BlockSizesValue = new[]
		{
			new KeySizes(128, 256, 64),		// Supports: 128, 192, 256
			new KeySizes(448, 576, 128),	// Supports: 448, 576
			new KeySizes(1024, 2048, 512)	// Supports: 1024, 1536, 2048
		};

		public SimpleReversingTweakableSymmetricAlgorithm()
		{
			BlockSizeValue = DefaultBlockSize;
			KeySizeValue = DefaultBlockSize;
			TweakSizeValue = DefaultTweakSize;
			FeedbackSizeValue = DefaultBlockSize;

			LegalBlockSizesValue = BlockSizesValue;
			LegalKeySizesValue = BlockSizesValue;
			LegalTweakSizesValue = BlockSizesValue;
		}

		public override int BlockSize
		{
			get => base.BlockSize;
			set => base.BlockSize = base.KeySize = base.TweakSize = value;
		}

		public override int KeySize
		{
			get => base.KeySize;
			set => base.KeySize = base.BlockSize = base.TweakSize = value;
		}

		public override int TweakSize
		{
			get => base.TweakSize;
			set => base.TweakSize = base.BlockSize = base.KeySize = value;
		}

		public override void GenerateIV()
			=> IVValue = CryptoUtilities.GetRandomNonZeroBytes(BlockSize / 8);

		public override void GenerateKey()
			=> KeyValue = CryptoUtilities.GetRandomNonZeroBytes(KeySize / 8);

		public override void GenerateTweak()
			=> TweakValue = CryptoUtilities.GetRandomNonZeroBytes(TweakSize / 8);

		/// <summary>
		/// Fills the provided span with a randomly generated key.
		/// </summary>
		public bool TryGenerateKey(Span<byte> destination)
		{
			int required = KeySize / 8;
			if (destination.Length < required)
				return false;

			CryptoUtilities.FillWithRandomNonZeroBytes(destination.Slice(0, required));
			return true;
		}

		/// <summary>
		/// Fills the provided span with a randomly generated tweak.
		/// </summary>
		public bool TryGenerateTweak(Span<byte> destination)
		{
			int required = TweakSize / 8;
			if (destination.Length < required)
				return false;

			CryptoUtilities.FillWithRandomNonZeroBytes(destination.Slice(0, required));
			return true;
		}

		/// <summary>
		/// Explicitly sets the tweak size in bits.
		/// </summary>
		public void SetTweakSize(int size)
			=> TweakSizeValue = size;

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV)
			=> NewEncryptor(ModeValue, rgbKey, rgbIV, null, FeedbackSizeValue, TransformMode.Encrypt);

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV)
			=> NewEncryptor(ModeValue, rgbKey, rgbIV, null, FeedbackSizeValue, TransformMode.Decrypt);

		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak)
			=> NewEncryptor(ModeValue, rgbKey, rgbIV, tweak, FeedbackSizeValue, TransformMode.Encrypt);

		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak)
			=> NewEncryptor(ModeValue, rgbKey, rgbIV, tweak, FeedbackSizeValue, TransformMode.Decrypt);

		/// <summary>
		/// Attempts to create an encryptor using the specified spans for key, IV, and tweak.
		/// </summary>
		public bool TryCreateEncryptor(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, ReadOnlySpan<byte> tweak, out ICryptoTransform? transform)
		{
			transform = null;

			if (key.Length != KeySize / 8 ||
				iv.Length != BlockSize / 8 ||
				tweak.Length != TweakSize / 8)
				return false;

			transform = new SimpleReversingCryptoTransform(
				BlockSize,
				FeedbackSize,
				key.ToArray(),
				iv.ToArray(),
				tweak.ToArray(),
				Mode,
				Padding,
				TransformMode.Encrypt
			);

			return true;
		}

		/// <summary>
		/// Attempts to create a decryptor using the specified spans for key, IV, and tweak.
		/// </summary>
		public bool TryCreateDecryptor(ReadOnlySpan<byte> key, ReadOnlySpan<byte> iv, ReadOnlySpan<byte> tweak, out ICryptoTransform? transform)
		{
			transform = null;

			if (key.Length != KeySize / 8 ||
				iv.Length != BlockSize / 8 ||
				tweak.Length != TweakSize / 8)
				return false;

			transform = new SimpleReversingCryptoTransform(
				BlockSize,
				FeedbackSize,
				key.ToArray(),
				iv.ToArray(),
				tweak.ToArray(),
				Mode,
				Padding,
				TransformMode.Decrypt
			);

			return true;
		}

		/// <summary>
		/// Creates a transform using byte[] fallbacks.
		/// </summary>
		private ICryptoTransform NewEncryptor(CipherMode cipherMode, byte[]? rgbKey, byte[]? rgbIV, byte[]? tweak, int feedbackSize, TransformMode encryptMode)
		{
			rgbKey ??= CryptoUtilities.GetRandomNonZeroBytes(KeySizeValue / 8);
			rgbIV ??= CryptoUtilities.GetRandomNonZeroBytes(BlockSizeValue / 8);
			tweak ??= CryptoUtilities.GetRandomNonZeroBytes(TweakSizeValue / 8);

			return new SimpleReversingCryptoTransform(
				BlockSizeValue,
				feedbackSize,
				rgbKey,
				rgbIV,
				tweak,
				cipherMode,
				PaddingValue,
				encryptMode
			);
		}
	}
}