// ---------------------------------------------------------------------------------------------------------------
// <copyright file="CircularBuffer.ICollection.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System.Collections;

namespace Bodu.Collections.Generic
{
	public partial class CircularBuffer<T>
		: System.Collections.ICollection
	{
		[NonSerialized]
		private object? syncRoot;

		/// <inheritdoc />
		public int Count => this.count;

		/// <inheritdoc />
		bool ICollection.IsSynchronized => false;

		/// <inheritdoc />
		object ICollection.SyncRoot
		{
			get
			{
				// Thread-safe lazy initialization
				return syncRoot ?? Interlocked.CompareExchange(ref syncRoot, new object(), null) ?? syncRoot!;
			}
		}

		/// <inheritdoc />
		void ICollection.CopyTo(Array array, int index)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowHelper.ThrowIfArrayIsNotSingleDimension(array);
			ThrowHelper.ThrowIfArrayIsNotZeroBased(array);
			ThrowHelper.ThrowIfLessThan(index, 0);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, index, count);

			// No elements to copy, exit early
			if (this.count == 0)
				return;

			try
			{
				// Determine if the buffer wraps around the array end
				if (this.head < this.tail)
				{
					// Single contiguous block copy
					Array.Copy(this.array, this.head, array, index, this.count);
				}
				else
				{
					// Buffer wraps around: copy in two segments
					int firstSegmentLength = this.array.Length - this.head;

					Array.Copy(this.array, this.head, array, index, firstSegmentLength);
					Array.Copy(this.array, 0, array, index + firstSegmentLength, this.tail);
				}
			}
			catch (ArrayTypeMismatchException ex)
			{
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayType, nameof(array), ex);
			}
		}
	}
}