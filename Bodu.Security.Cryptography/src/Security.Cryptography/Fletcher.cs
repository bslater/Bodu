// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Bodu.Extensions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Base class for computing hash using the <c>Fletcher</c> hash algorithm family (Fletcher-16, Fletcher-32, Fletcher-64). This class
	/// cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// The Fletcher algorithm is used to generate non-cryptographic hash values for a given byte sequence. It operates by computing two
	/// components (partA and partB) over the input data and produces a hash based on these.
	/// </para>
	/// <para>
	/// This implementation handles Fletcher hash sizes of 16, 32, and 64 bits. Derived classes (see <see cref="Fletcher16" />,
	/// <see cref="Fletcher32" />, <see cref="Fletcher64" />) can implement specific hash sizes.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for digital signatures,
	/// password hashing, or integrity verification in security-sensitive contexts.</note>
	/// </remarks>
	public abstract class Fletcher<T>
		: BlockHashAlgorithm<T>
			where T : Fletcher<T>, new()
	{
		private static readonly int[] ValidHashSizes = { 16, 32, 64 };

		private readonly ulong modulus;
		private ulong partA;
		private ulong partB;
		private bool disposed = false;
#if !NET6_0_OR_GREATER

		// Required for .NET Standard 2.0 or older frameworks
		private bool finalized;
#endif

		/// <inheritdoc />
		public override int InputBlockSize => BlockSizeBytes;

		/// <inheritdoc />
		public override int OutputBlockSize => BlockSizeBytes;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fletcher" /> class with the specified hash size.
		/// </summary>
		/// <param name="hashSize">The hash size in bits. Valid values are 16, 32, or 64.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="hashSize" /> is not 16, 32, or 64.</exception>
		protected Fletcher(int hashSize)
			: base(ValidHashSizes.Contains(hashSize)
				 ? hashSize / 16
				 : throw new ArgumentException(
					string.Format(ResourceStrings.CryptographicException_InvalidHashSize, hashSize, string.Join(", ", ValidHashSizes)),
					nameof(hashSize)))
		{
			this.modulus = (1UL << (hashSize / 2)) - 1;
			HashSizeValue = hashSize;
			this.partA = this.partB = 0;
		}

		/// <summary>
		/// Gets the fully qualified algorithm name for this Fletcher variant, based on the configured hash output size.
		/// </summary>
		/// <value>
		/// A string in the form <c>Fletcher-N</c>, where <c>N</c> is the number of bits in the final hash output (e.g., <c>Fletcher-32</c>
		/// or <c>Fletcher-64</c>).
		/// </value>
		/// <remarks>
		/// The naming convention follows the established Fletcher standard, where the suffix indicates the bit-width of the resulting checksum:
		/// </remarks>
		public string AlgorithmName =>
			$"Fletcher-{HashSizeValue}";

		/// <inheritdoc />
		protected override void Dispose(bool disposing)
		{
			if (this.disposed) return;

			if (disposing)
			{
				CryptoUtilities.ClearAndNullify(ref HashValue);

				this.partA = this.partB = 0;
			}

			this.disposed = true;
			base.Dispose(disposing);
		}

		/// <inheritdoc />
		/// <summary>
		/// Gets a value indicating whether the current hash algorithm instance can be reused after the hash computation is finalized.
		/// </summary>
		/// <returns><see langword="true" /> if the current instance supports reuse via <see cref="Initialize" />; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// When this property returns <see langword="true" />, you may call <see cref="Initialize" /> after computing a hash to reset the
		/// internal state and perform a new hash computation without creating a new instance.
		/// </remarks>
		public override bool CanReuseTransform => true;

		/// <inheritdoc />
		/// <summary>
		/// Gets a value indicating whether multiple blocks can be transformed in a single <see cref="HashCore" /> call.
		/// </summary>
		/// <returns>
		/// <see langword="true" /> if the implementation supports processing multiple blocks in a single operation; otherwise, <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// Most hash algorithms support processing multiple input blocks in a single call to <see cref="HashAlgorithm.TransformBlock" /> or
		/// <see cref="HashAlgorithm.HashCore(byte[], int, int)" />, making this property typically return <see langword="true" />. Override
		/// this to return <see langword="false" /> for algorithms that require strict block-by-block input.
		/// </remarks>
		public override bool CanTransformMultipleBlocks => true;

		/// <summary>
		/// Initializes the internal state of the hash algorithm.
		/// </summary>
		public override void Initialize()
		{
			ThrowIfDisposed();
			base.Initialize();
#if !NET6_0_OR_GREATER
			this.State = 0;
			this.finalized = false;
#endif
			this.partA = this.partB = 0;
		}

		/// <inheritdoc />
		protected override byte[] PadBlock(ReadOnlySpan<byte> block, ulong messageLength)
		{
			Span<byte> buffer = stackalloc byte[BlockSizeBytes];
			block.CopyTo(buffer);
			return buffer.ToArray();
		}

		/// <inheritdoc />
		protected override byte[] ProcessFinalBlock()
		{
			// Combine partA and partB into the final hash value
			ulong finalHash = (this.partA << (this.HashSizeValue / 2)) | this.partB;

			// Convert to a byte array and take the size
			return finalHash.GetBytes().SliceInternal(0, HashSizeValue / 8);
		}

		/// <inheritdoc />
		protected override bool ShouldPadFinalBlock() => false;

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void ProcessBlock(ReadOnlySpan<byte> block)
		{
			ulong b = 0;
			for (int i = 0; i < block.Length && i < BlockSizeBytes; i++)
			{
				b |= ((ulong)block[i]) << ((BlockSizeBytes - (i + 1)) << 3);
			}

			// Update the internal state (partA and partB)
			this.partA = (this.partA + b) % this.modulus;
			this.partB = (this.partB + this.partA) % this.modulus;
		}
	}
}