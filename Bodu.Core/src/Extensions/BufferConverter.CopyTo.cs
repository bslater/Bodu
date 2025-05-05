using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Bodu.Extensions
{
	public static partial class BufferConverter
	{
		/// <summary>
		/// Copies a specified number of elements of type <typeparamref name="T" /> from a byte array into a target array.
		/// </summary>
		/// <typeparam name="T">The unmanaged type to copy to.</typeparam>
		/// <param name="sourceArray">The source byte array.</param>
		/// <param name="sourceIndex">The starting index in the <paramref name="sourceArray" />.</param>
		/// <param name="targetArray">The target array to receive the elements.</param>
		/// <param name="targetIndex">The starting index in the <paramref name="targetArray" />.</param>
		/// <param name="count">The number of elements of type <typeparamref name="T" /> to copy.</param>
		/// <exception cref="ArgumentNullException"><paramref name="sourceArray" /> or <paramref name="targetArray" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="sourceIndex" />, <paramref name="targetIndex" />, or <paramref name="count" /> is out of range.
		/// </exception>
		/// <exception cref="ArgumentException">The specified range exceeds the bounds of the source or target arrays.</exception>
		/// <remarks>
		/// The method assumes that the byte array represents elements of type <typeparamref name="T" /> using platform-native endianness.
		/// </remarks>
		public static void CopyTo<T>(this byte[] sourceArray, int sourceIndex, T[] targetArray, int targetIndex, int count)
			where T : unmanaged
		{
			ThrowHelper.ThrowIfNull(sourceArray);
			ThrowHelper.ThrowIfNull(targetArray);

			ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(sourceArray, sourceIndex, count * Unsafe.SizeOf<T>());
			ThrowHelper.ThrowIfArrayOffsetOrCountInvalid(targetArray, targetIndex, count);

			ReadOnlySpan<byte> sourceSpan = sourceArray.AsSpan(sourceIndex, count * Unsafe.SizeOf<T>());
			Span<T> targetSpan = targetArray.AsSpan(targetIndex, count);

			MemoryMarshal.Cast<byte, T>(sourceSpan).CopyTo(targetSpan);
		}

		/// <summary>
		/// Copies a specified number of elements of type <typeparamref name="T" /> from a byte array into a target array, starting at index
		/// zero in the target array.
		/// </summary>
		/// <typeparam name="T">The unmanaged type to copy to.</typeparam>
		/// <param name="sourceArray">The source byte array.</param>
		/// <param name="sourceIndex">The starting index in the <paramref name="sourceArray" />.</param>
		/// <param name="targetArray">The target array to receive the elements.</param>
		/// <param name="count">The number of elements of type <typeparamref name="T" /> to copy.</param>
		public static void CopyTo<T>(this byte[] sourceArray, int sourceIndex, T[] targetArray, int count)
			where T : unmanaged
			=> CopyTo(sourceArray, sourceIndex, targetArray, 0, count);

		/// <summary>
		/// Copies a specified number of elements of type <typeparamref name="T" /> from a source span into a target span.
		/// </summary>
		/// <typeparam name="T">The unmanaged type to copy to.</typeparam>
		/// <param name="sourceSpan">The source span of bytes.</param>
		/// <param name="targetSpan">The target span of <typeparamref name="T" /> elements.</param>
		/// <param name="count">The number of elements to copy.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// The specified <paramref name="count" /> exceeds the available elements in the source or target span.
		/// </exception>
		public static void CopyTo<T>(this ReadOnlySpan<byte> sourceSpan, Span<T> targetSpan, int count)
			where T : unmanaged
		{
			// ThrowHelper will handle range and size checks
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(sourceSpan, 0, count * Unsafe.SizeOf<T>());
			ThrowHelper.ThrowIfSpanLengthIsInsufficient(targetSpan, 0, count);

			MemoryMarshal.Cast<byte, T>(sourceSpan.Slice(0, count * Unsafe.SizeOf<T>())).CopyTo(targetSpan.Slice(0, count));
		}

		/// <summary>
		/// Copies a specified number of elements of type <typeparamref name="T" /> from a memory region of bytes into a memory region of <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">The unmanaged type to copy to.</typeparam>
		/// <param name="sourceMemory">The source memory of bytes.</param>
		/// <param name="targetMemory">The target memory of <typeparamref name="T" /> elements.</param>
		/// <param name="count">The number of elements to copy.</param>
		public static void CopyTo<T>(this Memory<byte> sourceMemory, Memory<T> targetMemory, int count)
			where T : unmanaged
			=> CopyTo(sourceMemory.Span, targetMemory.Span, count);

		/// <summary>
		/// Copies a single value of type <typeparamref name="T" /> into a byte array at the specified index.
		/// </summary>
		/// <typeparam name="T">The unmanaged type to copy.</typeparam>
		/// <param name="value">The value to copy.</param>
		/// <param name="targetArray">The byte array to receive the value.</param>
		/// <param name="index">The starting index in the <paramref name="targetArray" />.</param>
		/// <exception cref="ArgumentNullException"><paramref name="targetArray" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// <paramref name="index" /> is negative or greater than the available space in <paramref name="targetArray" />.
		/// </exception>
		public static void CopyTo<T>(this T value, byte[] targetArray, int index)
			where T : unmanaged
		{
			ThrowHelper.ThrowIfNull(targetArray);

			ThrowHelper.ThrowIfArrayLengthIsInsufficient(targetArray, index, Unsafe.SizeOf<T>());

			Span<byte> targetSpan = targetArray.AsSpan(index, Unsafe.SizeOf<T>());
#if NET8_0_OR_GREATER
			MemoryMarshal.Write(targetSpan, in value); // Preferred in .NET 8+
#else
			MemoryMarshal.Write(targetSpan, ref value); // Required in .NET 6/7
#endif
		}
	}
}