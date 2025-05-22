// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher32.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Computes the hash for the input data using the <c>Fletcher-32</c> hash algorithm. This variant performs a non-cryptographic checksum
	/// calculation using two 16-bit accumulators to efficiently detect errors in byte sequences. This class cannot be inherited.
	/// </summary>
	/// <remarks>
	/// <para>
	/// <see cref="Fletcher32" /> is a non-cryptographic hash algorithm that computes a 32-bit checksum by iterating over the input data. It
	/// was invented by Brian Kernighan and Dennis Ritchie, and is typically used for error-checking in applications such as network protocols.
	/// </para>
	/// </remarks>
	public sealed class Fletcher32
		: Security.Cryptography.Fletcher<Fletcher32>
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