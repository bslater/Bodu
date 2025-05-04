// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="CircularBuffer.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Diagnostics;

namespace Bodu.Collections.Generic
{
	/// <summary>
	/// Represents a first-in, first-out (FIFO) collection of elements using a fixed-size circular buffer with optional overwrite support.
	/// </summary>
	/// <typeparam name="T">Specifies the type of elements in the collection.</typeparam>
	/// <remarks>
	/// <para>
	/// <see cref="CircularBuffer{T}" /> is a high-performance collection for storing a bounded number of elements in a circular manner.
	/// Elements are inserted at the tail and removed from the head. When the buffer reaches capacity, new elements overwrite the oldest
	/// entries if <see cref="AllowOverwrite" /> is <see langword="true" />.
	/// </para>
	/// <para>
	/// This type is not thread-safe. If concurrent access is required, use
	/// <see cref="Bodu.Collections.Generic.Concurrent.ConcurrentCircularBuffer{T}" /> instead.
	/// </para>
	/// <para>Key operations include:</para>
	/// <list type="bullet">
	/// <item>
	/// <description><see cref="Enqueue" /> and <see cref="TryEnqueue" /> — Add elements to the buffer.</description>
	/// </item>
	/// <item>
	/// <description><see cref="Dequeue" /> and <see cref="TryDequeue" /> — Remove and return the oldest element.</description>
	/// </item>
	/// <item>
	/// <description><see cref="Peek" /> and <see cref="TryPeek" /> — View the oldest element without removing it.</description>
	/// </item>
	/// </list>
	/// <para>
	/// The <see cref="Capacity" /> property defines the maximum number of elements the buffer can hold. If the buffer is full and
	/// <see cref="AllowOverwrite" /> is <see langword="false" />, attempts to enqueue additional elements will throw an <see cref="InvalidOperationException" />.
	/// </para>
	/// <para><see cref="CircularBuffer{T}" /> accepts <see langword="null" /> values (for reference types) and allows duplicate elements.</para>
	/// </remarks>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CircularBufferDebugView<>))]
	[Serializable]
	public partial class CircularBuffer<T>
	{
		private const int DefaultCapacity = 16;
#if !NET6_0_OR_GREATER
		private const int MaxArrayLength = 0x7FFFFFC7; // 2,147,483,647 - 1
#endif

