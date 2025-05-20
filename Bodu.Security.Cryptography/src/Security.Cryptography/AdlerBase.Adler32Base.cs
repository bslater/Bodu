using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides a reusable base class for Adler-32-style checksum algorithms using <see cref="uint" /> accumulators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class specializes the generic <see cref="AdlerBase{T}" /> base class for 32-bit checksums. It implements finalization logic
	/// specific to Adler-32 variants, combining the A and B accumulators into a 32-bit hash as: <c>(B &lt;&lt; 16) | A</c>.
	/// </para>
	/// <para>
	/// Derived classes such as <see cref="Adler32" /> and <see cref="Adler32C" /> supply different moduli (e.g., 65521 or 65536) depending
	/// on performance or compatibility needs.
	/// </para>
	/// </remarks>
	public abstract class Adler32Base
		: Bodu.Security.Cryptography.AdlerBase<uint>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Adler" /> class.
		/// </summary>
		protected Adler32Base(uint modulo)
			: base(modulo)
		{ }

		/// <inheritdoc />
		/// <summary>
		/// Finalizes the <see cref="Adler" /> checksum computation after all input data has been processed and returns the resulting 32-bit
		/// Adler hash value.
		/// </summary>
		/// <returns>
		/// A 4-byte array representing the computed Adler checksum, encoded in <b>big-endian</b> byte order. The result is formed by
		/// combining the internal accumulators: <c>(B &lt;&lt; 16) | A</c>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method reflects all input data previously supplied via <see cref="HashCore(byte[], int, int)" /> or
		/// <see cref="HashCore(ReadOnlySpan{byte})" />. It finalizes the current computation and returns a snapshot of the internal state
		/// as a 32-bit checksum.
		/// </para>
		/// <para>The result is encoded in big-endian format to ensure consistency across platforms regardless of architecture.</para>
		/// <para>
		/// Once finalized, the internal state is marked as complete. To reuse the same instance for a new hash operation, call
		/// <see cref="HashAlgorithm.Initialize" /> to reset the algorithm.
		/// </para>
		/// </remarks>
		protected override byte[] HashFinal()
		{
#if !NET6_0_OR_GREATER
			if (finalized)
				throw new CryptographicUnexpectedOperationException(ResourceStrings.CryptographicException_AlreadyFinalized);

			finalized = true;
			State = 2;
#endif
			ThrowIfDisposed();

			uint hash = (partB << 16) | partA;
			Span<byte> span = stackalloc byte[4];
			BinaryPrimitives.WriteUInt32BigEndian(span, hash); // Explicit big-endian output
			return span.ToArray();
		}
	}
}