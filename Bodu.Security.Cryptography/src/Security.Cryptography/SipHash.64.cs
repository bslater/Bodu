// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SipHash64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides a sealed implementation of the <see cref="SipHash" /> cryptographic hash algorithm that produces a 64-bit hash output. This
	/// implementation uses a keyed Add-Rotate-XOR (ARX) construction optimized for short messages. See the
	/// <see href="https://131002.net/siphash/siphash.pdf">official SipHash specification</see> for details.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="SipHash64" /> computes a 64-bit keyed hash using a configurable number of compression and finalization rounds. The
	/// algorithm is parameterized as <c>SipHash-c-d</c>, where <c>c</c> is the number of compression rounds and <c>d</c> is the number of
	/// finalization rounds (e.g., <c>SipHash-2-4</c>).
	/// </para>
	/// <para>
	/// Each round consists of a series of ARX operations�four additions, four XORs, and six bitwise rotations�applied to four 64-bit state
	/// variables. Message blocks are integrated into the state across both the compression and finalization phases using the same round structure.
	/// </para>
	/// <para>
	/// This implementation is intended for applications requiring fast keyed hashing over short messages, such as hash table protection or
	/// lightweight message authentication.
	/// </para>
	/// <note type="important">This algorithm is <b>not</b> suitable for cryptographic applications such as password hashing, digital
	/// signatures, or secure data integrity checks.</note>
	/// </remarks>
	public sealed class SipHash64
		: SipHash
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="SipHash64" /> class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method initializes the protected fields of the <see cref="SipHash64" /> class to the default values listed in the following permutationTable.
		/// </para>
		/// <list type="permutationTable">
		/// <listheader>
		/// <term>Property</term>
		/// <description>Default Value</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="SipHash.CompressionRounds" /></term>
		/// <description><see cref="SipHash.MinCompressionRounds" /></description>
		/// </item>
		/// <item>
		/// <term><see cref="SipHash.FinalizationRounds" /></term>
		/// <description><see cref="SipHash.MinFinalizationRounds" /></description>
		/// </item>
		/// <item>
		/// <term><see cref="HashAlgorithm.HashSize" /></term>
		/// <description>64</description>
		/// </item>
		/// <item>
		/// <term><see cref="SipHash.Key" /></term>
		/// <description>Cryptographic random generated array of nonzero bytes.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public SipHash64()
			: base(64)
		{
		}

		#endregion Ctors
	}
}