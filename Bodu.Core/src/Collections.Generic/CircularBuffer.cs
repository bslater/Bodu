// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="CircularBuffer.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Diagnostics;

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Represents a first-in, first-out collection (FirstInFirstOut) of objects using a fixed buffer and automatic overwrite support.
	/// </summary>
	/// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
	/// <remarks>
	/// <para>
	/// This class implements a generic a circular buffer of a fixed capacity. Objects stored in a <see cref="CircularBuffer{T}" /> are
	/// inserted at one end and removed from the other. Use <see cref="CircularBuffer{T}" /> if you need to limited the number of elements
	/// in the collection and to access the information from oldest to newest in the collection. Use
	/// <see cref="System.Collections.Generic.Queue{T}" /> if you do not need to restrict the number of elements in the collection.
	/// </para>
	/// <para>Three main operations can be performed on a <see cref="CircularBuffer{T}" /> and its elements:</para>
	/// <list type="bullet">
	/// <item>
	/// <description><see cref="CircularBuffer{T}.Enqueue(T)" /> adds an element to the end of the <see cref="CircularBuffer{T}" />.</description>
	/// </item>
	/// <item>
	/// <description><see cref="CircularBuffer{T}.Dequeue" /> removes the oldest element from the start of the <see cref="CircularBuffer{T}" />.</description>
	/// </item>
	/// <item>
	/// <description>
	/// <see cref="CircularBuffer{T}.Peek" /> returns the oldest element that is at the start of the <see cref="CircularBuffer{T}" /> but
	/// does not remove it from the <see cref="CircularBuffer{T}" />.
	/// </description>
	/// </item>
	/// </list>
	/// <para>
	/// The <see cref="CircularBuffer{T}.Capacity" /> of a <see cref="CircularBuffer{T}" /> is the maximum number of elements the
	/// <see cref="CircularBuffer{T}" /> can hold. As elements are added to a full <see cref="CircularBuffer{T}" /> the oldest elements are
	/// overwritten by default. Setting the <see cref="CircularBuffer{T}.AllowOverwrite" /> to <see langword="false" /> will throw an
	/// <see cref="System.InvalidOperationException" /> if an attempt is made to add an element to a <see cref="CircularBuffer{T}" /> where
	/// the <see cref="CircularBuffer{T}.Count" /> equals <see cref="CircularBuffer{T}.Capacity" />.
	/// </para>
	/// <para>
	/// <see cref="CircularBuffer{T}" /> accepts <see langword="null" /> (Nothing in Visual Basic) as a valid value for reference types and
	/// allows duplicate elements.
	/// </para>
	/// </remarks>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CircularBufferDebugView<>))]
	[Serializable]
	public partial class CircularBuffer<T>
	{
		private const int DefaultCapacity = 16;

		private T[] array;
		private int count;
		private int head;
		private int tail;
		private int version;

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class with the default capacity and allows overwriting by default.
		/// </summary>
		/// <remarks>
		/// The default capacity is defined by the <c>DefaultCapacity</c> constant. When the buffer is full, adding a new item will
		/// overwrite the oldest element.
		/// </remarks>
		public CircularBuffer()
			: this(DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes an empty <see cref="CircularBuffer{T}" /> with the specified capacity. Oldest elements are automatically overwritten
		/// when capacity is exceeded.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be non-negative.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than zero.</exception>
		public CircularBuffer(int capacity)
			: this(capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes an empty <see cref="CircularBuffer{T}" /> with the specified capacity and overwrite behavior.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than 0.</param>
		/// <param name="allowOverwrite">
		/// If <see langword="true" />, oldest elements are overwritten when capacity is reached; otherwise, adding beyond capacity will
		/// throw an exception.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than zero.</exception>
		public CircularBuffer(int capacity, bool allowOverwrite)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(capacity, 0);

			array = new T[capacity];
			AllowOverwrite = allowOverwrite;
			count = 0;
			head = 0;
			tail = 0;
		}

		/// <summary>
		/// Initializes a new <see cref="CircularBuffer{T}" /> by copying elements from the specified collection. Uses
		/// <see cref="DefaultCapacity" /> as the capacity. Oldest items are overwritten if necessary.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be null.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <remarks>
		/// If the number of elements in <paramref name="collection" /> exceeds <see cref="DefaultCapacity" />, only the most recent items
		/// are retained.
		/// </remarks>
		public CircularBuffer(IEnumerable<T> collection)
			: this(collection, DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new <see cref="CircularBuffer{T}" /> by copying elements from the specified collection and applying the given capacity.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be null.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be non-negative.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than zero.</exception>
		/// <remarks>
		/// If the number of elements in <paramref name="collection" /> exceeds <paramref name="capacity" />, the most recent items are retained.
		/// </remarks>
		public CircularBuffer(IEnumerable<T> collection, int capacity)
			: this(collection, capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new <see cref="CircularBuffer{T}" /> by copying elements from the specified collection, and using the specified
		/// capacity and overwrite behavior.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be null.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <param name="allowOverwrite">
		/// If <see langword="true" />, the most recent elements from the collection are retained if its size exceeds capacity. If
		/// <see langword="false" />, the collection size must not exceed capacity, or an exception is thrown.
		/// </param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than zero.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="allowOverwrite" /> is <see langword="false" /> and the collection size exceeds <paramref name="capacity" />.
		/// </exception>
		public CircularBuffer(IEnumerable<T> collection, int capacity, bool allowOverwrite)
		{
			ThrowHelper.ThrowIfNull(collection);
			ThrowHelper.ThrowIfLessThanOrEqual(capacity, 0);

			var items = collection as T[] ?? collection.ToArray();

			if (items.Length > capacity && !allowOverwrite)
				throw new InvalidOperationException(ResourceStrings.Arg_Invalid_ArrayLengthExceedsCapacity);

			array = new T[capacity];
			AllowOverwrite = allowOverwrite;

			if (items.Length > capacity)
			{
				// Retain the most recent elements that fit in the buffer.
				Array.Copy(items, items.Length - capacity, array, 0, capacity);
				count = capacity;
			}
			else
			{
				Array.Copy(items, array, items.Length);
				count = items.Length;
			}

			head = 0;
			tail = count % capacity;
		}

		/// <summary>
		/// Occurs immediately <b>after</b> an item has been evicted from the <see cref="CircularBuffer{T}" /> due to capacity limits.
		/// </summary>
		/// <remarks>
		/// <para>This event is raised only when the buffer reaches capacity and <see cref="AllowOverwrite" /> is <see langword="true" />.</para>
		/// <para>
		/// The event provides the evicted item, which is no longer present in the buffer. It can be used to record history, notify
		/// observers, or synchronize with external systems.
		/// </para>
		/// <para>
		/// <b>Important:</b> Exceptions thrown from event handlers will propagate to the caller of <see cref="Enqueue" /> or
		/// <see cref="TryEnqueue" />. Consumers should handle exceptions appropriately.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>
		///<![CDATA[
		///var buffer = new CircularBuffer<string>(capacity: 2, allowOverwrite: true);
		///buffer.ItemEvicted += item => Console.WriteLine($"Evicted: {item}");
		///
		///buffer.Enqueue("A");
		///buffer.Enqueue("B");
		///buffer.Enqueue("C"); // Triggers ItemEvicted for "A"
		///]]>
		/// </code>
		/// </example>
		public event Action<T>? ItemEvicted;

		/// <summary>
		/// Occurs immediately <b>before</b> an item is evicted from the <see cref="CircularBuffer{T}" /> due to capacity limits.
		/// </summary>
		/// <remarks>
		/// <para>This event is raised only when the buffer reaches capacity and <see cref="AllowOverwrite" /> is <see langword="true" />.</para>
		/// <para>
		/// It allows consumers to inspect the item that is about to be evicted. This is useful for diagnostics, logging, or synchronizing
		/// external state.
		/// </para>
		/// <para>
		/// <b>Important:</b> Exceptions thrown from event handlers will propagate to the caller and may prevent the item from being overwritten.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code>
		///<![CDATA[
		///var buffer = new CircularBuffer<string>(capacity: 2, allowOverwrite: true);
		///buffer.ItemEvicting += item => Console.WriteLine($"Evicting: {item}");
		///
		///buffer.Enqueue("A");
		///buffer.Enqueue("B");
		///buffer.Enqueue("C"); // Triggers ItemEvicting for "A"
		///]]>
		/// </code>
		/// </example>
		public event Action<T>? ItemEvicting;

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="CircularBuffer{T}" /> will automatically overwrite the oldest element
		/// when capacity is reached.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to automatically overwrite the oldest element when adding an element to a
		/// <see cref="CircularBuffer{T}" /> where the <see cref="CircularBuffer{T}.Count" /> equals
		/// <see cref="CircularBuffer{T}.Capacity" /> otherwise, <see langword="false" />.
		/// </value>
		/// <remarks>
		/// <para>
		/// Setting the <see cref="CircularBuffer{T}.AllowOverwrite" /> to <see langword="false" /> will throw an
		/// <see cref="System.InvalidOperationException" /> if an attempt is made to add an element to a <see cref="CircularBuffer{T}" />
		/// where the <see cref="CircularBuffer{T}.Count" /> equals <see cref="CircularBuffer{T}.Capacity" />. Otherwise the oldest element
		/// is overwritten.
		/// </para>
		/// </remarks>
		public bool AllowOverwrite { get; set; }

		/// <summary>
		/// Gets the maximum number of elements that the <see cref="CircularBuffer{T}" /> can contain.
		/// </summary>
		/// <value>The maximum number of elements that the current <see cref="CircularBuffer{T}" /> can contain.</value>
		public int Capacity => this.array.Length;

		/// <summary>
		/// Gets the element at the specified zero-based index from the oldest to the newest element.
		/// </summary>
		/// <param name="index">The zero-based index (0 is the oldest element).</param>
		/// <returns>The element at the specified position.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="index" /> is negative or greater than or equal to <see cref="Count" />.
		/// </exception>
		public T this[int index]
		{
			get
			{
				ThrowHelper.ThrowIfLessThan(index, 0);
				ThrowHelper.ThrowIfGreaterThanOrEqual(index, count);

				int actualIndex = (head + index) % array.Length;
				return array[actualIndex];
			}
		}

		/// <summary>
		/// Removes all elements from the <see cref="CircularBuffer{T}" />, resetting its state.
		/// </summary>
		/// <remarks>The buffer's capacity remains unchanged after clearing.</remarks>
		public void Clear()
		{
			if (count > 0)
			{
				if (head < tail)
				{
					Array.Clear(array, head, count);
				}
				else
				{
					Array.Clear(array, head, array.Length - head);
					Array.Clear(array, 0, tail);
				}

				head = 0;
				tail = 0;
				count = 0;
				version++;
			}
		}

		/// <summary>
		/// Determines whether the buffer contains a specific element.
		/// </summary>
		/// <param name="item">The element to locate in the buffer.</param>
		/// <returns><see langword="true" /> if <paramref name="item" /> is found; otherwise, <see langword="false" />.</returns>
		public bool Contains(T item)
		{
			int index = head;
			var comparer = EqualityComparer<T>.Default;

			for (int i = 0; i < count; i++)
			{
				if (comparer.Equals(array[index], item))
					return true;

				index = (index + 1) % array.Length;
			}

			return false;
		}

		/// <summary>
		/// Copies elements from the buffer to an existing array, starting at the specified array index.
		/// </summary>
		/// <param name="array">The target array for the copied elements.</param>
		/// <param name="index">The zero-based index at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="array" /> is null.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index" /> is negative.</exception>
		/// <exception cref="ArgumentException">Thrown if insufficient space in the target array.</exception>
		public void CopyTo(T[] array, int index) =>
			((ICollection)this).CopyTo(array, index);

		/// <summary>
		/// Removes and returns the oldest element from the buffer.
		/// </summary>
		/// <returns>The oldest element in the buffer.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty.</exception>
		public T Dequeue()
		{
			TryDequeueInternal(out var item, throwIfEmpty: true);
			return item;
		}

		/// <summary>
		/// Adds an element to the end of the buffer.
		/// </summary>
		/// <param name="item">The element to add. Can be null for reference types.</param>
		/// <exception cref="InvalidOperationException">Thrown if the buffer reaches capacity and overwriting is disabled.</exception>
		public void Enqueue(T item) =>
			_ = TryEnqueueInternal(item, throwIfFull: true);

		/// <summary>
		/// Provides direct read-only access to the internal buffer storage. Returns two array segments due to potential buffer wrap-around.
		/// </summary>
		/// <returns>
		/// A tuple containing two array segments (FirstSegment, SecondSegment). Use both segments sequentially to read all elements.
		/// </returns>
		public (ArraySegment<T> FirstSegment, ArraySegment<T> SecondSegment) GetSegments()
		{
			if (count == 0)
				return (new ArraySegment<T>(Array.Empty<T>()), new ArraySegment<T>(Array.Empty<T>()));

			if (head < tail)
			{
				// Single continuous segment
				return (new ArraySegment<T>(array, head, count), new ArraySegment<T>(Array.Empty<T>()));
			}
			else
			{
				// Wrapped segments
				int firstLength = array.Length - head;
				var firstSegment = new ArraySegment<T>(array, head, firstLength);
				var secondSegment = new ArraySegment<T>(array, 0, tail);

				return (firstSegment, secondSegment);
			}
		}

		/// <summary>
		/// Returns the oldest element without removing it from the buffer.
		/// </summary>
		/// <returns>The oldest element in the buffer.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty.</exception>
		public T Peek()
		{
			if (count == 0) throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionEmpty);

			return array[head];
		}

		/// <summary>
		/// Copies the buffer elements to a new array.
		/// </summary>
		/// <returns>A new array containing the buffer's elements from oldest to newest.</returns>
		public T[] ToArray()
		{
			T[] result = new T[count];

			if (count == 0)
				return result;

			if (head < tail)
			{
				Array.Copy(array, head, result, 0, count);
			}
			else
			{
				int firstSegment = array.Length - head;
				Array.Copy(array, head, result, 0, firstSegment);
				Array.Copy(array, 0, result, firstSegment, tail);
			}

			return result;
		}

		/// <summary>
		/// Reduces the buffer capacity to match the current count, freeing unused memory.
		/// </summary>
		/// <remarks>
		/// Useful if the buffer previously had a large capacity but now requires less memory.
		/// <para>
		/// If the buffer is empty ( <see cref="Count" /> is 0), the capacity is reduced to a minimal internal size (typically 1) to ensure
		/// the buffer remains usable.
		/// </para>
		/// </remarks>
		public void TrimExcess()
		{
			if (Count == Capacity)
				return; // No trimming needed

			int size = Math.Max(count, 1); // prevent zero-sized array
			T[] trimmed = new T[size];
			CopyTo(trimmed, 0);

			array = trimmed;
			head = 0;
			tail = count % array.Length;
			version++;
		}

		/// <summary>
		/// Attempts to remove and return the oldest item without throwing exceptions.
		/// </summary>
		/// <param name="item">The item removed from the buffer.</param>
		/// <returns><see langword="true" /> if an item was dequeued successfully; <see langword="false" /> if the buffer was empty.</returns>
		public bool TryDequeue(out T item) =>
			TryDequeueInternal(out item, throwIfEmpty: false);

		/// <summary>
		/// Attempts to add an item to the buffer without throwing exceptions.
		/// </summary>
		/// <param name="item">The item to enqueue.</param>
		/// <returns>
		/// <see langword="true" /> if the item was enqueued successfully; <see langword="false" /> if the buffer is full and overwriting is disabled.
		/// </returns>
		public bool TryEnqueue(T item) =>
			TryEnqueueInternal(item, throwIfFull: false);

		/// <summary>
		/// Attempts to retrieve the oldest element without removing it from the buffer.
		/// </summary>
		/// <param name="item">The oldest element in the buffer.</param>
		/// <returns><see langword="true" /> if an item was retrieved successfully; <see langword="false" /> if the buffer was empty.</returns>
		public bool TryPeek(out T item)
		{
			if (count == 0)
			{
				item = default!;
				return false;
			}

			item = array[head];
			return true;
		}

		/// <summary>
		/// Attempts to dequeue an item from the internal circular buffer.
		/// </summary>
		/// <param name="item">When this method returns, contains the dequeued item if successful; otherwise, the default value of <typeparamref name="T" />.</param>
		/// <param name="throwIfEmpty">
		/// If set to <c>true</c>, throws an <see cref="InvalidOperationException" /> when the buffer is empty; otherwise, returns
		/// <c>false</c> without throwing.
		/// </param>
		/// <returns><c>true</c> if an item was successfully dequeued; <c>false</c> if the buffer was empty.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="throwIfEmpty" /> is <c>true</c> and the buffer is empty.
		/// </exception>
		private bool TryDequeueInternal(out T item, bool throwIfEmpty)
		{
			if (count == 0)
			{
				if (throwIfEmpty)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_EmptySequence);

				item = default!;
				return false;
			}

			item = array[head];
			array[head] = default!;
			head = (head + 1) % array.Length;
			count--;
			version++;

			return true;
		}

		/// <summary>
		/// Attempts to enqueue an item into the internal circular buffer.
		/// </summary>
		/// <param name="item">The item to enqueue into the buffer.</param>
		/// <param name="throwIfFull">
		/// If set to <c>true</c>, throws an <see cref="InvalidOperationException" /> when the buffer is full and overwriting is disallowed;
		/// otherwise, returns <c>false</c> without throwing.
		/// </param>
		/// <returns><c>true</c> if the item was successfully enqueued; <c>false</c> if the buffer was full and overwriting was disallowed.</returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="throwIfFull" /> is <c>true</c>, the buffer is full, and overwriting items is disallowed or the buffer
		/// capacity is zero.
		/// </exception>
		private bool TryEnqueueInternal(T item, bool throwIfFull)
		{
			if (Capacity == 0)
			{
				if (throwIfFull)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CapacityExhausted);

				return false;
			}

			if (count == array.Length)
			{
				if (!AllowOverwrite)
				{
					if (throwIfFull)
						throw new InvalidOperationException(ResourceStrings.InvalidOperation_CapacityExhausted);

					return false;
				}

				T overwritten = array[tail];
				ItemEvicting?.Invoke(overwritten);

				array[tail] = item;
				head = tail = (tail + 1) % array.Length;

				ItemEvicted?.Invoke(overwritten);
			}
			else
			{
				array[tail] = item;
				tail = (tail + 1) % array.Length;
				count++;
			}

			version++;

			return true;
		}
	}
}