		private T[] array;
		private int count;
		private int capacity;
		private int head;
		private int tail;
		private int version;

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class using the default capacity and allowing overwrites by default.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This constructor initializes the buffer with a default capacity defined by the internal <c>DefaultCapacity</c> constant. When
		/// the buffer becomes full, new items will overwrite the oldest elements.
		/// </para>
		/// <para>
		/// To customize capacity or overwrite behavior, use an overloaded constructor such as
		/// <see cref="CircularBuffer{T}.CircularBuffer(int)" /> or <see cref="CircularBuffer{T}.CircularBuffer(int, bool)" />.
		/// </para>
		/// </remarks>
		public CircularBuffer()
					: this(DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class with the specified capacity, allowing overwrites when full.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// <para>
		/// When the number of items exceeds <paramref name="capacity" />, older elements are automatically overwritten. To disable
		/// overwrites, use the <see cref="CircularBuffer{T}.CircularBuffer(int, bool)" /> constructor with <c>allowOverwrite: false</c>.
		/// </para>
		/// </remarks>
		public CircularBuffer(int capacity)
					: this(capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class with the specified capacity and overwrite behavior.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <param name="allowOverwrite">
		/// <see langword="true" /> to allow the buffer to automatically overwrite the oldest elements when full; <see langword="false" />
		/// to prevent adding new elements when the buffer has reached capacity, which will cause an exception to be thrown during insertion.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// Use this constructor when you need to control whether the buffer should overwrite old data once full or strictly enforce capacity.
		/// </remarks>
		public CircularBuffer(int capacity, bool allowOverwrite)
		{
#if NET6_0_OR_GREATER
			ThrowHelper.ThrowIfOutOfRange(capacity, 1, Array.MaxLength);
#else
			ThrowHelper.ThrowIfNotBetweenInclusive(capacity, 1, MaxArrayLength);
#endif
			this.array = new T[capacity];
			this.capacity = capacity;
			this.AllowOverwrite = allowOverwrite;
			this.count = 0;
			this.head = 0;
			this.tail = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class by copying elements from the specified collection,
		/// using the default capacity and allowing overwriting if needed.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be <see langword="null" />.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <remarks>
		/// The default capacity is defined by <c>DefaultCapacity</c>. If the collection contains more elements than the buffer can hold,
		/// only the most recent items up to the buffer's capacity are retained. Older items are discarded during construction.
		/// </remarks>
		public CircularBuffer(IEnumerable<T> collection)
					: this(collection, DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class by copying elements from the specified collection and
		/// applying the specified capacity. Overwriting is enabled by default.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be <see langword="null" />.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// If the number of elements in <paramref name="collection" /> exceeds <paramref name="capacity" />, only the most recent items are
		/// retained. Older items are discarded during construction.
		/// </remarks>
		public CircularBuffer(IEnumerable<T> collection, int capacity)
					: this(collection, capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="CircularBuffer{T}" /> class by copying elements from the specified collection,
		/// applying the specified capacity and overwrite behavior.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied. Must not be <see langword="null" />.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <param name="allowOverwrite">
		/// If <see langword="true" />, the most recent elements from the collection are retained if its size exceeds
		/// <paramref name="capacity" />. If <see langword="false" />, the collection size must not exceed <paramref name="capacity" />, or
		/// an exception is thrown.
		/// </param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="allowOverwrite" /> is <see langword="false" /> and the collection contains more elements than the
		/// buffer capacity.
		/// </exception>
		/// <remarks>
		/// When the number of items in the collection exceeds the capacity, only the most recent items are copied into the buffer (if
		/// overwriting is allowed).
		/// </remarks>
		public CircularBuffer(IEnumerable<T> collection, int capacity, bool allowOverwrite)
		{
			ThrowHelper.ThrowIfNull(collection);
#if NET6_0_OR_GREATER
			ThrowHelper.ThrowIfOutOfRange(capacity, 1, Array.MaxLength);
#else
			ThrowHelper.ThrowIfNotBetweenInclusive(capacity, 1, MaxArrayLength);
#endif

			var items = collection as T[] ?? collection.ToArray();

			if (items.Length > capacity && !allowOverwrite)
				throw new InvalidOperationException(ResourceStrings.Arg_Invalid_ArrayLengthExceedsCapacity);

			this.array = new T[capacity];
			this.capacity = capacity;
			this.AllowOverwrite = allowOverwrite;

			if (items.Length > capacity)
			{
				// Retain the most recent elements that fit in the buffer.
				Array.Copy(items, items.Length - capacity, this.array, 0, capacity);
				this.count = capacity;
			}
			else
			{
				Array.Copy(items, this.array, items.Length);
				this.count = items.Length;
			}

			this.head = 0;
			this.tail = this.count % capacity;
		}

		/// <summary>
		/// Occurs immediately <b>after</b> an item has been evicted from the <see cref="CircularBuffer{T}" /> due to capacity limits.
		/// </summary>
		/// <remarks>
		/// <para>This event is raised only when the buffer reaches capacity and <see cref="AllowOverwrite" /> is <see langword="true" />.</para>
		/// <para>
		/// It provides access to the evicted item, which has been removed from the buffer. You can use this event to log history, trigger
		/// cleanup actions, or propagate notifications.
		/// </para>
		/// <para>
		/// <b>Important:</b> Exceptions thrown by event handlers are not caught and will propagate to the caller of <see cref="Enqueue" />
		/// or <see cref="TryEnqueue" />. Consumers should ensure event handlers are exception-safe.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
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
		/// <para>This event is raised only when the buffer has reached its capacity and <see cref="AllowOverwrite" /> is <see langword="true" />.</para>
		/// <para>
		/// It allows consumers to inspect the item that is about to be removed from the buffer. This can be useful for pre-eviction
		/// validation, logging, synchronization with external systems, or veto logic.
		/// </para>
		/// <para>
		/// <b>Important:</b> If the event handler throws an exception, the item will not be evicted, and the <see cref="Enqueue" /> or
		/// <see cref="TryEnqueue" /> operation will propagate that exception. Event handlers should therefore avoid throwing unless intentional.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
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
		/// <see langword="true" /> to overwrite the oldest item when the buffer is full; <see langword="false" /> to throw an exception
		/// instead of overwriting.
		/// </value>
		/// <remarks>
		/// <para>
		/// When <see cref="AllowOverwrite" /> is set to <see langword="true" />, the buffer permits new items to overwrite the oldest item
		/// once <see cref="Capacity" /> is reached.
		/// </para>
		/// <para>
		/// When set to <see langword="false" />, attempting to <see cref="Enqueue(T)" /> or <see cref="TryEnqueue(T)" /> into a full buffer
		/// will throw an <see cref="InvalidOperationException" /> or return <see langword="false" />, respectively.
		/// </para>
		/// <para>This property can be toggled at runtime to change eviction behavior dynamically.</para>
		/// </remarks>
		public bool AllowOverwrite { get; set; }

		/// <summary>
		/// Gets the maximum number of elements that the <see cref="CircularBuffer{T}" /> can contain.
		/// </summary>
		/// <value>The total capacity of the buffer, which determines how many elements it can hold before reaching its limit.</value>
		/// <remarks>
		/// <para>
		/// The <see cref="Capacity" /> is fixed at construction and defines the maximum number of items that can be stored in the buffer at
		/// once. If <see cref="AllowOverwrite" /> is <see langword="true" />, adding an item to a full buffer will evict the oldest item.
		/// Otherwise, an exception is thrown or the addition fails depending on the method used.
		/// </para>
		/// <para>To reduce memory usage after elements are removed, use <see cref="TrimExcess" /> to shrink the buffer.</para>
		/// </remarks>
		public int Capacity => this.capacity;

		/// <summary>
		/// Gets the element at the specified zero-based index from the oldest to the newest element.
		/// </summary>
		/// <param name="index">The zero-based index of the element to retrieve (0 refers to the oldest element).</param>
		/// <returns>The element stored at the specified index within the buffer.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index" /> is negative or greater than or equal to <see cref="Count" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The indexer provides read-only access to the elements in the buffer in logical order (from oldest to newest). Internally, the
		/// circular structure is resolved to return the correct element corresponding to the requested index.
		/// </para>
		/// <para>
		/// The index is relative to the logical start of the buffer. That is, <c>buffer[0]</c> returns the oldest element, and
		/// <c>buffer[Count - 1]</c> returns the most recently added item.
		/// </para>
		/// </remarks>
		public T this[int index]
		{
			get
			{
				ThrowHelper.ThrowIfLessThan(index, 0);
				ThrowHelper.ThrowIfGreaterThanOrEqual(index, this.count);

				int actualIndex = (this.head + index) % this.capacity;
				return array[actualIndex];
			}
		}

		/// <summary>
		/// Removes all elements from the <see cref="CircularBuffer{T}" />, resetting its state.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method clears the buffer by resetting internal state and clearing array contents. After calling <see cref="Clear" />, the
		/// buffer’s <see cref="Count" /> becomes <c>0</c>, but the <see cref="Capacity" /> remains unchanged.
		/// </para>
		/// <para>
		/// Any subscribed <see cref="ItemEvicting" /> or <see cref="ItemEvicted" /> handlers are not invoked by this operation, as
		/// <see cref="Clear" /> does not count as eviction.
		/// </para>
		/// </remarks>
		public void Clear()
		{
			if (this.count > 0)
			{
				if (this.head < this.tail)
				{
					Array.Clear(this.array, this.head, this.count);
				}
				else
				{
					Array.Clear(this.array, this.head, this.capacity - this.head);
					Array.Clear(this.array, 0, this.tail);
				}

				this.head = 0;
				this.tail = 0;
				this.count = 0;
			}
		}

		/// <summary>
		/// Determines whether the buffer contains a specific element.
		/// </summary>
		/// <param name="item">The element to locate in the buffer. Can be <see langword="null" /> for reference types.</param>
		/// <returns><see langword="true" /> if <paramref name="item" /> exists in the buffer; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// <para>This method performs a linear scan from the oldest to the newest element using the default equality comparer for <typeparamref name="T" />.</para>
		/// <para>The search is read-only and does not modify the buffer's state.</para>
		/// </remarks>
		public bool Contains(T item)
		{
			int index = this.head;
			var comparer = EqualityComparer<T>.Default;

			for (int i = 0; i < this.count; i++)
			{
				if (comparer.Equals(this.array[index], item))
					return true;

				index = (index + 1) % this.capacity;
			}

			return false;
		}

		/// <summary>
		/// Copies elements from the buffer to the specified target array, starting at the given array index.
		/// </summary>
		/// <param name="array">The destination array to copy elements into. Must not be <see langword="null" />.</param>
		/// <param name="index">The zero-based index in the destination array at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index" /> is negative.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the number of elements in the buffer exceeds the available space in the target array starting at the specified <paramref name="index" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// Elements are copied in logical FIFO order—from the oldest to the newest. If the buffer is wrapped internally, this method
		/// handles segmenting the copy operation appropriately.
		/// </para>
		/// <para>The target array must be large enough to accommodate the number of elements in the buffer.</para>
		/// </remarks>
		public void CopyTo(T[] array, int index)
		{
			ThrowHelper.ThrowIfNull(array);
			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, index, this.count);
			CopyToCore(array, index, isTypedArray: true);
		}

		/// <summary>
		/// Removes and returns the oldest element from the buffer.
		/// </summary>
		/// <returns>The oldest element in the buffer.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty when <see cref="Dequeue" /> is called.</exception>
		/// <remarks>
		/// <para>
		/// This method removes the element that has been in the buffer the longest (FIFO behavior). If the buffer is empty, an
		/// <see cref="InvalidOperationException" /> is thrown.
		/// </para>
		/// <para>Use <see cref="TryDequeue(out T)" /> to avoid exceptions when the buffer may be empty.</para>
		/// </remarks>
		public T Dequeue()
		{
			TryDequeueInternal(out var item, throwIfEmpty: true);
			return item;
		}

		/// <summary>
		/// Adds an element to the end of the buffer.
		/// </summary>
		/// <param name="item">The element to add. Can be <see langword="null" /> for reference types.</param>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the buffer is at full capacity and <see cref="AllowOverwrite" /> is <see langword="false" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// If <see cref="AllowOverwrite" /> is <see langword="true" />, the oldest element in the buffer is evicted to make room for the
		/// new item. If <see cref="AllowOverwrite" /> is <see langword="false" />, an exception is thrown when the buffer is full.
		/// </para>
		/// <para>To avoid exceptions when the buffer may be full, use <see cref="TryEnqueue(T)" /> instead.</para>
		/// </remarks>
		public void Enqueue(T item) =>
			_ = TryEnqueueInternal(item, throwIfFull: true);

		/// <summary>
		/// Provides direct read-only access to the internal buffer storage. Returns two array segments due to potential buffer wrap-around.
		/// </summary>
		/// <returns>
		/// A tuple containing two <see cref="ArraySegment{T}" /> values:
		/// <list type="bullet">
		/// <item>
		/// <description><c>FirstSegment</c> — The first segment of contiguous elements in the buffer.</description>
		/// </item>
		/// <item>
		/// <description><c>SecondSegment</c> — The second segment if the buffer wraps around; otherwise, an empty segment.</description>
		/// </item>
		/// </list>
		/// Iterate over both segments sequentially to access all elements in order from oldest to newest.
		/// </returns>
		/// <remarks>
		/// <para>
		/// If the buffer has not wrapped around, only the <c>FirstSegment</c> will contain elements. If wrap-around has occurred, elements
		/// will be split between <c>FirstSegment</c> and <c>SecondSegment</c>.
		/// </para>
		/// <para>This method is useful for reading buffer contents efficiently without allocating a new array.</para>
		/// </remarks>
		public (ArraySegment<T> FirstSegment, ArraySegment<T> SecondSegment) GetSegments()
		{
			if (this.count == 0)
				return (new ArraySegment<T>(Array.Empty<T>()), new ArraySegment<T>(Array.Empty<T>()));

			if (this.head < this.tail)
			{
				// Single continuous segment
				return (new ArraySegment<T>(this.array, this.head, this.count), new ArraySegment<T>(Array.Empty<T>()));
			}
			else
			{
				// Wrapped segments
				int firstLength = this.capacity - this.head;
				var firstSegment = new ArraySegment<T>(this.array, this.head, firstLength);
				var secondSegment = new ArraySegment<T>(this.array, 0, this.tail);

				return (firstSegment, secondSegment);
			}
		}

		/// <summary>
		/// Returns the oldest element in the <see cref="CircularBuffer{T}" /> without removing it.
		/// </summary>
		/// <returns>The element at the front of the buffer (the oldest item).</returns>
		/// <exception cref="InvalidOperationException">Thrown when the buffer is empty (i.e., <see cref="Count" /> equals <c>0</c>).</exception>
		/// <remarks>
		/// Use <see cref="Peek" /> to inspect the current front of the buffer without modifying its contents. If you need to remove the
		/// item as well, use <see cref="Dequeue" />.
		/// </remarks>
		public T Peek()
		{
			if (this.count == 0)
				throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionEmpty);

			return this.array[head];
		}

		/// <summary>
		/// Copies the contents of the <see cref="CircularBuffer{T}" /> to a new array.
		/// </summary>
		/// <returns>A new one-dimensional array containing the buffer's elements, ordered from oldest to newest.</returns>
		/// <remarks>
		/// This method creates a shallow copy of the buffer's contents. It is useful for inspection, snapshotting, or passing the elements
		/// to APIs that require array input.
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///var buffer = new CircularBuffer&lt;int&gt;(3);
		///buffer.Enqueue(1);
		///buffer.Enqueue(2);
		///buffer.Enqueue(3);
		///int[] copy = buffer.ToArray(); // copy = [1, 2, 3]
		/// </code>
		/// </example>
		public T[] ToArray()
		{
			T[] result = new T[this.count];
			if (this.count > 0)
			{
				CopyToCore(result, 0, isTypedArray: true);
			}
			return result;
		}

		/// <summary>
		/// Reduces the internal capacity of the <see cref="CircularBuffer{T}" /> to match the current number of elements, freeing unused memory.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method is useful in scenarios where the buffer previously had a large capacity but no longer needs to retain that size. It
		/// creates a new internal array sized to the current <see cref="Count" /> and copies the existing elements into it.
		/// </para>
		/// <para>
		/// If the buffer is empty, the internal storage is reduced to a minimal size (at least one element) to ensure the buffer remains operational.
		/// </para>
		/// <para>
		/// After trimming, the <see cref="Capacity" /> will equal the current <see cref="Count" />, and the buffer will be reset to a
		/// zero-based internal index layout.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///var buffer = new CircularBuffer&lt;string&gt;(100);
		///buffer.Enqueue("A");
		///buffer.Enqueue("B");
		///buffer.TrimExcess(); // Reduces capacity to 2
		/// </code>
		/// </example>
		public void TrimExcess()
		{
			if (this.count == this.capacity)
				return;

			int newCapacity = Math.Max(this.count, 1); // prevent zero-sized array
			T[] trimmed = new T[newCapacity];
			CopyTo(trimmed, 0);

			this.array = trimmed;
			this.capacity = newCapacity;
			this.head = this.tail = 0;
			this.version++;
		}

		/// <summary>
		/// Attempts to remove and return the oldest element from the <see cref="CircularBuffer{T}" /> without throwing an exception.
		/// </summary>
		/// <param name="item">
		/// When this method returns, contains the dequeued element if one was available; otherwise, the default value of <typeparamref name="T" />.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if an item was successfully dequeued; otherwise, <see langword="false" /> if the buffer was empty.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method is safe to call when the buffer may be empty. It avoids throwing <see cref="InvalidOperationException" /> and
		/// instead returns <see langword="false" /> if no items are available.
		/// </para>
		/// <para>Use this method when performance is critical or when you want to avoid exception-based control flow in empty-buffer scenarios.</para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		///var buffer = new CircularBuffer<string>(2);
		///buffer.Enqueue("A");
		///if (buffer.TryDequeue(out var value))
		///Console.WriteLine($"Removed: {value}");
		///]]>
		/// </code>
		/// </example>
		public bool TryDequeue(out T item) =>
			TryDequeueInternal(out item, throwIfEmpty: false);

		/// <summary>
		/// Attempts to add an element to the end of the <see cref="CircularBuffer{T}" /> without throwing an exception.
		/// </summary>
		/// <param name="item">The element to add. Can be <see langword="null" /> for reference types.</param>
		/// <returns>
		/// <see langword="true" /> if the item was successfully enqueued; <see langword="false" /> if the buffer is full and
		/// <see cref="AllowOverwrite" /> is <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method is safe to call when the buffer may be at capacity. It avoids throwing <see cref="InvalidOperationException" /> and
		/// instead returns <see langword="false" /> if the item could not be enqueued.
		/// </para>
		/// <para>Use this method when performance is critical or when you want to avoid exception-based control flow in full-buffer scenarios.</para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		///var buffer = new CircularBuffer<string>(2, allowOverwrite: false);
		///if (!buffer.TryEnqueue("X"))
		///Console.WriteLine("Item could not be added.");
		///]]>
		/// </code>
		/// </example>
		public bool TryEnqueue(T item) =>
			TryEnqueueInternal(item, throwIfFull: false);

		/// <summary>
		/// Attempts to retrieve the oldest element from the <see cref="CircularBuffer{T}" /> without removing it.
		/// </summary>
		/// <param name="item">
		/// When this method returns, contains the oldest element if the buffer is not empty; otherwise, contains the default value for <typeparamref name="T" />.
		/// </param>
		/// <returns><see langword="true" /> if an item was successfully retrieved; <see langword="false" /> if the buffer is empty.</returns>
		/// <remarks>
		/// <para>Use this method to inspect the oldest item in the buffer without modifying the buffer's contents.</para>
		/// <para>
		/// This method avoids throwing exceptions when the buffer is empty, making it suitable for high-throughput or exception-sensitive
		/// code paths.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		///var buffer = new CircularBuffer<string>(2);
		///buffer.Enqueue(10);
		///if (buffer.TryPeek(out int value))
		///Console.WriteLine($"Peeked: {value}");
		///]]>
		/// </code>
		/// </example>
		public bool TryPeek(out T item)
		{
			if (count == 0)
			{
				item = default!;
				return false;
			}

			item = this.array[head];
			return true;
		}

		/// <summary>
		/// Attempts to remove and return the oldest element from the <see cref="CircularBuffer{T}" />.
		/// </summary>
		/// <param name="item">When this method returns, contains the dequeued item if successful; otherwise, the default value of <typeparamref name="T" />.</param>
		/// <param name="throwIfEmpty">
		/// If <see langword="true" />, throws an <see cref="InvalidOperationException" /> when the buffer is empty; if
		/// <see langword="false" />, returns <see langword="false" /> without throwing.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if an element was successfully dequeued; otherwise, <see langword="false" /> if the buffer was empty and
		/// <paramref name="throwIfEmpty" /> was <see langword="false" />.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="throwIfEmpty" /> is <see langword="true" /> and the buffer is empty.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This internal method underpins both <see cref="Dequeue" /> and <see cref="TryDequeue" />, and provides a shared path for
		/// controlled exception handling.
		/// </para>
		/// </remarks>
		private bool TryDequeueInternal(out T item, bool throwIfEmpty)
		{
			if (count == 0)
			{
				if (throwIfEmpty)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_EmptySequence);

				item = default!;
				return false;
			}

			item = this.array[head];
			this.array[head] = default!;
			this.head = (head + 1) % this.capacity;
			count--;

			return true;
		}

		/// <summary>
		/// Attempts to enqueue an item into the internal <see cref="CircularBuffer{T}" />.
		/// </summary>
		/// <param name="item">The element to add. Can be <see langword="null" /> for reference types.</param>
		/// <param name="throwIfFull">
		/// If <see langword="true" />, throws an <see cref="InvalidOperationException" /> when the buffer is full and overwriting is
		/// disallowed; otherwise, returns <see langword="false" /> without throwing.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the item was enqueued successfully; <see langword="false" /> if the buffer was full and overwriting
		/// was disallowed and <paramref name="throwIfFull" /> was <see langword="false" />.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="throwIfFull" /> is <see langword="true" /> and the buffer is full while
		/// <see cref="AllowOverwrite" /> is <see langword="false" />, or the buffer’s capacity is zero.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method is used by both <see cref="Enqueue" /> and <see cref="TryEnqueue" /> to centralize the enqueue logic and exception control.
		/// </para>
		/// <para>
		/// If <see cref="AllowOverwrite" /> is enabled, the oldest element is evicted to make room for the new item, and the
		/// <see cref="ItemEvicting" /> and <see cref="ItemEvicted" /> events are raised.
		/// </para>
		/// </remarks>
		private bool TryEnqueueInternal(T item, bool throwIfFull)
		{
			if (this.capacity == 0)
			{
				if (throwIfFull)
					throw new InvalidOperationException(ResourceStrings.InvalidOperation_CapacityExhausted);

				return false;
			}

			if (count == array.Length)
			{
				if (!this.AllowOverwrite)
				{
					if (throwIfFull)
						throw new InvalidOperationException(ResourceStrings.InvalidOperation_CapacityExhausted);

					return false;
				}

				T overwritten = array[tail];
				this.ItemEvicting?.Invoke(overwritten);

				this.array[tail] = item;
				this.head = this.tail = (this.tail + 1) % this.capacity;

				ItemEvicted?.Invoke(overwritten);
			}
			else
			{
				this.array[tail] = item;
				this.tail = (this.tail + 1) % this.capacity;
				count++;
			}

			this.version++;

			return true;
		}

		/// <summary>
		/// Copies the contents of the circular buffer to the specified <see cref="Array" /> starting at the given index.
		/// </summary>
		/// <param name="destination">
		/// The destination array to which elements from the buffer will be copied. Must not be <see langword="null" /> and must have
		/// sufficient space.
		/// </param>
		/// <param name="destinationIndex">The zero-based index in the destination array at which copying begins.</param>
		/// <param name="isTypedArray">
		/// Indicates whether <paramref name="destination" /> is a strongly typed array of type <typeparamref name="T" />. If
		/// <see langword="false" />, a runtime cast is performed to validate element compatibility (used for non-generic
		/// <see cref="ICollection.CopyTo(Array, int)" /> scenarios).
		/// </param>
		/// <remarks>
		/// <para>
		/// This method performs the core logic for copying elements from the buffer to an external array, handling both contiguous and
		/// wrapped buffer layouts.
		/// </para>
		/// <para>
		/// If the buffer is empty, no operation is performed. If the buffer wraps around its internal array boundary, copying occurs in two segments.
		/// </para>
		/// <para>
		/// For non-typed destination arrays, a runtime type check is performed to ensure compatibility. If the types are incompatible, an
		/// <see cref="ArrayTypeMismatchException" /> will be thrown (typically caught and rethrown as an <see cref="ArgumentException" />
		/// by the caller).
		/// </para>
		/// </remarks>
		/// <exception cref="ArrayTypeMismatchException">
		/// Thrown when <paramref name="isTypedArray" /> is <see langword="false" /> and the buffer's element type <typeparamref name="T" />
		/// is not compatible with the destination array's element type.
		/// </exception>
		private void CopyToCore(Array destination, int destinationIndex, bool isTypedArray)
		{
			if (this.count == 0)
				return;

			if (this.head < this.tail)
			{
				Array.Copy(this.array, this.head, destination, destinationIndex, this.count);
			}
			else
			{
				int firstSegmentLength = this.array.Length - this.head;
				Array.Copy(this.array, this.head, destination, destinationIndex, firstSegmentLength);
				Array.Copy(this.array, 0, destination, destinationIndex + firstSegmentLength, this.tail);
			}

			// Type check for non-generic CopyTo to ensure type safety
			if (!isTypedArray)
			{
				// Will throw if there's a type mismatch (ArrayTypeMismatchException is handled by caller)
				_ = (T)destination.GetValue(destinationIndex)!;
			}
		}
	}
}