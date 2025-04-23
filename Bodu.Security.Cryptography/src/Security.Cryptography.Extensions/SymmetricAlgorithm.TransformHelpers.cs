// ---------------------------------------------------------------------------------------------------------------
// <copyright file="TransformHelpers.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Security.Cryptography.Extensions
{
	using System;
	using System.Buffers;
	using System.IO;
	using System.Security.Cryptography;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Provides helper methods for performing cryptographic transforms using streams and byte arrays.
	/// </summary>
	internal static class TransformHelpers
	{
		/// <summary>
		/// Transforms a specified range of bytes in the input array using the given cryptographic transform.
		/// </summary>
		/// <param name="transform">The <see cref="ICryptoTransform" /> used to perform the transformation.</param>
		/// <param name="input">The byte array containing the data to transform.</param>
		/// <param name="offset">The zero-based byte offset in the input array at which to begin transformation.</param>
		/// <param name="count">The number of bytes to transform.</param>
		/// <returns>A new byte array containing the transformed result.</returns>
		/// <remarks>
		/// This method does not validate input parameters. The caller is responsible for ensuring that <paramref name="transform" />,
		/// <paramref name="input" />, and the <paramref name="offset" /> and <paramref name="count" /> values are valid.
		/// </remarks>
		internal static byte[] Transform(ICryptoTransform transform, byte[] input, int offset, int count) =>
			 transform.TransformFinalBlock(input, offset, count);

		/// <summary>
		/// Transforms the content of the input stream using the given transform and writes the result to the output stream.
		/// </summary>
		/// <param name="transform">The <see cref="ICryptoTransform" /> used for the transformation.</param>
		/// <param name="input">The input stream to read from.</param>
		/// <param name="output">The output stream to write to.</param>
		/// <param name="bufferSize">The size of the buffer used for reading and writing.</param>
		/// <returns>The total number of bytes read from the input stream.</returns>
		/// <remarks>
		/// This method does not validate input parameters. The caller must ensure that <paramref name="transform" />,
		/// <paramref name="input" />, <paramref name="output" />, and <paramref name="bufferSize" /> are valid.
		/// </remarks>
		internal static int Transform(ICryptoTransform transform, Stream input, Stream output, int bufferSize)
		{
			using var cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write);
			var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
			int totalRead = 0;
			int read;

			try
			{
				while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
				{
					cryptoStream.Write(buffer, 0, read);
					totalRead += read;
				}

				cryptoStream.FlushFinalBlock();
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
			}

			return totalRead;
		}

		/// <summary>
		/// Asynchronously transforms the content of the input stream using the given transform and writes the result to the output stream.
		/// </summary>
		/// <param name="transform">The <see cref="ICryptoTransform" /> used for the transformation.</param>
		/// <param name="input">The input stream to read from.</param>
		/// <param name="output">The output stream to write to.</param>
		/// <param name="bufferSize">The size of the buffer used for reading and writing.</param>
		/// <param name="cancellationToken">The cancellation token to monitor for cancellation requests.</param>
		/// <returns>The total number of bytes read from the input stream.</returns>
		/// <remarks>
		/// This method does not validate input parameters. The caller must ensure that <paramref name="transform" />,
		/// <paramref name="input" />, <paramref name="output" />, and <paramref name="bufferSize" /> are valid.
		/// </remarks>
		public static async Task<int> TransformAsync(this ICryptoTransform transform, Stream input, Stream output, int bufferSize, CancellationToken cancellationToken)
		{
			return await TransformAsyncInternal(transform, input, output, bufferSize, cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Internal implementation for asynchronously transforming a stream using the specified cryptographic transform.
		/// </summary>
		/// <param name="transform">The cryptographic transform to apply.</param>
		/// <param name="input">The input stream to read from.</param>
		/// <param name="output">The output stream to write to.</param>
		/// <param name="bufferSize">The buffer size in bytes.</param>
		/// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
		/// <returns>The total number of bytes read from the input stream.</returns>
		/// <remarks>
		/// This method does not validate input parameters. The caller must ensure that <paramref name="transform" />,
		/// <paramref name="input" />, <paramref name="output" />, and <paramref name="bufferSize" /> are valid.
		/// </remarks>
		private static async Task<int> TransformAsyncInternal(ICryptoTransform transform, Stream input, Stream output, int bufferSize, CancellationToken cancellationToken)
		{
			using var cryptoStream = new CryptoStream(output, transform, CryptoStreamMode.Write);
			var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
			int totalRead = 0;
			int read;

			try
			{
				while ((read = await input.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken).ConfigureAwait(false)) > 0)
				{
					await cryptoStream.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
					totalRead += read;
				}

				await cryptoStream.FlushFinalBlockAsync(cancellationToken).ConfigureAwait(false);
			}
			finally
			{
				ArrayPool<byte>.Shared.Return(buffer, clearArray: true);
			}

			return totalRead;
		}
	}
}