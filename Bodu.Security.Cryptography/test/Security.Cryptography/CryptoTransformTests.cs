// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CryptoTransformTests.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Base class for testing types implementing <see cref="ICryptoTransform" />.
	/// </summary>
	/// <typeparam name="TAlgorithm">The crypto transform type under test.</typeparam>
	public abstract partial class CryptoTransformTests<TAlgorithm>
		where TAlgorithm : System.Security.Cryptography.ICryptoTransform
	{
		/// <summary>
		/// Creates a new instance of the crypto transform under test.
		/// </summary>
		protected abstract TAlgorithm CreateAlgorithm();

		/// <summary>
		/// Returns the expected hash result when hashing an empty byte array using the default variant.
		/// </summary>
		/// <exception cref="KeyNotFoundException">Thrown if the expected hash for the empty input is not defined for the default variant.</exception>
		protected abstract byte[] ExpectedEmptyInputHash { get; }
	}
}