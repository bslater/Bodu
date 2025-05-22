using System;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents the 128-bit variant of the <see cref="Snefru{T}" /> hash algorithm.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Snefru128" /> is a concrete implementation of the Snefru cryptographic hash function that produces a 128-bit (16-byte)
	/// output. It uses 4 words (32-bit unsigned integers) of internal state and processes input data in 64-byte blocks with double-sized padding.
	/// </para>
	/// <para>
	/// This variant applies 8 rounds of S-box substitutions and word rotations to transform each input block, then combines the result into
	/// the internal state using bitwise XOR. Upon finalization, the internal state is encoded in big-endian format to produce the final
	/// hash digest.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks.</note>
	/// </remarks>
	public sealed class Snefru128 : Snefru<Snefru128>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Snefru128" /> class using a fixed 128-bit output size.
		/// </summary>
		public Snefru128()
			: base(128)
		{
		}
	}
}