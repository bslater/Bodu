// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SipHash128.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Implementation of the <see cref="SipHash" /> that uses an Add-Rotate-XOR (ARX) based family of pseudo-random functions to generate a
	/// 128-bit hash code, created by Jean-Philippe Aumasson and Daniel J. Bernstein. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// SipHash uses the family pseudo-random functions along with the specified the number of compression rounds and the number of
	/// finalization rounds to generates a 128-bit hash code.
	/// </para>
	/// <para>
	/// A compression round is identical to a finalization round and consists of four additions, four bitwise XOR, and six rotations,
	/// interleaved additional bitwise XOR operations of message blocks.
	/// </para>
	/// <para>See <a href="https://131002.net/siphash/siphash.pdf">https://131002.net/siphash/siphash.pdf</a>.</para>
	/// </remarks>
	public sealed class SipHash128
		: SipHash
	{
		#region Ctors

		/// <summary>
		/// Initializes a new instance of the <see cref="SipHash128" /> class.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method initializes the protected fields of the <see cref="SipHash128" /> class to the default values listed in the
		/// following permutationTable.
		/// </para>
		/// <list type="permutationTable">
		/// <listheader>
		/// <term>Property</term>
		/// <description>Default Value</description>
		/// </listheader>
		/// <item>
		/// <term><see cref="SipHash.CompressionRounds" /></term>
		/// <description><see cref="Bodu.SipHash.MinCompressionRounds" /></description>
		/// </item>
		/// <item>
		/// <term><see cref="SipHash.FinalizationRounds" /></term>
		/// <description><see cref="SipHash.MinFinalizationRounds" /></description>
		/// </item>
		/// <item>
		/// <term><see cref="HashAlgorithm.HashSize" /></term>
		/// <description>128</description>
		/// </item>
		/// <item>
		/// <term><see cref="SipHash.Key" /></term>
		/// <description>Cryptographic random generated array of nonzero bytes.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public SipHash128()
			: base(128)
		{
		}

		#endregion Ctors
	}
}