using Bodu.Extensions;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Implements the <c>Threefish-256</c> block cipher algorithm, part of the Skein family. This cipher operates on 256-bit (32-byte)
	/// blocks and uses a 256-bit key with a 128-bit tweak.
	/// </summary>
	public sealed class Threefish256Engine : IBlockCipher
	{
		private const int BlockSizeBytes = BlockWords * sizeof(ulong);
		private const int BlockWords = 4;
		private const ulong KeyParityValue = 0x1BD11BDAA9FC1A22;

		// Mix rotation constants (Skein 1.3 spec)
		private const int R00 = 0x0E, R01 = 0x10;

		private const int R10 = 0x34, R11 = 0x39;
		private const int R20 = 0x17, R21 = 0x28;
		private const int R30 = 0x05, R31 = 0x25;
		private const int R40 = 0x19, R41 = 0x21;
		private const int R50 = 0x2E, R51 = 0x0C;
		private const int R60 = 0x3A, R61 = 0x16;
		private const int R70 = 0x20, R71 = 0x20;

		private const int ThreefishRounds = 72;

		private static readonly int[] MOD17 = new int[ThreefishRounds];
		private static readonly int[] MOD3 = new int[ThreefishRounds];
		private static readonly int[] MOD5 = new int[ThreefishRounds];
		private static readonly int[] MOD9 = new int[ThreefishRounds];

		// KeySchedule: [K0, K1, K2, K3, K4=parity, K0, K1, K2, K3]
		private readonly ulong[] _keySchedule;

		// TweakSchedule: [T0, T1, T2=T0^T1, T0, T1]
		private readonly ulong[] _tweakSchedule;

		private bool _disposed;

		static Threefish256Engine()
		{
			for (int i = 0; i < ThreefishRounds; i++)
			{
				MOD3[i] = i % 3;
				MOD5[i] = i % 5;
				MOD9[i] = i % 9;
				MOD17[i] = i % 17;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Threefish256Engine" /> class using the specified key and tweak.
		/// </summary>
		/// <param name="key">A 32-byte (256-bit) encryption key.</param>
		/// <param name="tweak">A 16-byte (128-bit) tweak value.</param>
		/// <exception cref="ArgumentException">Thrown if the key or tweak length is invalid.</exception>
		public Threefish256Engine(ReadOnlySpan<byte> key, ReadOnlySpan<byte> tweak)
		{
			ThrowHelper.ThrowIfArrayLengthInvalid(key, 32);
			ThrowHelper.ThrowIfArrayLengthInvalid(tweak, 16);

			// Key schedule initialization: 4 words + parity + duplicated key
			_keySchedule = new ulong[9];
			MemoryMarshal.Cast<byte, ulong>(key).CopyTo(_keySchedule);

			ulong parity = KeyParityValue
				^ _keySchedule[0]
				^ _keySchedule[1]
				^ _keySchedule[2]
				^ _keySchedule[3];

			_keySchedule[4] = parity;
			_keySchedule[5] = _keySchedule[0];
			_keySchedule[6] = _keySchedule[1];
			_keySchedule[7] = _keySchedule[2];
			_keySchedule[8] = _keySchedule[3];

			// Tweak schedule initialization: T0, T1, T2 = T0^T1, then duplicate T0/T1
			_tweakSchedule = new ulong[5];
			MemoryMarshal.Cast<byte, ulong>(tweak).CopyTo(_tweakSchedule);
			_tweakSchedule[2] = _tweakSchedule[0] ^ _tweakSchedule[1];
			_tweakSchedule[3] = _tweakSchedule[0];
			_tweakSchedule[4] = _tweakSchedule[1];
		}

		/// <summary>
		/// Gets the fixed block size of the cipher in bytes.
		/// </summary>
		public int BlockSize => BlockSizeBytes;

		/// <summary>
		/// Decrypts a 32-byte ciphertext block and writes the plaintext to the specified output span.
		/// </summary>
		/// <param name="input">The 32-byte ciphertext block.</param>
		/// <param name="output">The 32-byte output block to write the decrypted result.</param>
		public void Decrypt(ReadOnlySpan<byte> input, Span<byte> output)
		{
			ValidateBlockSize(input);
			ValidateBlockSize(output);

			Span<ulong> block = stackalloc ulong[BlockWords];
			block[0] = BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(0));
			block[1] = BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(8));
			block[2] = BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(16));
			block[3] = BinaryPrimitives.ReadUInt64LittleEndian(input.Slice(24));

			var key = _keySchedule;
			var tweak = _tweakSchedule;

			for (int d = (ThreefishRounds / 4) - 1; d >= 1; d -= 2)
			{
				int dm5 = MOD5[d], dm3 = MOD3[d];

				// Reverse subkey injection
				block[0] -= key[dm5 + 1];
				block[1] -= key[dm5 + 2] + tweak[dm3 + 1];
				block[2] -= key[dm5 + 3] + tweak[dm3 + 2];
				block[3] -= key[dm5 + 4] + (uint)d + 1;

				// Reverse 4 mix rounds
				Unmix(ref block[0], ref block[3], R70);
				Unmix(ref block[2], ref block[1], R71);
				Unmix(ref block[0], ref block[1], R60);
				Unmix(ref block[2], ref block[3], R61);
				Unmix(ref block[0], ref block[3], R50);
				Unmix(ref block[2], ref block[1], R51);
				Unmix(ref block[0], ref block[1], R40);
				Unmix(ref block[2], ref block[3], R41);

				// Reverse subkey injection
				block[0] -= key[dm5];
				block[1] -= key[dm5 + 1] + tweak[dm3];
				block[2] -= key[dm5 + 2] + tweak[dm3 + 1];
				block[3] -= key[dm5 + 3] + (uint)d;

				// Reverse 4 mix rounds
				Unmix(ref block[0], ref block[3], R30);
				Unmix(ref block[2], ref block[1], R31);
				Unmix(ref block[0], ref block[1], R20);
				Unmix(ref block[2], ref block[3], R21);
				Unmix(ref block[0], ref block[3], R10);
				Unmix(ref block[2], ref block[1], R11);
				Unmix(ref block[0], ref block[1], R00);
				Unmix(ref block[2], ref block[3], R01);
			}

			block[0] -= key[0];
			block[1] -= key[1] + tweak[0];
			block[2] -= key[2] + tweak[1];
			block[3] -= key[3];

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}

		/// <summary>
		/// Releases all resources used by the cipher, including key material and tweak schedule.
		/// </summary>
		public void Dispose()
		{
			if (_disposed) return;
			CryptoUtilities.Clear(MemoryMarshal.AsBytes(_keySchedule.AsSpan()));
			CryptoUtilities.Clear(MemoryMarshal.AsBytes(_tweakSchedule.AsSpan()));
			_disposed = true;
		}

		/// <summary>
		/// Encrypts a 32-byte block of input data and writes the ciphertext to the specified output span.
		/// </summary>
		/// <param name="input">The 32-byte input block.</param>
		/// <param name="output">The 32-byte output block to write the encrypted result.</param>
		public void Encrypt(ReadOnlySpan<byte> input, Span<byte> output)
		{
			ValidateBlockSize(input);
			ValidateBlockSize(output);

			Span<ulong> block = stackalloc ulong[BlockWords];
			MemoryMarshal.Cast<byte, ulong>(input).CopyTo(block);

			var key = _keySchedule;
			var tweak = _tweakSchedule;

			block[0] += key[0];
			block[1] += key[1] + tweak[0];
			block[2] += key[2] + tweak[1];
			block[3] += key[3];

			for (int d = 1; d < (ThreefishRounds / 4); d += 2)
			{
				int dm5 = MOD5[d], dm3 = MOD3[d];

				// First 4 rounds
				Mix(ref block[0], ref block[1], R00);
				Mix(ref block[2], ref block[3], R01);
				Mix(ref block[0], ref block[3], R10);
				Mix(ref block[2], ref block[1], R11);
				Mix(ref block[0], ref block[1], R20);
				Mix(ref block[2], ref block[3], R21);
				Mix(ref block[0], ref block[3], R30);
				Mix(ref block[2], ref block[1], R31);

				// Mid-round subkey injection
				block[0] += key[dm5];
				block[1] += key[dm5 + 1] + tweak[dm3];
				block[2] += key[dm5 + 2] + tweak[dm3 + 1];
				block[3] += key[dm5 + 3] + (uint)d;

				// Second 4 rounds
				Mix(ref block[0], ref block[1], R40);
				Mix(ref block[2], ref block[3], R41);
				Mix(ref block[0], ref block[3], R50);
				Mix(ref block[2], ref block[1], R51);
				Mix(ref block[0], ref block[1], R60);
				Mix(ref block[2], ref block[3], R61);
				Mix(ref block[0], ref block[3], R70);
				Mix(ref block[2], ref block[1], R71);

				// Post-round subkey injection
				block[0] += key[dm5 + 1];
				block[1] += key[dm5 + 2] + tweak[dm3 + 1];
				block[2] += key[dm5 + 3] + tweak[dm3 + 2];
				block[3] += key[dm5 + 4] + (uint)d + 1;
			}

			MemoryMarshal.Cast<ulong, byte>(block).CopyTo(output);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Mix(ref ulong a, ref ulong b, int rotation)
		{
			a += b;
			b = BitOperations.RotateLeft(b, rotation) ^ a;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Unmix(ref ulong a, ref ulong b, int rotation)
		{
			b ^= a;
			b = BitOperations.RotateRight(b, rotation);
			a -= b;
		}

		[Conditional("DEBUG")]
		private static void ValidateBlockSize(ReadOnlySpan<byte> span) =>
			Debug.Assert(span.Length == BlockSizeBytes);
	}
}