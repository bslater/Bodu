using System;
using System.IO;
using System.Security.Cryptography;
using Bodu.Security.Cryptography.Extensions;
using Bodu.Extensions;

namespace Bodu.Security.Cryptography.Extensions
{
	public static partial class ICryptoTransformExtensions
	{
		/// <summary>
		/// Asynchronously applies a cryptographic transformation to data read from a source stream and writes the transformed output to a
		/// target stream.
		/// </summary>
		/// <param name="transform">The cryptographic transform to apply.</param>
		/// <param name="sourceStream">The stream to read untransformed data from.</param>
		/// <param name="targetStream">The stream to write transformed data to.</param>
		/// <param name="bufferSize">The buffer size (in bytes) used for streaming. Must be greater than zero.</param>
		/// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
		/// <returns>A task representing the asynchronous transformation operation.</returns>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="transform" />, <paramref name="sourceStream" />, or <paramref name="targetStream" /> is <c>null</c>.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="bufferSize" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// <para>
		/// This method uses a <see cref="CryptoStream" /> internally to write the transformed data to the target stream. It ensures
		/// <see cref="CryptoStream.FlushFinalBlockAsync" /> is called to finalize the cryptographic operation properly.
		/// </para>
		/// <para>
		/// Both source and target streams must support asynchronous operations. The method does not dispose the source or target stream.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>
		///using var aes = Aes.Create();
		///using var input = File.OpenRead("input.txt");
		///using var output = File.Create("encrypted.bin");
		///await aes.CreateEncryptor().TransformAsync(input, output, 4096);
		/// </code>
		/// </example>
		public static async Task TransformAsync(
			this ICryptoTransform transform,
			Stream sourceStream,
			Stream targetStream,
			int bufferSize,
			CancellationToken cancellationToken = default)
		{
			ThrowHelper.ThrowIfNull(transform);
			ThrowHelper.ThrowIfNull(sourceStream);
			ThrowHelper.ThrowIfNull(targetStream);
			ThrowHelper.ThrowIfLessThanOrEqual(bufferSize, 0);

			byte[] buffer = new byte[bufferSize];

			using CryptoStream cryptoStream = new CryptoStream(targetStream, transform, CryptoStreamMode.Write);
			int bytesRead;

			while ((bytesRead = await sourceStream.ReadAsync(buffer.AsMemory(0, bufferSize), cancellationToken).ConfigureAwait(false)) > 0)
			{
				await cryptoStream.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken).ConfigureAwait(false);
			}

			await cryptoStream.FlushFinalBlockAsync(cancellationToken).ConfigureAwait(false);
		}

		/// <summary>
		/// Asynchronously transforms the input memory region and writes the result into the destination memory region.
		/// </summary>
		/// <param name="transform">The cryptographic transform to apply.</param>
		/// <param name="input">The memory region containing the input data.</param>
		/// <param name="destination">The memory region to write the transformed output.</param>
		/// <param name="cancellationToken">Optional token for cancelling the operation.</param>
		/// <returns>The number of bytes written.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="transform" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">Thrown if <paramref name="destination" /> is too small.</exception>
		public static async Task<int> TransformAsync(
			this ICryptoTransform transform,
			ReadOnlyMemory<byte> input,
			Memory<byte> destination,
			CancellationToken cancellationToken = default)
		{
			ThrowHelper.ThrowIfNull(transform);
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(destination.Span, 0, input.Length + transform.OutputBlockSize);

			using var ms = new MemoryStream(destination.Length);
			using var cryptoStream = new CryptoStream(ms, transform, CryptoStreamMode.Write);

			await cryptoStream.WriteAsync(input, cancellationToken).ConfigureAwait(false);
			await cryptoStream.FlushFinalBlockAsync(cancellationToken).ConfigureAwait(false);

			if (!ms.TryGetBuffer(out ArraySegment<byte> segment))
				throw new InvalidOperationException("Failed to access transformed data.");

			segment.AsSpan().CopyTo(destination.Span);
			return segment.Count;
		}
	}
}