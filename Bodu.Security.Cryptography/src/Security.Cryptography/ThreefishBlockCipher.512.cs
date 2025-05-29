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
	/// Implements the <c>Threefish-512</c> block cipher algorithm, which is part of the Skein family of cryptographic functions. This
	/// cipher operates on 512-bit (64-byte) blocks and uses a 512-bit key with a 128-bit tweak for enhanced security and flexibility.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Threefish is a tweakable block cipher optimized for 64-bit platforms and forms the core primitive of the Skein hash function. The
	/// <c>Threefish-512</c> variant operates on four 64-bit words, using a mix of modular addition, bitwise rotation, and XOR operations.
	/// </para>
	/// <para>This implementation supports both encryption and decryption of fixed-size blocks.</para>
	/// </remarks>
	internal sealed class Threefish512Cipher
		: ThreefishBlockCipher
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Threefish512Cipher" /> class using the specified key and tweak.
		/// </summary>
		/// <param name="key">The 512-bit (64-byte) key used for encryption and decryption.</param>
		/// <param name="tweak">The 128-bit (16-byte) tweak value used to modify the block cipher behavior.</param>
		public Threefish512Cipher(ReadOnlySpan<byte> key, ReadOnlySpan<byte> tweak)
			: base(key, tweak) { }

		/// <inheritdoc />
		public override int BlockSize => 64;

		/// <inheritdoc />
		protected override int BlockWords => 8;

		/// <inheritdoc />
		protected override int[] RotationSchedule => new int[]
		{
			46, 36, 19, 37, 33, 27, 14, 42,
			17, 49, 36, 39, 44, 9, 54, 56,
			39, 30, 34, 24, 13, 50, 10, 17,
			25, 29, 39, 43, 8, 35, 56, 22
		};

		/// <inheritdoc />
		protected override int Rounds => 72;

		/// <summary>
		/// Decrypts a single 64-byte ciphertext block using the <c>Threefish-512</c> cipher and writes the result to the specified output buffer.
		/// </summary>
		/// <param name="input">The 64-byte ciphertext block to decrypt.</param>
		/// <param name="output">The 64-byte buffer to receive the decrypted plaintext block.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the cipher has been disposed.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="input" /> or <paramref name="output" /> is not 64 bytes.</exception>
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
			var rot = RotationSchedule;

			for (int d = (Rounds / 4) - 1; d >= 1; d -= 2)
			{
				int dm9 = d % 9, dm3 = d % 3;

				block[0] -= key[dm9 + 1];
				block[1] -= key[dm9 + 2];
				block[2] -= key[dm9 + 3];
				block[3] -= key[dm9 + 4];
				block[4] -= key[dm9 + 5];
				block[5] -= key[dm9 + 6] + tweak[dm3 + 1];
				block[6] -= key[dm9 + 7] + tweak[dm3 + 2];
				block[7] -= key[dm9 + 8] + (ulong)(d + 1);

				Unmix(ref block[4], ref block[3], rot[31]);
				Unmix(ref block[2], ref block[5], rot[30]);
				Unmix(ref block[0], ref block[7], rot[29]);
				Unmix(ref block[6], ref block[1], rot[28]);
				Unmix(ref block[2], ref block[7], rot[27]);
				Unmix(ref block[0], ref block[5], rot[26]);
				Unmix(ref block[6], ref block[3], rot[25]);
				Unmix(ref block[4], ref block[1], rot[24]);
				Unmix(ref block[0], ref block[3], rot[23]);
				Unmix(ref block[6], ref block[5], rot[22]);
				Unmix(ref block[4], ref block[7], rot[21]);
				Unmix(ref block[2], ref block[1], rot[20]);
				Unmix(ref block[6], ref block[7], rot[19]);
				Unmix(ref block[4], ref block[5], rot[18]);
				Unmix(ref block[2], ref block[3], rot[17]);
				Unmix(ref block[0], ref block[1], rot[16]);

				block[0] -= key[dm9];
				block[1] -= key[dm9 + 1];
				block[2] -= key[dm9 + 2];
				block[3] -= key[dm9 + 3];
				block[4] -= key[dm9 + 4];
				block[5] -= key[dm9 + 5] + tweak[dm3];
				block[6] -= key[dm9 + 6] + tweak[dm3 + 1];
				block[7] -= key[dm9 + 7] + (ulong)d;

				Unmix(ref block[4], ref block[3], rot[15]);
				Unmix(ref block[2], ref block[5], rot[14]);
				Unmix(ref block[0], ref block[7], rot[13]);
				Unmix(ref block[6], ref block[1], rot[12]);
				Unmix(ref block[2], ref block[7], rot[11]);
				Unmix(ref block[0], ref block[5], rot[10]);
				Unmix(ref block[6], ref block[3], rot[9]);
				Unmix(ref block[4], ref block[1], rot[8]);
				Unmix(ref block[0], ref block[3], rot[7]);
				Unmix(ref block[6], ref block[5], rot[6]);
				Unmix(ref block[4], ref block[7], rot[5]);
				Unmix(ref block[2], ref block[1], rot[4]);
				Unmix(ref block[6], ref block[7], rot[3]);
				Unmix(ref block[4], ref block[5], rot[2]);
				Unmix(ref block[2], ref block[3], rot[1]);
				Unmix(ref block[0], ref block[1], rot[0]);
			}

			block[0] -= key[0];
			block[1] -= key[1];
			block[2] -= key[2];
			block[3] -= key[3];
			block[4] -= key[4];
			block[5] -= key[5] + tweak[0];
			block[6] -= key[6] + tweak[1];
			block[7] -= key[7];

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}

		/// <summary>
		/// Encrypts a single 64-byte plaintext block using the <c>Threefish-512</c> cipher and writes the result to the specified output buffer.
		/// </summary>
		/// <param name="input">The 64-byte plaintext block to encrypt.</param>
		/// <param name="output">The 64-byte buffer to receive the encrypted ciphertext block.</param>
		/// <exception cref="ObjectDisposedException">Thrown if the cipher has been disposed.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="input" /> or <paramref name="output" /> is not 64 bytes.</exception>
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

			block[0] += key[0];
			block[1] += key[1];
			block[2] += key[2];
			block[3] += key[3];
			block[4] += key[4];
			block[5] += key[5] + tweak[0];
			block[6] += key[6] + tweak[1];
			block[7] += key[7];

			for (int d = 1; d < Rounds / 4; d += 2)
			{
				int dm9 = d % 9, dm3 = d % 3;

				Mix(ref block[0], ref block[1], rot[0]);
				Mix(ref block[2], ref block[3], rot[1]);
				Mix(ref block[4], ref block[5], rot[2]);
				Mix(ref block[6], ref block[7], rot[3]);
				Mix(ref block[2], ref block[1], rot[4]);
				Mix(ref block[4], ref block[7], rot[5]);
				Mix(ref block[6], ref block[5], rot[6]);
				Mix(ref block[0], ref block[3], rot[7]);
				Mix(ref block[4], ref block[1], rot[8]);
				Mix(ref block[6], ref block[3], rot[9]);
				Mix(ref block[0], ref block[5], rot[10]);
				Mix(ref block[2], ref block[7], rot[11]);
				Mix(ref block[6], ref block[1], rot[12]);
				Mix(ref block[0], ref block[7], rot[13]);
				Mix(ref block[2], ref block[5], rot[14]);
				Mix(ref block[4], ref block[3], rot[15]);

				block[0] += key[dm9];
				block[1] += key[dm9 + 1];
				block[2] += key[dm9 + 2];
				block[3] += key[dm9 + 3];
				block[4] += key[dm9 + 4];
				block[5] += key[dm9 + 5] + tweak[dm3];
				block[6] += key[dm9 + 6] + tweak[dm3 + 1];
				block[7] += key[dm9 + 7] + (ulong)d;

				Mix(ref block[0], ref block[1], rot[16]);
				Mix(ref block[2], ref block[3], rot[17]);
				Mix(ref block[4], ref block[5], rot[18]);
				Mix(ref block[6], ref block[7], rot[19]);
				Mix(ref block[2], ref block[1], rot[20]);
				Mix(ref block[4], ref block[7], rot[21]);
				Mix(ref block[6], ref block[5], rot[22]);
				Mix(ref block[0], ref block[3], rot[23]);
				Mix(ref block[4], ref block[1], rot[24]);
				Mix(ref block[6], ref block[3], rot[25]);
				Mix(ref block[0], ref block[5], rot[26]);
				Mix(ref block[2], ref block[7], rot[27]);
				Mix(ref block[6], ref block[1], rot[28]);
				Mix(ref block[0], ref block[7], rot[29]);
				Mix(ref block[2], ref block[5], rot[30]);
				Mix(ref block[4], ref block[3], rot[31]);

				block[0] += key[dm9 + 1];
				block[1] += key[dm9 + 2];
				block[2] += key[dm9 + 3];
				block[3] += key[dm9 + 4];
				block[4] += key[dm9 + 5];
				block[5] += key[dm9 + 6] + tweak[dm3 + 1];
				block[6] += key[dm9 + 7] + tweak[dm3 + 2];
				block[7] += key[dm9 + 8] + (ulong)(d + 1);
			}

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}
	}
}