using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents a symmetric encryption algorithm that supports an additional tweak value in addition to the encryption key and
	/// initialization vector (IV).
	/// </summary>
	/// <remarks>
	/// <para>
	/// This abstract class extends the <see cref="SymmetricAlgorithm" /> base class to support tweakable ciphers, such as Threefish, which
	/// use an additional tweak input to influence encryption.
	/// </para>
	/// <para>
	/// The tweak is provided as a byte array and may vary in size depending on the algorithm implementation. Valid tweak sizes are
	/// specified via the <see cref="LegalTweakSizes" /> property.
	/// </para>
	/// <para>
	/// Derived classes must override the encryptor and decryptor creation methods to incorporate the tweak into the encryption or
	/// decryption process.
	/// </para>
	/// </remarks>
	public abstract class TweakableSymmetricAlgorithm
		: System.Security.Cryptography.SymmetricAlgorithm
	{
		/// <summary>
		/// Specifies the tweak sizes, in bits, that are supported by the algorithm.
		/// </summary>
		protected KeySizes[] LegalTweakSizesValue = Array.Empty<KeySizes>();

		/// <summary>
		/// Stores the currently configured tweak size in bits.
		/// </summary>
		protected int TweakSizeValue = 0;

		/// <summary>
		/// Stores the current tweak value.
		/// </summary>
		protected byte[] TweakValue = Array.Empty<byte>();

		/// <summary>
		/// Gets the tweak sizes, in bits, that are supported by the symmetric algorithm.
		/// </summary>
		/// <returns>An array of <see cref="KeySizes" /> values indicating valid tweak sizes.</returns>
		public virtual KeySizes[] LegalTweakSizes =>
			this.LegalTweakSizesValue.ToArray();

		/// <summary>
		/// Gets or sets the tweak value for the algorithm.
		/// </summary>
		/// <value>The tweak value as a byte array.</value>
		/// <exception cref="ArgumentNullException">Thrown if the value is <see langword="null" />.</exception>
		/// <exception cref="CryptographicException">
		/// Thrown if the tweak value has not been set, or if the provided <paramref name="value" /> length is not supported by <see cref="LegalTweakSizes" />.
		/// </exception>
		/// <remarks>
		/// The provided value must conform to one of the allowed sizes defined in <see cref="LegalTweakSizes" />. A defensive copy is made
		/// during assignment and retrieval.
		/// </remarks>
		public virtual byte[] Tweak
		{
			get
			{
				ThrowIfTweakNotSet();

				return TweakValue.ToArray();
			}

			set
			{
				this.ThrowIfInvalidTweakSize(value);

				TweakValue = value.ToArray();
				TweakSizeValue = value.Length * 8;
			}
		}

		/// <summary>
		/// Gets or sets the size, in bits, of the tweak used by the algorithm.
		/// </summary>
		/// <value>The configured tweak size, in bits, as defined by the implementation.</value>
		/// <exception cref="CryptographicException">Thrown if the specified tweak size is not supported by <see cref="LegalTweakSizes" />.</exception>
		/// <remarks>
		/// When a new tweak size is assigned, the current <see cref="Tweak" /> value is cleared and reset to an empty byte array. This
		/// ensures that any previously configured tweak does not remain valid if its length no longer matches the new size.
		/// </remarks>
		public virtual int TweakSize
		{
			get => this.TweakSizeValue;

			set
			{
				this.ThrowIfInvalidTweakSize(value);

				this.TweakSizeValue = value;
				this.TweakValue = Array.Empty<byte>();
			}
		}

		/// <summary>
		/// Gets a read-only span view of the current tweak value.
		/// </summary>
		/// <remarks>
		/// This property exposes the current tweak as a <see cref="ReadOnlySpan{Byte}" /> for efficient access in span-based cryptographic
		/// operations. If no tweak has been set, this span is empty.
		/// </remarks>
		protected ReadOnlySpan<byte> TweakSpan =>
			this.TweakValue.AsSpan(0, this.TweakSizeValue / 8);

		/// <inheritdoc />
		public override ICryptoTransform CreateDecryptor() =>
			this.CreateDecryptor(this.Key, this.IV, this.Tweak);

		/// <summary>
		/// Creates a symmetric decryptor object using the specified key, IV, and tweak.
		/// </summary>
		/// <param name="rgbKey">The encryption key.</param>
		/// <param name="rgbIV">The initialization vector.</param>
		/// <param name="tweak">The tweak value to use.</param>
		/// <returns>An <see cref="ICryptoTransform" /> instance for decryption.</returns>
		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak);

		/// <inheritdoc />
		public override ICryptoTransform CreateEncryptor() =>
			this.CreateEncryptor(this.Key, this.IV, this.Tweak);

		/// <summary>
		/// Creates a symmetric encryptor object using the specified key, IV, and tweak.
		/// </summary>
		/// <param name="rgbKey">The encryption key.</param>
		/// <param name="rgbIV">The initialization vector.</param>
		/// <param name="tweak">The tweak value to use.</param>
		/// <returns>An <see cref="ICryptoTransform" /> instance for encryption.</returns>
		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak);

		/// <summary>
		/// Generates a new tweak value for use by the algorithm.
		/// </summary>
		/// <remarks>
		/// This method should populate the <see cref="Tweak" /> property with a valid value based on the current <see cref="TweakSize" />.
		/// Implementations must ensure the generated tweak conforms to <see cref="LegalTweakSizes" />.
		/// </remarks>
		public abstract void GenerateTweak();

		/// <summary>
		/// Attempts to create a symmetric encryptor using span-based key, IV, and tweak values.
		/// </summary>
		/// <param name="key">The encryption key as a read-only span.</param>
		/// <param name="iv">The initialization vector as a read-only span.</param>
		/// <param name="tweak">The tweak value as a read-only span.</param>
		/// <param name="transform">The resulting <see cref="ICryptoTransform" /> instance if successful.</param>
		/// <returns><see langword="true" /> if the encryptor was created successfully; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method provides a span-based alternative to <see cref="CreateEncryptor(byte[], byte[], byte[])" />. The default
		/// implementation converts the inputs to arrays. Override for custom span handling.
		/// </remarks>
		public virtual bool TryCreateEncryptor(
			ReadOnlySpan<byte> key,
			ReadOnlySpan<byte> iv,
			ReadOnlySpan<byte> tweak,
			out ICryptoTransform transform)
		{
			transform = this.CreateEncryptor(key.ToArray(), iv.ToArray(), tweak.ToArray());
			return true;
		}

		/// <summary>
		/// Attempts to create a symmetric decryptor using span-based key, IV, and tweak values.
		/// </summary>
		/// <param name="key">The encryption key as a read-only span.</param>
		/// <param name="iv">The initialization vector as a read-only span.</param>
		/// <param name="tweak">The tweak value as a read-only span.</param>
		/// <param name="transform">The resulting <see cref="ICryptoTransform" /> instance if successful.</param>
		/// <returns><see langword="true" /> if the decryptor was created successfully; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// This method provides a span-based alternative to <see cref="CreateDecryptor(byte[], byte[], byte[])" />. The default
		/// implementation converts the inputs to arrays. Override for custom span handling.
		/// </remarks>
		public virtual bool TryCreateDecryptor(
			ReadOnlySpan<byte> key,
			ReadOnlySpan<byte> iv,
			ReadOnlySpan<byte> tweak,
			out ICryptoTransform transform)
		{
			transform = this.CreateDecryptor(key.ToArray(), iv.ToArray(), tweak.ToArray());
			return true;
		}

		/// <summary>
		/// Determines whether the specified tweak size, in bits, is valid for this algorithm.
		/// </summary>
		/// <param name="length">The tweak size in bits.</param>
		/// <returns><see langword="true" /> if the size is valid; otherwise, <see langword="false" />.</returns>
		public bool ValidTweakSize(int length)
		{
			foreach (var size in this.LegalTweakSizes)
			{
				if (length < size.MinSize || length > size.MaxSize)
					continue;

				if (size.SkipSize == 0)
					return length == size.MinSize;

				if ((length - size.MinSize) % size.SkipSize == 0)
					return true;
			}

			return false;
		}

		/// <summary>
		/// Throws a <see cref="CryptographicException" /> if the tweak has not been set or generated.
		/// </summary>
		/// <exception cref="CryptographicException">
		/// Thrown when the tweak value is empty, indicating it has not been initialized or generated.
		/// </exception>
		/// <remarks>
		/// This method ensures that the <see cref="Tweak" /> property is not accessed in an uninitialized state. It is recommended to call
		/// <see cref="GenerateTweak" /> or assign a value to <see cref="Tweak" /> before use.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfTweakNotSet()
		{
			if (TweakValue is null || TweakValue.Length == 0)
				throw new CryptographicException(ResourceStrings.CryptographicException_TweakNotSet);
		}

		/// <summary>
		/// Throws if the specified <paramref name="tweak" /> is <see langword="null" /> or its size is not valid.
		/// </summary>
		/// <param name="tweak">The tweak value to validate.</param>
		/// <param name="paramName">The name of the parameter, automatically inferred if not specified.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="tweak" /> is <see langword="null" />.</exception>
		/// <exception cref="CryptographicException">Thrown when <paramref name="tweak" /> is not supported by <see cref="LegalTweakSizes" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfInvalidTweakSize(
			byte[] tweak,
			[CallerArgumentExpression("_tweakSchedule")] string? paramName = null)
		{
			ArgumentNullException.ThrowIfNull(tweak, paramName);
			this.ThrowIfInvalidTweakSize(tweak.Length * 8, paramName);
		}

		/// <summary>
		/// Throws if the specified bit length is not a valid tweak size for this algorithm.
		/// </summary>
		/// <param name="bitLength">The tweak size in bits.</param>
		/// <param name="paramName">The name of the parameter, automatically inferred if not specified.</param>
		/// <exception cref="CryptographicException">Thrown when <paramref name="bitLength" /> is not supported by <see cref="LegalTweakSizes" />.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfInvalidTweakSize(
			int bitLength,
			[CallerArgumentExpression("bitLength")] string? paramName = null)
		{
			if (!ValidTweakSize(bitLength))
			{
				string validSizes = string.Join(", ",
					this.LegalTweakSizes.Select(ks =>
						ks.SkipSize == 0
							? ks.MinSize.ToString()
							: $"{ks.MinSize}-{ks.MaxSize} step {ks.SkipSize}"));

				throw new CryptographicException(
					string.Format(ResourceStrings.CryptographicException_InvalidTweakSize, bitLength, validSizes));
			}
		}

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (disposing && TweakValue.Length > 0)
			{
				CryptographicOperations.ZeroMemory(this.TweakValue);
				this.TweakValue = Array.Empty<byte>();
			}

			base.Dispose(disposing);
		}
	}
}