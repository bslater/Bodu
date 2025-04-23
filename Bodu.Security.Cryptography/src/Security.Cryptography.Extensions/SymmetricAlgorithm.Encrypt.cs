// ---------------------------------------------------------------------------------------------------------------
// <copyright file="SymmetricAlgorithm.Encrypt.cs" company="PlaceholderCompany">
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
		/// Defines the default buffer size, in bytes, to be used when reading from or writing to streams during encryption.
		/// </summary>
		public const int DefaultBufferSize = 81920;

		/// <summary>
		/// Encrypts a range of bytes from the specified byte array using the configured <see cref="SymmetricAlgorithm" />.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="data">The byte array containing the data to encrypt.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="data" /> at which to begin encryption.</param>
		/// <param name="count">The number of bytes to encrypt starting from <paramref name="offset" />.</param>
		/// <returns>A new byte array containing the encrypted data.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="offset" /> or <paramref name="count" /> is negative, or if the combination of
		/// <paramref name="offset" /> and <paramref name="count" /> exceeds the length of <paramref name="data" />.
		/// </exception>
		/// <remarks>
		/// This method uses <see cref="SymmetricAlgorithm.CreateEncryptor" /> to perform the encryption and returns the result as a new
		/// byte array.
		/// </remarks>
		public static byte[] Encrypt(this SymmetricAlgorithm algorithm, byte[] data, int offset, int count)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(data);
			ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(data, offset, count);

			using var transform = algorithm.CreateEncryptor();
			return TransformHelpers.Transform(transform, data, offset, count);
		}

		/// <summary>
		/// Encrypts the specified byte array using the configured <see cref="SymmetricAlgorithm" />.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="data">The byte array to encrypt.</param>
		/// <returns>A new byte array containing the encrypted data.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <remarks>This overload encrypts the entire byte array.</remarks>
		public static byte[] Encrypt(this SymmetricAlgorithm algorithm, byte[] data) =>
			algorithm.Encrypt(data, 0, data?.Length ?? 0);

		/// <summary>
		/// Encrypts the specified byte array using the configured <see cref="SymmetricAlgorithm" />, starting at the given offset.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="data">The byte array to encrypt.</param>
		/// <param name="offset">The zero-based byte offset in <paramref name="data" /> at which to begin encryption.</param>
		/// <returns>A new byte array containing the encrypted data from the specified offset to the end of the array.</returns>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="algorithm" /> or <paramref name="data" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="offset" /> is negative or exceeds the length of <paramref name="data" />.
		/// </exception>
		/// <remarks>This overload encrypts the bytes from the specified offset to the end of the array.</remarks>
		public static byte[] Encrypt(this SymmetricAlgorithm algorithm, byte[] data, int offset) =>
			algorithm.Encrypt(data, offset, data?.Length - offset ?? 0);

		/// <summary>
		/// Encrypts the data read from the <paramref name="source" /> stream and writes the encrypted bytes to the
		/// <paramref name="target" /> stream.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="source">The input stream to read the plaintext from.</param>
		/// <param name="target">The output stream to write the ciphertext to.</param>
		/// <param name="bufferSize">The size, in bytes, of the buffer used to read and write data. Must be greater than zero.</param>
		/// <returns>The total number of bytes read from the <paramref name="source" /> stream.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// This method uses a buffer of the specified size to read from the input stream and encrypts the data in blocks, writing the
		/// encrypted output to the target stream.
		/// </remarks>
		public static int Encrypt(this SymmetricAlgorithm algorithm, Stream source, Stream target, int bufferSize)
		{
			ThrowHelper.ThrowIfNull(algorithm);
			ThrowHelper.ThrowIfNull(source);
			ThrowHelper.ThrowIfNull(target);
			ThrowHelper.ThrowIfLessThanOrEqual(bufferSize, 0);

			using var transform = algorithm.CreateEncryptor();
			return TransformHelpers.Transform(transform, source, target, bufferSize);
		}

		/// <summary>
		/// Encrypts the data read from the <paramref name="source" /> stream using the default buffer size and writes the encrypted bytes
		/// to the <paramref name="target" /> stream.
		/// </summary>
		/// <param name="algorithm">The symmetric algorithm instance used to perform the encryption.</param>
		/// <param name="source">The input stream to read the plaintext from.</param>
		/// <param name="target">The output stream to write the ciphertext to.</param>
		/// <returns>The total number of bytes read from the <paramref name="source" /> stream.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="algorithm" />, <paramref name="source" />, or <paramref name="target" /> is <c>null</c>.
		/// </exception>
		/// <remarks>This method is a convenience overload that uses a buffer size of <see cref="DefaultBufferSize" />.</remarks>
		public static int Encrypt(this SymmetricAlgorithm algorithm, Stream source, Stream target) =>
			algorithm.Encrypt(source, target, SymmetricAlgorithmExtensions.DefaultBufferSize);
	}
}