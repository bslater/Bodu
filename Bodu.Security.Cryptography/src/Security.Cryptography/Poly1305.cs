﻿// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Poly1305.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the message authentication code (MAC) for the input data using the <c>Poly1305</c> algorithm. This implementation enforces
	/// one-time key usage and produces a fixed 16-byte (128-bit) tag from a 256-bit key, as specified in RFC 8439.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Poly1305" /> is a cryptographic MAC function that operates by interpreting input data as a polynomial over a finite
	/// field, evaluated using a secret 128-bit key <c>r</c>, and combined with an additional 128-bit key <c>s</c> to produce the final
	/// output tag. The keys are derived from a single 256-bit (32-byte) input, where the first half is clamped to create <c>r</c> and the
	/// second half serves as the nonce-derived offset <c>s</c>.
	/// </para>
	/// <para>
	/// The algorithm is designed for speed and simplicity, particularly on systems without specialized cryptographic hardware. It is most
	/// commonly used in conjunction with a stream cipher (e.g., ChaCha20) in AEAD constructions such as <c>ChaCha20-Poly1305</c>. This
	/// implementation adheres to the <a href="https://datatracker.ietf.org/doc/html/rfc8439">RFC 8439</a> specification and computes a
	/// 16-byte tag for each input message, ensuring authenticity and integrity.
	/// </para>
	/// <para>
	/// Internally, the input is split into 16-byte blocks, each treated as a 130-bit number (with an additional high bit if full-sized),
	/// and accumulated using modular arithmetic modulo <c>2¹³⁰ - 5</c>. After processing all blocks, the accumulator is finalized by adding
	/// the second half of the key <c>s</c> and serializing the result as the final MAC tag.
	/// </para>
	/// <note type="important">This algorithm is a <b>one-time authenticator</b> and must <b>not</b> be used with the same key for multiple
	/// messages. Reusing a key with different inputs <b>compromises</b> the cryptographic security and may lead to forgery attacks.</note>
	/// </remarks>
	public sealed class Poly1305
		: KeyedBlockHashAlgorithm<Poly1305>
	{
		/// <summary>
		/// The required key size in bytes (256 bits).
		/// </summary>
		public const int KeySize = 32;

		private const uint Mask26 = 0x3ffffff;

		private static readonly int BlockSize = 16;

		private readonly uint[] _acc = new uint[5];
		private readonly uint[] _key = new uint[4];
		private readonly uint[] _r = new uint[5];    // Polynomial key

		// Encrypted nonce
		private readonly uint[] _s = new uint[4];    // Precomputed 5 * r[1..4]

		// Polynomial accumulator

		private bool disposed = false;

		/// <summary>
		/// Initializes a new instance of the <see cref="Poly1305" /> class.
		/// </summary>
		public Poly1305()
			: base(BlockSize)
		{
			HashSizeValue = 128;
			KeyValue = new byte[KeySize];
			CryptoUtilities.FillWithRandomNonZeroBytes(KeyValue);
			InitializeKey();
		}

		/// <summary>
		/// Gets a value indicating whether this transform instance can be reused after a hash operation is completed.
		/// </summary>
		/// <returns>
		/// <see langword="false" /> for <see cref="Poly1305" />, which is a one-time authenticator that must not be reused with the same key.
		/// </returns>
		/// <remarks>
		/// <para>
		/// <see cref="Poly1305" /> is a one-time message authentication code (MAC) algorithm. Reusing the same instance with the same key
		/// for multiple messages violates the security guarantees of the algorithm and may lead to forgery attacks.
		/// </para>
		/// <para>
		/// This implementation clears the key material after finalization to prevent accidental reuse. To compute a new MAC, a new instance
		/// must be created with a fresh key.
		/// </para>
		/// </remarks>
		public override bool CanReuseTransform => false;

		/// <summary>
		/// Gets a value indicating whether this transform supports processing multiple blocks of data in a single operation.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if multiple input blocks can be transformed in sequence without intermediate finalization; otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// Most hash algorithms and block ciphers support multi-block transformations for streaming input. If <see langword="false" />, the
		/// transform must be invoked one block at a time.
		/// </remarks>
		public override bool CanTransformMultipleBlocks => true;

		/// <inheritdoc />
		public override int InputBlockSize => BlockSize;

		/// <inheritdoc />
		public override byte[] Key
		{
			get
			{
				ThrowIfDisposed();
				return KeyValue.Copy();
			}

			set
			{
				ThrowIfDisposed();
				ThrowIfInvalidState();
				ThrowHelper.ThrowIfNull(value);

				if (value.Length != KeySize)
					throw new CryptographicException(string.Format(ResourceStrings.CryptographicException_InvalidKeySize, value.Length, KeySize));

				KeyValue = value.Copy();
				InitializeKey();
			}
		}

		/// <inheritdoc />
		public override int OutputBlockSize => BlockSize;

		/// <inheritdoc />
		public override void Initialize()
		{
			ThrowIfDisposed();
			base.Initialize();
			Array.Clear(_acc);

			// If KeyValue was not explicitly set or was cleared, regenerate a random key
			if (KeyValue is null || KeyValue.Length != KeySize)
			{
				KeyValue = new byte[KeySize];
				CryptoUtilities.FillWithRandomNonZeroBytes(KeyValue);
			}

			InitializeKey();
		}

		/// <summary>
		/// Releases the unmanaged resources used by the algorithm and clears the key from memory.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		/// <remarks>This override ensures all sensitive information is zero out to avoid leaking secrets before disposal.</remarks>
		protected override void Dispose(bool disposing)
		{
			if (disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);
				Array.Clear(_acc);
				Array.Clear(_r);
				Array.Clear(_key);
				Array.Clear(_s);
			}

			disposed = true;
			base.Dispose(disposing);
		}

		/// <summary>
		/// Initializes internal key parameters from the <see cref="Key" /> value.
		/// </summary>

		/// <inheritdoc />
		protected override byte[] PadBlock(ReadOnlySpan<byte> block, ulong messageLength) => throw new NotImplementedException();

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void ProcessBlock(ReadOnlySpan<byte> block)
		{
			// Copy input to 16-byte buffer and append a single '1' byte if block is short (RFC: padding = 1 byte then zeros)
			Span<byte> padded = stackalloc byte[BlockSize];
			block.CopyTo(padded);
			if (block.Length < BlockSize)
				padded[block.Length] = 1;

			// Load accumulator state
			ulong h0 = _acc[0], h1 = _acc[1], h2 = _acc[2], h3 = _acc[3], h4 = _acc[4];

			// Convert padded input block into 130-bit number split into 5 26-bit limbs (as per RFC)
			ulong t0 = BinaryPrimitives.ReadUInt64LittleEndian(padded);
			ulong t1 = BinaryPrimitives.ReadUInt64LittleEndian(padded.Slice(8));
			h0 += (uint)(t0 & 0x3ffffff);
			h1 += (uint)((t0 >> 26) & 0x3ffffff);
			h2 += (uint)(((t0 >> 52) | (t1 << 12)) & 0x3ffffff);
			h3 += (uint)((t1 >> 14) & 0x3ffffff);
			h4 += (uint)((t1 >> 40) & 0x3ffffff);

			// If full 16 bytes were present, set highest bit (equivalent to adding 2^128 per RFC)
			if (block.Length == BlockSize)
				h4 += (1 << 24);

			// Load r and perform 130-bit polynomial multiplication: accumulator * r
			ulong r0 = _r[0], r1 = _r[1], r2 = _r[2], r3 = _r[3], r4 = _r[4];

			// Compute limb products with optimized carry structure
			ulong t00 = h0 * r0 + h1 * _s[3] + h2 * _s[2] + h3 * _s[1] + h4 * _s[0];
			ulong t01 = h0 * r1 + h1 * r0 + h2 * _s[3] + h3 * _s[2] + h4 * _s[1];
			ulong t02 = h0 * r2 + h1 * r1 + h2 * r0 + h3 * _s[3] + h4 * _s[2];
			ulong t03 = h0 * r3 + h1 * r2 + h2 * r1 + h3 * r0 + h4 * _s[3];
			ulong t04 = h0 * r4 + h1 * r3 + h2 * r2 + h3 * r1 + h4 * r0;

			// Perform carry propagation and modular reduction mod 2^130 - 5
			t01 += t00 >> 26; h0 = (uint)(t00 & Mask26);
			t02 += t01 >> 26; h1 = (uint)(t01 & Mask26);
			t03 += t02 >> 26; h2 = (uint)(t02 & Mask26);
			t04 += t03 >> 26; h3 = (uint)(t03 & Mask26);
			ulong carry = t04 >> 26; h4 = (uint)(t04 & Mask26);

			// Fold final carry into h0 (modulo 2^130 - 5 reduction)
			h0 += (uint)(carry * 5);
			carry = h0 >> 26; h0 &= (uint)Mask26;
			h1 += (uint)carry;

			// Save accumulator state
			_acc[0] = (uint)h0; _acc[1] = (uint)h1; _acc[2] = (uint)h2; _acc[3] = (uint)h3; _acc[4] = (uint)h4;
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override byte[] ProcessFinalBlock()
		{
			// Final modular reduction: canonicalize accumulator to [0..2^130-5]
			uint h0 = _acc[0], h1 = _acc[1], h2 = _acc[2], h3 = _acc[3], h4 = _acc[4];

			// Propagate carries across limbs
			h1 += (h0 >> 26); h0 &= Mask26;
			h2 += (h1 >> 26); h1 &= Mask26;
			h3 += (h2 >> 26); h2 &= Mask26;
			h4 += (h3 >> 26); h3 &= Mask26;
			h0 += (h4 >> 26) * 5; h4 &= Mask26;
			h1 += (h0 >> 26); h0 &= Mask26;

			// Compute g = h + 5, then conditionally reduce modulo 2^130-5 if g >= 2^130
			var g0 = h0 + 5;
			var b = g0 >> 26; g0 &= Mask26;
			var g1 = h1 + b; b = g1 >> 26; g1 &= Mask26;
			var g2 = h2 + b; b = g2 >> 26; g2 &= Mask26;
			var g3 = h3 + b; b = g3 >> 26; g3 &= Mask26;
			var g4 = h4 + b - (1 << 26);

			// Mask to select either h or h - (2^130 - 5) based on overflow
			b = (g4 >> 31) - 1;
			h0 = (h0 & ~b) | (g0 & b);
			h1 = (h1 & ~b) | (g1 & b);
			h2 = (h2 & ~b) | (g2 & b);
			h3 = (h3 & ~b) | (g3 & b);
			h4 = (h4 & ~b) | (g4 & b);

			// Pack h into 128-bit output and add s (the final key part, RFC step 3)
			ulong f0 = ((h0) | (h1 << 26)) + _key[0];
			ulong f1 = (f0 >> 32) + ((h1 >> 6) | (h2 << 20)) + _key[1];
			ulong f2 = (f1 >> 32) + ((h2 >> 12) | (h3 << 14)) + _key[2];
			ulong f3 = (f2 >> 32) + ((h3 >> 18) | (h4 << 8)) + _key[3];

			Span<byte> tag = stackalloc byte[16];
			BinaryPrimitives.WriteUInt32LittleEndian(tag.Slice(0), (uint)f0);
			BinaryPrimitives.WriteUInt32LittleEndian(tag.Slice(4), (uint)f1);
			BinaryPrimitives.WriteUInt32LittleEndian(tag.Slice(8), (uint)f2);
			BinaryPrimitives.WriteUInt32LittleEndian(tag.Slice(12), (uint)f3);

			// Clear key immediately after use to prevent reuse
			CryptoUtilities.ClearAndNullify(ref KeyValue);

			return tag.ToArray();
		}

		/// <inheritdoc />
		protected override bool ShouldPadFinalBlock() => false;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InitializeKey()
		{
			ReadOnlySpan<byte> key = KeyValue;

			// Load and clamp the first 128 bits of the key as the polynomial 'r' key Clamp 'r' by setting/clearing specific bits to avoid
			// vulnerabilities as per RFC 8439, Section 2.5.1
			uint t0 = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(0));
			uint t1 = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(4));
			uint t2 = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(8));
			uint t3 = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(12));

			// Split 128-bit r into 5 x 26-bit limbs with clamping (see RFC for bitmask values)
			_r[0] = t0 & 0x03FFFFFFU;
			_r[1] = ((t0 >> 26) | (t1 << 6)) & 0x03FFFF03U;
			_r[2] = ((t1 >> 20) | (t2 << 12)) & 0x03FFC0FFU;
			_r[3] = ((t2 >> 14) | (t3 << 18)) & 0x03F03FFFU;
			_r[4] = (t3 >> 8) & 0x000FFFFFU;

			// Precompute 5*r[i] values to optimize carry-reduction step later
			_s[0] = _r[1] * 5;
			_s[1] = _r[2] * 5;
			_s[2] = _r[3] * 5;
			_s[3] = _r[4] * 5;

			// Load the second half of the key (s), which will be added during final tag computation
			_key[0] = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(16));
			_key[1] = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(20));
			_key[2] = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(24));
			_key[3] = BinaryPrimitives.ReadUInt32LittleEndian(key.Slice(28));
		}
	}
}