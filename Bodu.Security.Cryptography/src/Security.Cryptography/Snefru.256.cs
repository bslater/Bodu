using System;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents the 256-bit variant of the <see cref="Snefru{T}" /> hash algorithm.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Snefru256" /> is a concrete implementation of the Snefru cryptographic hash function that produces a 256-bit (32-byte)
	/// output. It uses 8 words (32-bit unsigned integers) of internal state and processes input data in 64-byte blocks with double-sized padding.
	/// </para>
	/// <para>
	/// This variant applies 8 rounds of S-box substitutions and word rotations to transform each input block, then combines the result into
	/// the internal state using bitwise XOR. Upon finalization, the internal state is encoded in big-endian format to produce the final
	/// hash digest.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks.</note>
	/// </remarks>
	public sealed class Snefru256
		: Snefru<Snefru256>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Snefru256" /> class using a fixed 256-bit output size.
		/// </summary>
		public Snefru256()
			: base(256)
		{
		}
	}
}