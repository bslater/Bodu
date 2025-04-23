// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricAlgorithm.Decrypt.cs" company="PlaceholderCompany">
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
		/// Decrypts a range of bytes from the specified byte array using the configured <see cref="SymmetricAlgorithm" />.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="data">The byte array containing the data to decrypt.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="data" /> at which to begin decryption.</param>
		/// <param name="count">The number of bytes to decrypt starting from <paramref name="offset" />.</param>
		/// <returns>A new byte array containing the decrypted data.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="offset" /> or <paramref name="count" /> is negative, or if the combination of
		/// <paramref name="offset" /> and <paramref name="count" /> exceeds the length of <paramref name="data" />.
		/// </exception>
		/// <remarks>
		/// This method uses <see cref="SymmetricAlgorithm.CreateDecryptor" /> to perform the decryption and returns the result as a new
		/// byte array.
		/// </remarks>
		public static byte[] Decrypt(this SymmetricAlgorithm algorithm, byte[] data, int offset, int count)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(data);
			ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(data, offset, count);

			using var transform = algorithm.CreateDecryptor();
			return TransformHelpers.Transform(transform, data, offset, count);
		}

		/// <summary>
		/// Decrypts the specified byte array using the configured <see cref="SymmetricAlgorithm" />.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="data">The byte array to decrypt.</param>
		/// <returns>A new byte array containing the decrypted data.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <remarks>This overload decrypts the entire byte array.</remarks>
		public static byte[] Decrypt(this SymmetricAlgorithm algorithm, byte[] data) =>
			algorithm.Decrypt(data, 0, data?.Length ?? 0);

		/// <summary>
		/// Decrypts the specified byte array using the configured <see cref="SymmetricAlgorithm" />, starting at the given offset.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="data">The byte array to decrypt.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="data" /> at which to begin decryption.</param>
		/// <returns>A new byte array containing the decrypted data from the specified offset to the end of the array.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="offset" /> is negative or exceeds the length of <paramref name="data" />.
		/// </exception>
		/// <remarks>This overload decrypts the bytes from the specified offset to the end of the array.</remarks>
		public static byte[] Decrypt(this SymmetricAlgorithm algorithm, byte[] data, int offset) =>
			algorithm.Decrypt(data, offset, data?.Length - offset ?? 0);

		/// <summary>
		/// Decrypts the data read from the <paramref name="source" /> stream and writes the decrypted bytes to the
		/// <paramref name="target" /> stream.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="source">The input stream to read the ciphertext from.</param>
		/// <param name="target">The output stream to write the plaintext to.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer used to read and write data. Must be greater than zero.</param>
		/// <returns>The total number of bytes read from the <paramref name="source" /> stream.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// This method uses a buffer of the specified size to read from the input stream and decrypts the data in blocks, writing the
		/// plaintext output to the target stream.
		/// </remarks>
		public static int Decrypt(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(target);
			ThrowHelper.ThrowIfLessThanOrEqual(bufferSize, 0);

			using var transform = algorithm.CreateDecryptor();
			return TransformHelpers.Transform(transform, source, target, bufferSize);
		}

		/// <summary>
		/// Decrypts the data read from the <paramref name="source" /> stream using the default buffer size and writes the decrypted bytes
		/// to the <paramref name="target" /> stream.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the decryption.</param>
		/// <param name="source">The input stream to read the ciphertext from.</param>
		/// <param name="target">The output stream to write the plaintext to.</param>
		/// <returns>The total number of bytes read from the <paramref name="source" /> stream.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <remarks>This overload uses the default buffer size of <see cref="SymmetricAlgorithmExtensions.DefaultBufferSize" />.</remarks>
		public static int Decrypt(this SymmetricAlgorithm algorithm, Stream source, Stream target) =>
			algorithm.Decrypt(source, target, SymmetricAlgorithmExtensions.DefaultBufferSize);
	}
}