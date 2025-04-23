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
	/// <typeparam name="T">The crypto transform type under test.</typeparam>
	public abstract partial class CryptoTransformTests<T>
		where T : System.Security.Cryptography.ICryptoTransform
	{
		/// <summary>
		/// Creates a new instance of the crypto transform under test.
		/// </summary>
		protected abstract T CreateAlgorithm();
	}
}