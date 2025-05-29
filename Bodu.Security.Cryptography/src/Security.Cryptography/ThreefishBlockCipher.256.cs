﻿using Bodu.Extensions;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Implements the <c>Threefish-256</c> block cipher algorithm, which is part of the Skein family of cryptographic functions. This
	/// cipher operates on 256-bit (32-byte) blocks and uses a 256-bit key with a 128-bit tweak for enhanced security and flexibility.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Threefish is a tweakable block cipher optimized for 64-bit platforms and forms the core primitive of the Skein hash function. The
	/// <c>Threefish-256</c> variant operates on four 64-bit words, using a mix of modular addition, bitwise rotation, and XOR operations.
	/// </para>
	/// <para>This implementation supports both encryption and decryption of fixed-size blocks.</para>
	/// </remarks>
	internal sealed class Threefish256Cipher
		: ThreefishBlockCipher
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Threefish256Cipher" /> class using the specified key and tweak.
		/// </summary>
		/// <param name="key">The 256-bit (32-byte) key used for encryption and decryption.</param>
		/// <param name="tweak">The 128-bit (16-byte) tweak value used to modify the block cipher behavior.</param>
		public Threefish256Cipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> tweak)
			: base(key, tweak) { }

		/// <inheritdoc />
		public override int BlockSize => 32;

		/// <inheritdoc />
		protected override int BlockWords => 4;

		/// <inheritdoc />
		protected override int[] RotationSchedule => new int[]
		{
			14, 16, 52, 57, 23, 40, 5, 37,
			25, 33, 46, 12, 58, 22, 32, 32
		};

		/// <inheritdoc />
		protected override int Rounds => 72;

		/// <summary>
		/// Decrypts a single 32-byte ciphertext block using the <c>Threefish-256</c> cipher and writes the result to the specified output buffer.
		/// </summary>
		/// <param name="input">The 32-byte ciphertext block to decrypt.</param>
		/// <param name="output">The 32-byte buffer to receive the decrypted plaintext block.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the cipher has been disposed.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="input" /> or <paramref name="output" /> is not 32 bytes.</exception>
		public override void Decrypt(ReadOnlySpan<byte> input, Span<byte> output)
		{
			ThrowIfDisposed();
			if (input.Length != BlockSize || output.Length != BlockSize)
				throw new ArgumentException(
					string.Format(ResourceStrings.CryptographicException_InvalidBlockLength, BlockSize));

			Span<ulong> block = stackalloc ulong[BlockWords];
			MemoryMarshal.Cast<byte, ulong>(input).CopyTo(block);

			var key = KeySchedule;
			var tweak = TweakSchedule;
			var rot = RotationSchedule; // Use indexed rotation constants

			for (int d = (Rounds / 4) - 1; d >= 1; d -= 2)
			{
				int dm5 = d % 5; int dm3 = d % 3;

				// Reverse post-round subkey injection
				block[0] -= key[dm5 + 1];
				block[1] -= key[dm5 + 2] + tweak[dm3 + 1];
				block[2] -= key[dm5 + 3] + tweak[dm3 + 2];
				block[3] -= key[dm5 + 4] + (uint)(d + 1);

				// Reverse second 4 rounds
				Unmix(ref block[2], ref block[1], rot[15]);
				Unmix(ref block[0], ref block[3], rot[14]);
				Unmix(ref block[2], ref block[3], rot[13]);
				Unmix(ref block[0], ref block[1], rot[12]);
				Unmix(ref block[2], ref block[1], rot[11]);
				Unmix(ref block[0], ref block[3], rot[10]);
				Unmix(ref block[2], ref block[3], rot[9]);
				Unmix(ref block[0], ref block[1], rot[8]);

				// Reverse mid-round subkey injection
				block[0] -= key[dm5];
				block[1] -= key[dm5 + 1] + tweak[dm3];
				block[2] -= key[dm5 + 2] + tweak[dm3 + 1];
				block[3] -= key[dm5 + 3] + (uint)d;

				// Reverse first 4 rounds
				Unmix(ref block[2], ref block[1], rot[7]);
				Unmix(ref block[0], ref block[3], rot[6]);
				Unmix(ref block[2], ref block[3], rot[5]);
				Unmix(ref block[0], ref block[1], rot[4]);
				Unmix(ref block[2], ref block[1], rot[3]);
				Unmix(ref block[0], ref block[3], rot[2]);
				Unmix(ref block[2], ref block[3], rot[1]);
				Unmix(ref block[0], ref block[1], rot[0]);
			}

			// Final subkey removal (round 0)
			block[0] -= key[0];
			block[1] -= key[1] + tweak[0];
			block[2] -= key[2] + tweak[1];
			block[3] -= key[3];

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}

		/// <summary>
		/// Encrypts a single 32-byte plaintext block using the <c>Threefish-256</c> cipher and writes the result to the specified output buffer.
		/// </summary>
		/// <param name="input">The 32-byte plaintext block to encrypt.</param>
		/// <param name="output">The 32-byte buffer to receive the encrypted ciphertext block.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the cipher has been disposed.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="input" /> or <paramref name="output" /> is not 32 bytes.</exception>
		public override void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
		{
			ThrowIfDisposed();
			if (input.Length != BlockSize || output.Length != BlockSize)
				throw new ArgumentException(
					string.Format(ResourceStrings.CryptographicException_InvalidBlockLength, BlockSize));

			Span<ulong> block = stackalloc ulong[BlockWords];
			MemoryMarshal.Cast<byte, ulong>(input).CopyTo(block);

			var key = KeySchedule;
			var tweak = TweakSchedule;
			var rot = RotationSchedule;

			// Initial key injection (round 0)
			block[0] += key[0x00];
			block[1] += key[0x01] + tweak[0x00];
			block[2] += key[0x02] + tweak[0x01];
			block[3] += key[0x03];

			for (int d = 1; d < Rounds / 4; d += 2)
			{
				int dm5 = d % 5; int dm3 = d % 3;

				// First 4 MIX rounds
				Mix(ref block[0], ref block[1], rot[0]);
				Mix(ref block[2], ref block[3], rot[1]);
				Mix(ref block[0], ref block[3], rot[2]);
				Mix(ref block[2], ref block[1], rot[3]);
				Mix(ref block[0], ref block[1], rot[4]);
				Mix(ref block[2], ref block[3], rot[5]);
				Mix(ref block[0], ref block[3], rot[6]);
				Mix(ref block[2], ref block[1], rot[7]);

				// Mid-round subkey injection
				block[0] += key[dm5];
				block[1] += key[dm5 + 1] + tweak[dm3];
				block[2] += key[dm5 + 2] + tweak[dm3 + 1];
				block[3] += key[dm5 + 3] + (ulong)d;

				// Second 4 MIX rounds
				Mix(ref block[0], ref block[1], rot[8]);
				Mix(ref block[2], ref block[3], rot[9]);
				Mix(ref block[0], ref block[3], rot[10]);
				Mix(ref block[2], ref block[1], rot[11]);
				Mix(ref block[0], ref block[1], rot[12]);
				Mix(ref block[2], ref block[3], rot[13]);
				Mix(ref block[0], ref block[3], rot[14]);
				Mix(ref block[2], ref block[1], rot[15]);

				// Post-round subkey injection
				block[0] += key[dm5 + 1];
				block[1] += key[dm5 + 2] + tweak[dm3 + 1];
				block[2] += key[dm5 + 3] + tweak[dm3 + 2];
				block[3] += key[dm5 + 4] + (ulong)d + 1;
			}

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}
	}
}