// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher32.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <see cref="Fletcher32" /> hash algorithm. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Fletcher32" /> is a non-cryptographic hash algorithm that computes a 32-bit checksum by iterating over the input data. It
	/// was invented by Brian Kernighan and Dennis Ritchie, and is typically used for error-checking in applications such as network protocols.
	/// </para>
	/// </remarks>
	public sealed class Fletcher32
		: Security.Cryptography.Fletcher
	{
		// Define a constant for the hash size
		private const int FletcherHashSize = 32;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fletcher32" /> class with a 32-bit hash size.
		/// </summary>
		public Fletcher32()
			: base(FletcherHashSize)
		{ }
	}
}