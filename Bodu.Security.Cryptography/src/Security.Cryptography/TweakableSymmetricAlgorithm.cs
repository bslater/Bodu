﻿using Bodu.Extensions;
using System;
using System.Diagnostics.CodeAnalysis;
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
		/// <remarks>
		/// This backing field defines the range of acceptable tweak sizes for a given algorithm. It is used internally by the
		/// <see cref="LegalTweakSizes" /> property.
		/// </remarks>
		[MaybeNull] protected KeySizes[] LegalTweakSizesValue = null!;

		/// <summary>
		/// Stores the currently configured tweak size, in bits, for the algorithm instance.
		/// </summary>
		/// <remarks>
		/// This backing field represents the effective size of the tweak currently configured via <see cref="TweakSize" /> or <see cref="Tweak" />.
		/// </remarks>
		protected int TweakSizeValue = 0;

		/// <summary>
		/// Stores the current tweak value used by the algorithm.
		/// </summary>
		/// <remarks>
		/// The value stored here is used internally and may be cleared or regenerated via <see cref="Dispose" />,
		/// <see cref="GenerateTweak" />, or changes to <see cref="TweakSize" />. Defensive copies are used when accessing or assigning
		/// through the <see cref="Tweak" /> property.
		/// </remarks>
		protected byte[]? TweakValue = null!;

		private bool _disposed;

		/// <summary>
		/// Gets the tweak sizes, in bits, that are supported by the symmetric algorithm.
		/// </summary>
		/// <value>
		/// An array of <see cref="KeySizes" /> instances indicating the valid minimum, maximum, and step sizes supported for tweak values.
		/// </value>
		/// <remarks>
		/// This property returns a cloned copy of the internal tweak size definitions to prevent external modification. Override this
		/// property in derived types to define custom tweak size support for a specific algorithm.
		/// </remarks>
		public virtual KeySizes[] LegalTweakSizes =>
			(KeySizes[])LegalTweakSizesValue.Clone();

		/// <summary>
		/// Gets or sets the tweak value for the symmetric algorithm.
		/// </summary>
		/// <value>A byte array representing the tweak to be used. Must conform to one of the valid lengths specified by <see cref="LegalTweakSizes" />.</value>
		/// <exception cref="ArgumentNullException">Thrown when the value is <see langword="null" />.</exception>
		/// <exception cref="CryptographicException">
		/// Thrown when the tweak size is invalid or the tweak has not been set and <see cref="TweakSize" /> is missing or invalid.
		/// </exception>
		/// <remarks>
		/// If the tweak is not explicitly set, it will be lazily generated via <see cref="GenerateTweak" />. Defensive copies are made
		/// during assignment and retrieval to prevent external mutation.
		/// </remarks>
		public virtual byte[] Tweak
		{
			get
			{
				ThrowIfDisposed();
				if (TweakValue == null)
					GenerateTweak();

				return TweakValue.Copy()!; // defensive copy
			}

			set
			{
				ThrowIfDisposed();
				ArgumentNullException.ThrowIfNull(value);

				ThrowIfInvalidTweakSize(value.Length * 8);

				TweakValue = value.Copy(); // defensive copy
				TweakSizeValue = value.Length * 8;
			}
		}

		/// <summary>
		/// Gets or sets the size, in bits, of the tweak value for the algorithm.
		/// </summary>
		/// <value>An integer representing the bit length of the tweak. Must match one of the values defined in <see cref="LegalTweakSizes" />.</value>
		/// <exception cref="CryptographicException">Thrown when the value does not match a valid tweak size defined in <see cref="LegalTweakSizes" />.</exception>
		/// <remarks>Changing this value clears the current tweak. A new one can be generated via <see cref="GenerateTweak" />.</remarks>
		public virtual int TweakSize
		{
			get
			{
				ThrowIfDisposed();
				return TweakSizeValue;
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidTweakSize(value);

				TweakSizeValue = value;
				TweakValue = null; // Clear previous tweak
			}
		}

		/// <inheritdoc />
		public override ICryptoTransform CreateDecryptor() =>
			CreateDecryptor(Key, IV, Tweak);

		/// <inheritdoc />
		public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[]? rgbIV) =>
			CreateDecryptor(rgbKey, rgbIV, Tweak);

		/// <summary>
		/// Creates a symmetric decryptor using the specified key, initialization vector (IV), and tweak value.
		/// </summary>
		/// <param name="rgbKey">The secret key to use for decryption.</param>
		/// <param name="rgbIV">The initialization vector to use for the decryption operation.</param>
		/// <param name="tweak">The tweak value that modifies the decryption process.</param>
		/// <returns>An <see cref="ICryptoTransform" /> instance that can be used to perform the decryption.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="rgbKey" />, <paramref name="rgbIV" />, or <paramref name="tweak" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CryptographicException">
		/// Thrown if any input does not conform to the expected size, format, or algorithm-specific constraints.
		/// </exception>
		/// <remarks>
		/// This method must be implemented by derived types to support decryption with a tweak, as required by tweakable block ciphers such
		/// as Threefish.
		/// </remarks>
		public abstract ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak);

		/// <inheritdoc />
		public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[]? rgbIV) =>
			CreateEncryptor(rgbKey, rgbIV, Tweak);

		/// <inheritdoc />
		public override ICryptoTransform CreateEncryptor() =>
			CreateEncryptor(Key, IV, Tweak);

		/// <summary>
		/// Creates a symmetric encryptor using the specified key, initialization vector (IV), and tweak value.
		/// </summary>
		/// <param name="rgbKey">The secret key to use for encryption.</param>
		/// <param name="rgbIV">The initialization vector to use for the encryption operation.</param>
		/// <param name="tweak">The tweak value that modifies the encryption process.</param>
		/// <returns>An <see cref="ICryptoTransform" /> instance that can be used to perform the encryption.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="rgbKey" />, <paramref name="rgbIV" />, or <paramref name="tweak" /> is <see langword="null" />.
		/// </exception>
		/// <exception cref="CryptographicException">
		/// Thrown if any input does not conform to the expected size, format, or algorithm-specific constraints.
		/// </exception>
		/// <remarks>
		/// This method must be implemented by derived types to support encryption with a tweak, as required by tweakable block ciphers such
		/// as Threefish.
		/// </remarks>
		public abstract ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV, byte[] tweak);

		/// <summary>
		/// Generates a new tweak value for the algorithm based on the current <see cref="TweakSize" />.
		/// </summary>
		/// <exception cref="CryptographicException">Thrown if <see cref="TweakSize" /> is not configured to a valid size.</exception>
		/// <remarks>
		/// <para>
		/// This method initializes the tweak with random or algorithm-specific data. The generated size will match the current
		/// <see cref="TweakSize" />. If no size has been set, an exception will be thrown.
		/// </para>
		/// </remarks>
		public abstract void GenerateTweak();

		/// <summary>
		/// Determines whether the specified tweak size, in bits, is supported by the algorithm.
		/// </summary>
		/// <param name="length">The tweak size to validate, in bits.</param>
		/// <returns>
		/// <see langword="true" /> if the specified size matches any of the valid configurations in <see cref="LegalTweakSizes" />;
		/// otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method checks the specified bit length against all entries in <see cref="LegalTweakSizes" />. A size is considered valid if
		/// it falls within the defined range and aligns with the specified skip size (if any).
		/// </para>
		/// </remarks>
		public bool ValidTweakSize(int length)
		{
			foreach (var size in LegalTweakSizes)
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

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					if (TweakValue is not null && TweakValue.Length > 0)
					{
						CryptographicOperations.ZeroMemory(TweakValue);
						TweakValue = Array.Empty<byte>();
					}

					TweakSizeValue = 0;
				}

				_disposed = true;
			}

			base.Dispose(disposing);
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
			[CallerArgumentExpression("TweakSchedule")] string? paramName = null)
		{
			ArgumentNullException.ThrowIfNull(tweak, paramName);
			ThrowIfInvalidTweakSize(tweak.Length * 8, paramName);
		}

		/// <summary>
		/// Throws an exception if the specified bit length is not a valid tweak size for this algorithm.
		/// </summary>
		/// <param name="bitLength">The length of the tweak in bits.</param>
		/// <param name="paramName">The name of the calling parameter.</param>
		/// <exception cref="CryptographicException">Thrown if the specified bit length is not among the legal sizes defined by <see cref="LegalTweakSizes" />.</exception>
		/// <remarks>This method should be used internally to validate programmatic assignment to <see cref="TweakSize" /> or <see cref="Tweak" />.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfInvalidTweakSize(
			int bitLength,
			[CallerArgumentExpression("bitLength")] string? paramName = null)
		{
			if (!ValidTweakSize(bitLength))
				throw new CryptographicException(
					string.Format(ResourceStrings.CryptographicException_InvalidTweakSize, bitLength, CryptoHelpers.FormatLegalSizes(LegalTweakSizes)));
		}

		/// <summary>
		/// Throws if the tweak has not been set or generated.
		/// </summary>
		/// <exception cref="CryptographicException">Thrown if the internal tweak value is <see langword="null" /> or empty.</exception>
		/// <remarks>
		/// Call this method before using the <see cref="Tweak" /> or <see cref="TweakSpan" /> properties to ensure that the tweak has been initialized.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected void ThrowIfTweakNotSet()
		{
			if (TweakValue is null || TweakValue.Length == 0)
				throw new CryptographicException(ResourceStrings.CryptographicException_TweakNotSet);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void ThrowIfDisposed()
		{
			if (_disposed)
				throw new ObjectDisposedException(GetType().Name);
		}
	}
}