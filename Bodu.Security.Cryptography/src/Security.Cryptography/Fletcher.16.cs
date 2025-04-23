// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher16.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <see cref="Fletcher16" /> hash algorithm. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Fletcher16" /> is a non-cryptographic hash algorithm that computes a 16-bit checksum by iterating over the input data. It
	/// was invented by Brian Kernighan and Dennis Ritchie, and is typically used for error-checking in applications such as network protocols.
	/// </para>
	/// </remarks>
	public sealed class Fletcher16
		: Security.Cryptography.Fletcher
	{
		// Define a constant for the hash size
		private const int FletcherHashSize = 16;

		/// <summary>
		/// Initializes a new instance of the <see cref="Fletcher16" /> class with a 16-bit hash size.
		/// </summary>
		public Fletcher16()
			: base(FletcherHashSize)
		{ }
	}
}