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
	/// Provides a reusable base class for Adler-64-style checksum algorithms using <see cref="ulong" /> accumulators.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class specializes the generic <see cref="AdlerBase{T}" /> base class for 64-bit checksums. It implements finalization logic
	/// specific to Adler-64 variants, combining the A and B accumulators into a 64-bit hash as: <c>(B &lt;&lt; 32) | A</c>.
	/// </para>
	/// <para>
	/// Derived classes such as <see cref="Adler64" /> supply the modulus 4294967291 (2^32 − 5) to support wide-range checksums with reduced
	/// wraparound risk compared to Adler-32.
	/// </para>
	/// </remarks>
	public abstract class Adler64Base
		: Bodu.Security.Cryptography.AdlerBase<ulong>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Adler" /> class.
		/// </summary>
		protected Adler64Base(ulong modulo)
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

			ulong hash = (partB << 32) | partA;
			Span<byte> span = stackalloc byte[8];
			BinaryPrimitives.WriteUInt64BigEndian(span, hash); // Explicit big-endian output
			return span.ToArray();
		}
	}
}