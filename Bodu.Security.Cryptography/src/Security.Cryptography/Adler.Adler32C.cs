// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Adler32.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents the standard Adler-32 checksum algorithm using a modulus of 65521.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Adler-32 is a non-cryptographic checksum algorithm developed by Mark Adler for use in the zlib compression library. It produces a
	/// 32-bit checksum by maintaining two running sums (A and B), which are updated while reading input data and then combined into the
	/// final result as <c>(B &lt;&lt; 16) | A</c>. This implementation follows the original specification using a modulus of 65521 (the
	/// largest prime less than 2 <sup>16</sup>).
	/// </para>
	/// <para>
	/// This class is a concrete implementation of the abstract <see cref="Adler" /> base class (formerly <c>Adler32Base</c>), which
	/// provides the shared logic for Adler-style checksums. Use this class when a standard, zlib-compatible Adler-32 checksum is required.
	/// </para>
	/// <note type="important"> This algorithm is <b>not</b> cryptographically secure and should <b>not</b> be used for password hashing,
	/// digital signatures, or tamper-proof integrity checks in security-critical applications. </note>
	/// </remarks>
	public sealed class Adler32C
		: Adler
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="Adler32" /> class using the standard Adler-32 modulus (65521).
		/// </summary>
		public Adler32C()
			: base(Adler32C_Modulo)
		{
		}
	}
}