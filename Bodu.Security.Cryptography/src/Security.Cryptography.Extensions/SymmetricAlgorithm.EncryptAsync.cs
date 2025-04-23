// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricAlgorithm.EncryptAsync.cs" company="PlaceholderCompany">
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
		/// Asynchronously encrypts the data read from the <paramref name="source" /> stream and writes the encrypted bytes to the
		/// <paramref name="target" /> stream using the specified buffer size.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="source">The input stream to read the plaintext from.</param>
		/// <param name="target">The output stream to write the ciphertext to.</param>
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
		/// This method encrypts data in blocks, reading from the input stream and writing encrypted data to the output stream
		/// asynchronously. The operation can be cancelled by passing a <see cref="CancellationToken" />.
		/// </remarks>
		public static async Task<int> EncryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize, CancellationToken cancellationToken)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(target);
			ThrowHelper.ThrowIfLessThanOrEqual(bufferSize, 0);

			using var transform = algorithm.CreateEncryptor();
			return await TransformHelpers.TransformAsync(transform, source, target, bufferSize, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously encrypts the data read from the <paramref name="source" /> stream and writes the encrypted bytes to the
		/// <paramref name="target" /> stream using the specified buffer size.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="source">The input stream to read the plaintext from.</param>
		/// <param name="target">The output stream to write the ciphertext to.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer used to read and write data. Must be greater than zero.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains the total number of bytes read from the
		/// <paramref name="source" /> stream.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>This overload uses <see cref="CancellationToken.None" /> for the operation.</remarks>
		public static Task<int> EncryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize) =>
			algorithm.EncryptAsync(source, target, bufferSize, CancellationToken.None);

		/// <summary>
		/// Asynchronously encrypts the data read from the <paramref name="source" /> stream and writes the encrypted bytes to the
		/// <paramref name="target" /> stream using the default buffer size.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="source">The input stream to read the plaintext from.</param>
		/// <param name="target">The output stream to write the ciphertext to.</param>
		/// <returns>
		/// A task that represents the asynchronous operation. The task result contains the total number of bytes read from the
		/// <paramref name="source" /> stream.
		/// </returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <remarks>
		/// This overload uses the default buffer size of <see cref="SymmetricAlgorithmExtensions.DefaultBufferSize" /> and
		/// <see cref="CancellationToken.None" /> for the operation.
		/// </remarks>
		public static Task<int> EncryptAsync(this SymmetricAlgorithm algorithm, Stream source, Stream target) =>
			algorithm.EncryptAsync(source, target, SymmetricAlgorithmExtensions.DefaultBufferSize, CancellationToken.None);
	}
}