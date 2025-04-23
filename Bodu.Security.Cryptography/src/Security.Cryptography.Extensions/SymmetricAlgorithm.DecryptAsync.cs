// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricAlgorithm.DecryptAsync.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography.Extensions
{
	using System;
	using System.IO;
	using System.Security.Cryptography;
	using System.Threading;
	using System.Threading.Tasks;

	public static partial class SymmetricAlgorithmExtensions
	{
		/// <summary>
		/// Asynchronously decrypts the data read from the <paramref name="source" /> stream and writes the decrypted bytes to the
		/// <paramref name="target" /> stream using the specified buffer size.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="source">The input stream to read the ciphertext from.</param>
		/// <param name="target">The output stream to write the plaintext to.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer used to read and write data. Must be greater than zero.</param>
		/// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains the total number of bytes read from the
		/// <paramref name="source" /> stream.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// This method performs non-blocking decryption of data read from the input stream. The output is written to the target stream in
		/// blocks, using the specified buffer size. The operation can be cancelled using the provided <paramref name="cancellationToken" />.
		/// </remarks>
		public static async Task<int> DecryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize, CancellationToken cancellationToken)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(target);
			ThrowHelper.ThrowIfLessThanOrEqual(bufferSize, 0);

			using var transform = algorithm.CreateDecryptor();
			return await TransformHelpers.TransformAsync(transform, source, target, bufferSize, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously decrypts the data read from the <paramref name="source" /> stream and writes the decrypted bytes to the
		/// <paramref name="target" /> stream using the specified buffer size.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="source">The input stream to read the ciphertext from.</param>
		/// <param name="target">The output stream to write the plaintext to.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer used to read and write data. Must be greater than zero.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains the total number of bytes read from the
		/// <paramref name="source" /> stream.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>This overload uses <see cref="CancellationToken.None" /> to disable cancellation.</remarks>
		public static Task<int> DecryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize) =>
			algorithm.DecryptAsync(source, target, bufferSize, CancellationToken.None);

		/// <summary>
		/// Asynchronously decrypts the data read from the <paramref name="source" /> stream using the default buffer size and writes the
		/// decrypted bytes to the <paramref name="target" /> stream.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="source">The input stream to read the ciphertext from.</param>
		/// <param name="target">The output stream to write the plaintext to.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains the total number of bytes read from the
		/// <paramref name="source" /> stream.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <remarks>This overload uses a default buffer size of <c>81920</c> and disables cancellation by using <see cref="CancellationToken.None" />.</remarks>
		public static Task<int> DecryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target) =>
			algorithm.DecryptAsync(source, target, 81920, CancellationToken.None);
	}
}