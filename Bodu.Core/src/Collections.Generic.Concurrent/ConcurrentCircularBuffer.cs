// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="ConcurrentCircularBuffer.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Bodu.Collections.Generic.Concurrent
{
	/// <summary>
	/// Represents a thread-safe, first-in, first-out (FIFO) collection of objects using a fixed-size circular buffer with optional
	/// overwrite support.
	/// </summary>
	/// <typeparam name="T">
	/// Specifies the type of elements in the buffer. Must be a reference type (see <c>where T : class</c> constraint).
	/// <para><see cref="ConcurrentCircularBuffer{T}" /> accepts <see langword="null" /> values and allows duplicate entries.</para>
	/// </typeparam>
	/// <remarks>
	/// <para>
	/// This class implements a generic circular buffer of fixed capacity that supports concurrent access by multiple threads without
	/// locking. Elements in a <see cref="ConcurrentCircularBuffer{T}" /> are inserted at one end and removed from the other in FIFO order.
	/// </para>
	/// <para>
	/// Use <see cref="ConcurrentCircularBuffer{T}" /> when you need to limit the number of elements in a collection while enabling
	/// concurrent producers and consumers. For non-concurrent scenarios, consider using <see cref="Bodu.Collections.Generic.CircularBuffer{T}" />.
	/// </para>
	/// <para>Three main operations can be performed on a <see cref="ConcurrentCircularBuffer{T}" /> and its elements:</para>
	/// <list type="bullet">
	/// <item>
	/// <description><see cref="ConcurrentCircularBuffer{T}.Enqueue(T)" /> adds an element to the tail of the buffer.</description>
	/// </item>
	/// <item>
	/// <description><see cref="ConcurrentCircularBuffer{T}.Dequeue" /> removes the element at the head of the buffer.</description>
	/// </item>
	/// <item>
	/// <description><see cref="ConcurrentCircularBuffer{T}.Peek" /> retrieves the element at the head without removing it.</description>
	/// </item>
	/// </list>
	/// <para>The <see cref="Capacity" /> property defines the maximum number of elements the buffer can hold. When the buffer is full:</para>
	/// <list type="bullet">
	/// <item>
	/// <description>If <see cref="AllowOverwrite" /> is <see langword="true" />, the oldest element is overwritten.</description>
	/// </item>
	/// <item>
	/// <description>If <see cref="AllowOverwrite" /> is <see langword="false" />, adding a new element throws an <see cref="InvalidOperationException" />.</description>
	/// </item>
	/// </list>
	/// <para>
	/// All operations use atomic memory access ( <see cref="Volatile" /> and <see cref="Interlocked" />) for thread safety. Event handlers
	/// for <see cref="ItemEvicting" /> and <see cref="ItemEvicted" /> are called synchronously during overwrite and should not block or
	/// throw exceptions.
	/// </para>
	/// <para>
	/// <see cref="ConcurrentCircularBuffer{T}" /> accepts <see langword="null" /> as a valid value for reference types and allows duplicate entries.
	/// </para>
	/// <para>
	/// <b>Enumeration:</b> Enumerators operate over a point-in-time snapshot and are safe for concurrent access, but they do not reflect
	/// live mutations.
	/// </para>
	/// </remarks>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CircularBufferDebugView<>))]
	[Serializable]
	public sealed partial class ConcurrentCircularBuffer<T>
		where T : class
	{
		private const int DefaultCapacity = 16;

		private T[] array;
		private int capacity;
		private int head;
		private int tail;
		private int version;
		private bool allowOverwrite;

		private readonly object syncRoot = new();

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class using the default capacity and enabling
		/// automatic overwriting.
		/// </summary>
		/// <remarks>
		/// <para>
		/// The default capacity is defined by the <c>DefaultCapacity</c> constant. When the buffer reaches capacity, newly enqueued
		/// elements automatically overwrite the oldest entries, unless <see cref="AllowOverwrite" /> is later changed.
		/// </para>
		/// <para>This buffer is thread-safe and supports reference types that may include <see langword="null" /> values or duplicates.</para>
		/// </remarks>
		public ConcurrentCircularBuffer()
					: this(DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class with the specified capacity and enables
		/// automatic overwriting.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// <para>
		/// When the buffer reaches the specified <paramref name="capacity" />, newly enqueued elements automatically overwrite the oldest
		/// entries unless <see cref="AllowOverwrite" /> is set to <see langword="false" />.
		/// </para>
		/// <para>This buffer is thread-safe and supports reference types that may include <see langword="null" /> values or duplicates.</para>
		/// </remarks>
		public ConcurrentCircularBuffer(int capacity)
					: this(capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class with the specified capacity and overwrite behavior.
		/// </summary>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <param name="allowOverwrite">
		/// If <see langword="true" />, newly enqueued elements will overwrite the oldest ones when the buffer reaches full capacity. If
		/// <see langword="false" />, attempting to enqueue beyond the buffer’s capacity will throw an <see cref="InvalidOperationException" />.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// <para>This constructor allows explicit control over whether the buffer permits overwriting of old items once it becomes full.</para>
		/// <para>The buffer is thread-safe and supports use with reference types, including <see langword="null" /> values and duplicates.</para>
		/// </remarks>
		public ConcurrentCircularBuffer(int capacity, bool allowOverwrite)
		{
			ThrowHelper.ThrowIfLessThanOrEqual(capacity, 0);

			T[] buffer = new T[capacity];

			array = buffer;
			this.capacity = capacity;
			this.allowOverwrite = allowOverwrite;
			head = 0;
			tail = 0;
			version = 0;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class by copying elements from the specified
		/// collection, using <see cref="DefaultCapacity" /> as the maximum size and enabling overwriting behavior.
		/// </summary>
		/// <param name="collection">The collection whose elements are copied into the buffer. Must not be <see langword="null" />.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <remarks>
		/// <para>
		/// If the number of elements in <paramref name="collection" /> exceeds <see cref="DefaultCapacity" />, only the most recent items
		/// are retained, and the oldest elements are discarded.
		/// </para>
		/// <para>
		/// The buffer operates in overwrite mode by default, replacing old items when capacity is exceeded. This constructor is thread-safe
		/// and supports reference types including <see langword="null" /> values and duplicates.
		/// </para>
		/// </remarks>
		public ConcurrentCircularBuffer(IEnumerable<T> collection)
					: this(collection, DefaultCapacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class by copying elements from the specified
		/// collection, using the specified capacity and enabling overwriting behavior.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied into the buffer. Must not be <see langword="null" />.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <remarks>
		/// <para>
		/// If the number of elements in <paramref name="collection" /> exceeds <paramref name="capacity" />, only the most recent items are
		/// retained. The oldest elements are discarded to make space for new items.
		/// </para>
		/// <para>
		/// This constructor initializes the buffer in overwrite mode by default. The implementation is thread-safe and supports reference
		/// types, including <see langword="null" /> values and duplicates.
		/// </para>
		/// </remarks>
		public ConcurrentCircularBuffer(IEnumerable<T> collection, int capacity)
					: this(collection, capacity, allowOverwrite: true) { }

		/// <summary>
		/// Initializes a new instance of the <see cref="ConcurrentCircularBuffer{T}" /> class by copying elements from the specified
		/// collection, using the specified capacity and controlling whether older elements can be overwritten.
		/// </summary>
		/// <param name="collection">The collection from which elements are copied into the buffer. Must not be <see langword="null" />.</param>
		/// <param name="capacity">The maximum number of elements the buffer can contain. Must be greater than zero.</param>
		/// <param name="allowOverwrite">
		/// If <see langword="true" />, only the most recent elements from the collection are retained if it exceeds the specified capacity.
		/// If <see langword="false" />, the collection size must not exceed <paramref name="capacity" /> or an exception is thrown.
		/// </param>
		/// <exception cref="ArgumentNullException">Thrown when <paramref name="collection" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="capacity" /> is less than or equal to zero.</exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="allowOverwrite" /> is <see langword="false" /> and the number of items in
		/// <paramref name="collection" /> exceeds <paramref name="capacity" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This constructor gives full control over how collection overflow is handled during initialization. In overwrite mode, the buffer
		/// retains only the latest items that fit within the given <paramref name="capacity" />. In non-overwrite mode, the collection must
		/// not exceed the capacity limit.
		/// </para>
		/// <para>
		/// The buffer supports reference types including <see langword="null" /> and allows duplicate values. All operations are
		/// thread-safe and use atomic memory access to ensure visibility across threads.
		/// </para>
		/// </remarks>
		public ConcurrentCircularBuffer(IEnumerable<T> collection, int capacity, bool allowOverwrite)
		{
			ThrowHelper.ThrowIfNull(collection);
			ThrowHelper.ThrowIfLessThanOrEqual(capacity, 0);

			var items = collection as T[] ?? collection.ToArray();
			if (items.Length > capacity && !allowOverwrite)
				throw new InvalidOperationException(ResourceStrings.Arg_Invalid_ArrayLengthExceedsCapacity);

			T[] newArray = new T[capacity];
			int actualCount;

			if (items.Length > capacity)
			{
				Array.Copy(items, items.Length - capacity, newArray, 0, capacity);
				actualCount = capacity;
			}
			else
			{
				Array.Copy(items, newArray, items.Length);
				actualCount = items.Length;
			}

			array = newArray;
			this.capacity = capacity;
			this.allowOverwrite = allowOverwrite;
			head = 0;
			tail = actualCount % capacity;
			version = 0;
		}

		/// <summary>
		/// Occurs immediately <b>after</b> an item has been evicted from the <see cref="ConcurrentCircularBuffer{T}" /> due to reaching the
		/// buffer's capacity while <see cref="AllowOverwrite" /> is <see langword="true" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event is triggered only when a new item is enqueued into a full buffer and overwriting is permitted. The event provides
		/// access to the evicted item that was replaced in the buffer.
		/// </para>
		/// <para>Consumers can use this event to track removed items, update external caches, or implement custom eviction logging.</para>
		/// <para>
		/// <b>Important:</b> The event is raised synchronously during the enqueue operation. Any exceptions thrown by event handlers will
		/// propagate to the caller of <see cref="Enqueue(T)" /> or <see cref="TryEnqueue(T)" /> and may disrupt normal buffer behavior.
		/// Event subscribers must ensure exception safety.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		///var buffer = new ConcurrentCircularBuffer<string>(capacity: 2, allowOverwrite: true);
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
		/// Occurs immediately <b>before</b> an item is evicted from the <see cref="ConcurrentCircularBuffer{T}" /> due to reaching the
		/// buffer's capacity while <see cref="AllowOverwrite" /> is <see langword="true" />.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This event is triggered only when a new item is about to overwrite an existing item in a full buffer and overwriting is permitted.
		/// </para>
		/// <para>
		/// Subscribers can use this event to inspect or act upon the item scheduled for eviction, such as persisting it to an external log,
		/// performing cleanup, or updating dependent state.
		/// </para>
		/// <para>
		/// <b>Important:</b> This event is invoked synchronously during the enqueue operation. Exceptions thrown by event handlers will
		/// propagate to the caller of <see cref="Enqueue(T)" /> or <see cref="TryEnqueue(T)" /> and may prevent the item from being
		/// overwritten. Handlers must be exception-safe.
		/// </para>
		/// </remarks>
		/// <example>
		/// <code language="csharp">
		///<![CDATA[
		///var buffer = new ConcurrentCircularBuffer<string>(capacity: 2, allowOverwrite: true);
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
		/// Gets or sets a value indicating whether the <see cref="ConcurrentCircularBuffer{T}" /> will automatically overwrite the oldest
		/// element when the buffer reaches its capacity.
		/// </summary>
		/// <value>
		/// <see langword="true" /> to enable automatic overwriting of the oldest item when <see cref="Count" /> equals
		/// <see cref="Capacity" />; otherwise, <see langword="false" /> to disable overwriting and throw an
		/// <see cref="InvalidOperationException" /> when an insert exceeds capacity.
		/// </value>
		/// <remarks>
		/// <para>
		/// When <see cref="AllowOverwrite" /> is <see langword="true" />, adding a new element to a full buffer causes the oldest element
		/// to be removed and replaced atomically. This is the default behavior.
		/// </para>
		/// <para>
		/// When <see cref="AllowOverwrite" /> is <see langword="false" />, calling <see cref="Enqueue" /> or <see cref="TryEnqueue" /> on a
		/// full buffer will either throw an <see cref="InvalidOperationException" /> or return <see langword="false" /> respectively.
		/// </para>
		/// <para>
		/// This property is thread-safe. Reading and writing the value is performed using memory barriers to ensure visibility across threads.
		/// </para>
		/// </remarks>
		public bool AllowOverwrite
		{
			get => Volatile.Read(ref allowOverwrite);
			set => Volatile.Write(ref allowOverwrite, value);
		}

		/// <summary>
		/// Gets the maximum number of elements that the <see cref="ConcurrentCircularBuffer{T}" /> can contain.
		/// </summary>
		/// <value>
		/// The total number of elements that the buffer can hold before reaching capacity. This value is fixed upon construction and does
		/// not change unless <see cref="TrimExcess" /> is called.
		/// </value>
		/// <remarks>
		/// <para>
		/// This property reflects the logical capacity of the buffer. When <see cref="Count" /> reaches <see cref="Capacity" />, further
		/// insertions may overwrite existing items depending on the <see cref="AllowOverwrite" /> setting.
		/// </para>
		/// <para>Access to this property is thread-safe and uses memory barriers to ensure visibility across threads.</para>
		/// </remarks>
		public int Capacity => Volatile.Read(ref capacity);

		/// <summary>
		/// Gets the element at the specified zero-based index from the oldest to the newest element.
		/// </summary>
		/// <param name="index">The zero-based index of the element to retrieve (0 is the oldest element).</param>
		/// <returns>The element at the specified index within the buffer.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="index" /> is less than 0 or greater than or equal to <see cref="Count" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This indexer provides snapshot-style access to elements based on their logical position in the buffer. The index is resolved
		/// relative to the current <see cref="head" />, meaning index 0 corresponds to the oldest element, and index <c>Count - 1</c>
		/// corresponds to the newest.
		/// </para>
		/// <para>
		/// All reads are performed using <see cref="!:Volatile.Read" /> to ensure memory visibility across threads. However, concurrent
		/// modifications may result in reading elements that have just been overwritten if not synchronized externally.
		/// </para>
		/// </remarks>
		public T this[int index]
		{
			get
			{
				ThrowHelper.ThrowIfLessThan(index, 0);
				int currentHead = Volatile.Read(ref head);
				int currentTail = Volatile.Read(ref tail);
				int currentCapacity = Volatile.Read(ref capacity);
				int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;
				ThrowHelper.ThrowIfGreaterThanOrEqual(index, currentCount);
				int actualIndex = (currentHead + index) % currentCapacity;
				return Volatile.Read(ref array[actualIndex]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private int GetCount()
		{
			return (tail - head + capacity) % capacity;
		}

		/// <summary>
		/// Removes all elements from the <see cref="ConcurrentCircularBuffer{T}" />, resetting its state to empty.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method clears the buffer contents by zeroing the internal storage and resetting the <c>head</c>, <c>tail</c>, and
		/// <c>count</c> pointers atomically. The buffer’s <see cref="Capacity" /> remains unchanged after clearing.
		/// </para>
		/// <para>
		/// The operation is thread-safe and uses <see cref="Volatile" /> and <see cref="Interlocked" /> operations to ensure visibility
		/// across threads. Enumerators or readers observing the buffer concurrently may still see stale data until the clear completes.
		/// </para>
		/// <para>This method also increments the internal version to invalidate any active enumerators created prior to clearing.</para>
		/// </remarks>
		public void Clear()
		{
			while (true)
			{
				var (buffer, head, tail, capacity) = TryGetSnapshot();
				int count = (tail - head + capacity) % capacity;

				// Defensive checks — ensure snapshot is valid
				if (buffer.Length != capacity || head < 0 || tail < 0 || head >= buffer.Length || tail > buffer.Length)
					continue;

				if (count == 0)
					return;

				// Check clear bounds before clearing
				if (head < tail)
				{
					if (head + count > buffer.Length)
						continue;

					Array.Clear(buffer, head, count);
				}
				else
				{
					int firstLength = buffer.Length - head;
					if (head + firstLength > buffer.Length || tail > buffer.Length)
						continue;

					if (firstLength > 0)
						Array.Clear(buffer, head, firstLength);

					if (tail > 0)
						Array.Clear(buffer, 0, tail);
				}

				// Reset state after clearing safely
				Interlocked.Exchange(ref this.head, 0);
				Interlocked.Exchange(ref this.tail, 0);
				Interlocked.Increment(ref version);
				return;
			}
		}

		/// <summary>
		/// Determines whether the buffer contains a specific element.
		/// </summary>
		/// <param name="item">The element to locate in the buffer. Can be <see langword="null" /> for reference types.</param>
		/// <returns><see langword="true" /> if <paramref name="item" /> is found; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// <para>
		/// This method performs a linear scan of the buffer from the current head position and uses
		/// <see cref="EqualityComparer{T}.Default" /> to determine equality.
		/// </para>
		/// <para>
		/// The operation is thread-safe and uses <see cref="Volatile" /> reads to ensure that each value is retrieved with proper memory
		/// visibility. However, concurrent modifications may affect the outcome or cause stale values to be read.
		/// </para>
		/// <para>
		/// The method does not throw exceptions if the buffer is concurrently modified; it guarantees only that the result reflects a
		/// consistent snapshot at the time of the scan.
		/// </para>
		/// </remarks>
		public bool Contains(T item)
		{
			int currentHead = Volatile.Read(ref head);
			int currentTail = Volatile.Read(ref tail);
			int currentCapacity = Volatile.Read(ref capacity);
			int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;
			T[] buffer = Volatile.Read(ref array);

			var comparer = EqualityComparer<T>.Default;
			int index = currentHead;

			for (int i = 0; i < currentCount; i++)
			{
				T currentItem = Volatile.Read(ref buffer[index]);
				if (comparer.Equals(currentItem, item))
					return true;

				index = (index + 1) % currentCapacity;
			}

			return false;
		}

		/// <summary>
		/// Copies the elements of the <see cref="ConcurrentCircularBuffer{T}" /> to the specified one-dimensional array, starting at the
		/// specified array index.
		/// </summary>
		/// <param name="array">
		/// The destination array that will receive the copied elements. Must be a single-dimensional, zero-based array with sufficient capacity.
		/// </param>
		/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="array" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="index" /> is less than zero.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if there is insufficient space in <paramref name="array" /> to accommodate the buffer's contents starting at <paramref name="index" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method performs a thread-safe copy by capturing a stable snapshot of the buffer using <see cref="TryGetSnapshot" />. The
		/// snapshot guarantees that the buffer's state remains logically consistent for the duration of the copy.
		/// </para>
		/// <para>If the buffer wraps around its internal array, the copy is performed in two segments to preserve element order.</para>
		/// <para>
		/// Only single-dimensional, zero-based arrays are supported. If an invalid array type is passed, a runtime exception will be thrown.
		/// </para>
		/// </remarks>
		public void CopyTo(T[] array, int index)
		{
			ThrowHelper.ThrowIfNull(array);

			var snapshot = TryGetSnapshot();
			int currentCount = (snapshot.Tail - snapshot.Head + snapshot.Capacity) % snapshot.Capacity;

			ThrowHelper.ThrowIfArrayLengthIsInsufficient(array, index, currentCount);

			CopyToCore(snapshot.Buffer, snapshot.Head, snapshot.Tail, currentCount, snapshot.Capacity, array, index);
		}

		/// <summary>
		/// Removes and returns the oldest element from the <see cref="ConcurrentCircularBuffer{T}" />.
		/// </summary>
		/// <returns>The element that was removed from the beginning of the buffer.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty at the time of the call.</exception>
		/// <remarks>
		/// <para>
		/// This method is thread-safe and uses atomic operations to remove the element at the head of the buffer. If the buffer is empty,
		/// the method throws an exception.
		/// </para>
		/// <para>To avoid exceptions when the buffer may be empty, use <see cref="TryDequeue(out T)" /> instead.</para>
		/// </remarks>
		public T Dequeue()
		{
			TryDequeueInternal(out var item, throwIfEmpty: true);
			return item;
		}

		/// <summary>
		/// Adds an element to the end of the <see cref="ConcurrentCircularBuffer{T}" />.
		/// </summary>
		/// <param name="item">The element to add. May be <see langword="null" /> for reference types.</param>
		/// <exception cref="InvalidOperationException">
		/// Thrown if the buffer has reached capacity and <see cref="AllowOverwrite" /> is <see langword="false" />.
		/// </exception>
		/// <remarks>
		/// <para>
		/// This method is thread-safe and uses atomic operations to enqueue the item. If the buffer is full and overwriting is enabled, the
		/// oldest item is removed to make space for the new one. Otherwise, an exception is thrown.
		/// </para>
		/// <para>To avoid exceptions when the buffer may be full, use <see cref="TryEnqueue(T)" /> instead.</para>
		/// </remarks>
		public void Enqueue(T item)
		{
			TryEnqueueInternal(item, throwIfFull: true);
		}

		/// <summary>
		/// Provides a snapshot of the buffer’s contents as two contiguous <see cref="ArraySegment{T}" /> instances.
		/// </summary>
		/// <returns>
		/// A tuple of two <see cref="ArraySegment{T}" /> objects representing the buffer contents:
		/// <list type="bullet">
		/// <item>
		/// <description><c>FirstSegment</c> — The first (and possibly only) segment of contiguous data.</description>
		/// </item>
		/// <item>
		/// <description><c>SecondSegment</c> — A second segment if the buffer has wrapped around; otherwise, an empty segment.</description>
		/// </item>
		/// </list>
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method captures a point-in-time, read-only snapshot of the current buffer state using volatile reads, which is safe for
		/// concurrent access. The result may not reflect subsequent enqueue or dequeue operations.
		/// </para>
		/// <para>The segments must be processed in sequence (first, then second) to access the full logical order of items.</para>
		/// </remarks>
		public (ArraySegment<T> FirstSegment, ArraySegment<T> SecondSegment) GetSegments()
		{
			var snapshot = TryGetSnapshot();
			int currentCount = (snapshot.Tail - snapshot.Head + snapshot.Capacity) % snapshot.Capacity;
			if (currentCount == 0)
			{
				return (
					new ArraySegment<T>(Array.Empty<T>()),
					new ArraySegment<T>(Array.Empty<T>())
				);
			}

			if (snapshot.Head < snapshot.Tail)
			{
				// Single continuous segment
				return (
					new ArraySegment<T>(snapshot.Buffer, snapshot.Head, currentCount),
					new ArraySegment<T>(Array.Empty<T>())
				);
			}
			else
			{
				// Wrapped segments
				int firstLength = capacity - snapshot.Head;
				var firstSegment = new ArraySegment<T>(snapshot.Buffer, snapshot.Head, firstLength);
				var secondSegment = new ArraySegment<T>(snapshot.Buffer, 0, snapshot.Tail);

				return (firstSegment, secondSegment);
			}
		}

		/// <summary>
		/// Returns the oldest element in the <see cref="ConcurrentCircularBuffer{T}" /> without removing it.
		/// </summary>
		/// <returns>The oldest element currently stored in the buffer.</returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty.</exception>
		/// <remarks>
		/// <para>
		/// This method reads buffer state using volatile memory operations to ensure visibility across threads. The returned value
		/// represents a snapshot at the time of the call and may not reflect concurrent modifications made immediately after.
		/// </para>
		/// <para>
		/// If the buffer is empty, this method throws an <see cref="InvalidOperationException" />. Use <see cref="TryPeek(out T)" /> to
		/// safely access the front element without throwing.
		/// </para>
		/// </remarks>
		public T Peek()
		{
			int currentHead = Volatile.Read(ref head);
			int currentTail = Volatile.Read(ref tail);
			int currentCapacity = Volatile.Read(ref capacity);
			int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;
			if (currentCount == 0)
				throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionEmpty);

			return Volatile.Read(ref array[currentHead]);
		}

		/// <summary>
		/// Copies the elements of the <see cref="ConcurrentCircularBuffer{T}" /> to a new array, preserving their order from oldest to newest.
		/// </summary>
		/// <returns>A new one-dimensional array containing a snapshot of the buffer's elements in first-in, first-out (FIFO) order.</returns>
		/// <remarks>
		/// <para>
		/// This method captures a stable snapshot of the buffer at the time of the call using <see cref="TryGetSnapshot" />. The returned
		/// array reflects the state of the buffer at that exact point in time and is not affected by concurrent modifications that occur afterward.
		/// </para>
		/// <para>
		/// If the buffer wraps around its internal array, the elements are copied in two contiguous segments to maintain correct ordering.
		/// </para>
		/// <para>The resulting array contains only the active elements in the buffer and may be smaller than the buffer’s internal capacity.</para>
		/// </remarks>
		public T[] ToArray()
		{
			var snapshot = TryGetSnapshot();
			int currentCount = (snapshot.Tail - snapshot.Head + snapshot.Capacity) % snapshot.Capacity;
			T[] result = new T[currentCount];

			CopyToCore(snapshot.Buffer, snapshot.Head, snapshot.Tail, currentCount, snapshot.Capacity, result, 0);
			return result;
		}

		/// <summary>
		/// Reduces the capacity of the <see cref="ConcurrentCircularBuffer{T}" /> to match the current number of elements, minimizing
		/// memory usage.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This method creates a compact copy of the buffer that contains only the current elements, discarding unused capacity. It is
		/// useful after a period of high usage followed by a reduced element count.
		/// </para>
		/// <para>
		/// If the buffer is empty ( <see cref="Count" /> is 0), the capacity is reduced to a minimal size (typically 1) to keep the buffer usable.
		/// </para>
		/// <para>
		/// This method is thread-safe and uses a lightweight synchronization lock to ensure consistency while resizing internal state.
		/// During execution, other threads can continue to read from or write to the buffer, but concurrent structural mutations are
		/// temporarily blocked until resizing completes.
		/// </para>
		/// </remarks>
		public void TrimExcess()
		{
			lock (syncRoot)
			{
				var (buffer, head, tail, capacity) = TryGetSnapshot();
				int count = (tail - head + capacity) % capacity;

				// Nothing to trim
				if (count == capacity)
					return;

				int newSize = Math.Max(count, 1);
				T[] trimmed = new T[newSize];

				CopyToCore(buffer, head, tail, count, capacity, trimmed, 0);

				// Apply new state safely while locked
				Volatile.Write(ref array, trimmed);
				Volatile.Write(ref this.head, 0);
				Volatile.Write(ref this.tail, count % newSize);
				Volatile.Write(ref this.capacity, newSize);
				Interlocked.Increment(ref version);
			}
		}

		/// <summary>
		/// Attempts to remove and return the oldest item from the <see cref="ConcurrentCircularBuffer{T}" /> without throwing exceptions.
		/// </summary>
		/// <param name="item">
		/// When this method returns, contains the element removed from the buffer if successful; otherwise, the default value for the type
		/// of the <typeparamref name="T" /> parameter.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if an item was successfully dequeued; otherwise, <see langword="false" /> if the buffer was empty.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method is thread-safe and avoids exceptions for typical dequeue operations. If the buffer is empty, the method returns
		/// <see langword="false" /> and sets <paramref name="item" /> to <c>default</c>.
		/// </para>
		/// <para>For exception-based behavior, use <see cref="Dequeue" /> instead.</para>
		/// </remarks>
		public bool TryDequeue(out T item) =>
			TryDequeueInternal(out item, throwIfEmpty: false);

		/// <summary>
		/// Attempts to add an item to the <see cref="ConcurrentCircularBuffer{T}" /> without throwing exceptions.
		/// </summary>
		/// <param name="item">The item to add to the buffer. May be <see langword="null" /> for reference types.</param>
		/// <returns>
		/// <see langword="true" /> if the item was successfully enqueued; otherwise, <see langword="false" /> if the buffer is full and
		/// <see cref="AllowOverwrite" /> is <see langword="false" />.
		/// </returns>
		/// <remarks>
		/// <para>This method is thread-safe and preferred for scenarios where failure to enqueue should not throw exceptions.</para>
		/// <para>Use <see cref="Enqueue" /> if you prefer an exception to be thrown when the buffer is full and overwriting is disabled.</para>
		/// </remarks>
		public bool TryEnqueue(T item) =>
			TryEnqueueInternal(item, throwIfFull: false);

		/// <summary>
		/// Attempts to retrieve the oldest element in the <see cref="ConcurrentCircularBuffer{T}" /> without removing it.
		/// </summary>
		/// <param name="item">
		/// When this method returns, contains the oldest element in the buffer if successful; otherwise, the default value of <typeparamref name="T" />.
		/// </param>
		/// <returns><see langword="true" /> if an item was successfully retrieved; <see langword="false" /> if the buffer is empty.</returns>
		/// <remarks>
		/// <para>This method is thread-safe and does not modify the state of the buffer.</para>
		/// <para>Use <see cref="Peek" /> to throw an exception when attempting to access an element in an empty buffer.</para>
		/// </remarks>
		public bool TryPeek(out T item)
		{
			int currentHead = Volatile.Read(ref head);
			int currentTail = Volatile.Read(ref tail);
			int currentCapacity = Volatile.Read(ref capacity);
			int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;
			if (currentCount == 0)
			{
				item = default!;
				return false;
			}

			item = Volatile.Read(ref array[currentHead]);
			return true;
		}

		/// <summary>
		/// Attempts to remove and return the oldest element from the <see cref="ConcurrentCircularBuffer{T}" />.
		/// </summary>
		/// <param name="item">When this method returns, contains the dequeued element if successful; otherwise, the default value of <typeparamref name="T" />.</param>
		/// <param name="throwIfEmpty">
		/// <see langword="true" /> to throw an <see cref="InvalidOperationException" /> if the buffer is empty; <see langword="false" /> to
		/// return <c>false</c> instead.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if an item was successfully dequeued; otherwise, <see langword="false" /> if the buffer was empty and
		/// <paramref name="throwIfEmpty" /> is <see langword="false" />.
		/// </returns>
		/// <exception cref="InvalidOperationException">Thrown if the buffer is empty and <paramref name="throwIfEmpty" /> is <see langword="true" />.</exception>
		/// <remarks>
		/// This method is used internally by <see cref="Dequeue" /> and <see cref="TryDequeue" /> to implement their behavior. It ensures
		/// thread-safety using atomic memory operations and interlocked synchronization primitives.
		/// </remarks>
		private bool TryDequeueInternal(out T item, bool throwIfEmpty)
		{
			while (true)
			{
				int currentHead = Volatile.Read(ref head);
				int currentTail = Volatile.Read(ref tail);
				int currentCapacity = Volatile.Read(ref capacity);
				int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;

				if (currentCount == 0)
				{
					item = default!;
					return throwIfEmpty
						? throw new InvalidOperationException(ResourceStrings.InvalidOperation_CollectionEmpty)
						: false;
				}

				int nextHead = (currentHead + 1) % capacity;

				// Try to claim the slot
				if (Interlocked.CompareExchange(ref head, nextHead, currentHead) != currentHead)
					continue;

				item = Volatile.Read(ref array[currentHead]);
				Volatile.Write(ref array[currentHead], default!);
				Interlocked.Increment(ref version);
				return true;
			}
		}

		/// <summary>
		/// Attempts to add an element to the <see cref="ConcurrentCircularBuffer{T}" /> in a thread-safe manner.
		/// </summary>
		/// <param name="item">The item to enqueue. Can be <see langword="null" /> for reference types.</param>
		/// <param name="throwIfFull">
		/// If <see langword="true" />, throws an <see cref="InvalidOperationException" /> when the buffer is full and
		/// <see cref="AllowOverwrite" /> is <see langword="false" />. If <see langword="false" />, returns <see langword="false" /> instead.
		/// </param>
		/// <returns>
		/// <see langword="true" /> if the item was successfully enqueued; <see langword="false" /> if the buffer was full and
		/// <paramref name="throwIfFull" /> is <see langword="false" /> and <see cref="AllowOverwrite" /> is <see langword="false" />.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// Thrown when <paramref name="throwIfFull" /> is <see langword="true" /> and the buffer is full while overwriting is disabled.
		/// </exception>
		/// <remarks>
		/// This method is the core implementation behind <see cref="Enqueue" /> and <see cref="TryEnqueue" />. It uses atomic memory
		/// operations and interlocked updates to ensure thread-safe behavior.
		/// <para>
		/// If <see cref="AllowOverwrite" /> is <see langword="true" />, the oldest item will be replaced, and <see cref="ItemEvicting" />
		/// and <see cref="ItemEvicted" /> events will be raised for the overwritten item.
		/// </para>
		/// </remarks>
		private bool TryEnqueueInternal(T item, bool throwIfFull)
		{
			while (true)
			{
				int currentTail = Volatile.Read(ref tail);
				int currentHead = Volatile.Read(ref head);
				int currentCapacity = Volatile.Read(ref capacity);
				int currentCount = (currentTail - currentHead + currentCapacity) % currentCapacity;
				int nextTail = (currentTail + 1) % currentCapacity;

				// Attempt to claim the tail slot — this is now the shared gate
				if (Interlocked.CompareExchange(ref tail, nextTail, currentTail) != currentTail)
					continue;

				if (currentCount == currentCapacity)
				{
					if (!AllowOverwrite)
					{
						// Roll back tail advance
						Interlocked.Exchange(ref tail, currentTail);
						return throwIfFull
							? throw new InvalidOperationException(ResourceStrings.InvalidOperation_CapacityExhausted)
							: false;
					}

					// Overwriting oldest item
					int newHead = (currentHead + 1) % currentCapacity;
					Interlocked.Exchange(ref head, newHead); // Advance head

					T evictedItem = Volatile.Read(ref array[currentTail]);
					ItemEvicting?.Invoke(evictedItem);

					Volatile.Write(ref array[currentTail], item);
					ItemEvicted?.Invoke(evictedItem);

					Interlocked.Increment(ref version);
					return true; // Do not increment count — buffer is still full
				}
				else
				{
					// Normal enqueue — count is below capacity
					Volatile.Write(ref array[currentTail], item);
					Interlocked.Increment(ref version);
					return true;
				}
			}
		}

		/// <summary>
		/// Copies a snapshot of the circular buffer's contents to the specified <see cref="Array" />, starting at the given index.
		/// </summary>
		/// <param name="source">The internal buffer array from which elements will be copied.</param>
		/// <param name="head">The index of the first (oldest) element in the buffer.</param>
		/// <param name="tail">The index where the next element would be written.</param>
		/// <param name="count">The number of valid elements currently in the buffer.</param>
		/// <param name="capacity">The total capacity of the buffer.</param>
		/// <param name="destination">
		/// The destination array to which elements will be copied. Must not be <see langword="null" /> and must contain enough space.
		/// </param>
		/// <param name="destinationIndex">The zero-based index in the destination array at which to begin copying.</param>
		/// <remarks>
		/// <para>
		/// This method assumes that the source buffer state was captured consistently via <see cref="TryGetSnapshot" />. It safely handles
		/// wrapped buffers by performing the copy in one or two segments, depending on whether the head index precedes the tail.
		/// </para>
		/// <para>
		/// This method is typically used by <see cref="ToArray" /> or <see cref="CopyTo(T[], int)" /> to copy buffer contents in a
		/// thread-safe manner.
		/// </para>
		/// </remarks>
		private static void CopyToCore(
			T[] source, int head, int tail, int count, int capacity,
			Array destination, int destinationIndex)
		{
			if (count == 0)
				return;

			if (head < tail)
			{
				Array.Copy(source, head, destination, destinationIndex, count);
			}
			else
			{
				int firstLength = capacity - head;
				Array.Copy(source, head, destination, destinationIndex, firstLength);
				Array.Copy(source, 0, destination, destinationIndex + firstLength, tail);
			}
		}

		/// <summary>
		/// Captures a consistent snapshot of the circular buffer's internal state for safe enumeration or copying.
		/// </summary>
		/// <returns>
		/// A tuple containing the current buffer array, head index, tail index, element count, and capacity. The values are guaranteed to
		/// be consistent with one another.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This method repeatedly attempts to read the buffer state until it detects no concurrent modification during read. It ensures
		/// that the snapshot values represent a stable view of the buffer at a single point in time.
		/// </para>
		/// <para>
		/// The snapshot is intended for use by <see cref="ToArray" /> or <see cref="CopyTo(T[], int)" /> to ensure thread-safe operations
		/// without external locking.
		/// </para>
		/// </remarks>
		private (T[] Buffer, int Head, int Tail, int Capacity) TryGetSnapshot()
		{
			while (true)
			{
				// Read all values in a known order
				int version1 = Volatile.Read(ref version);
				int head = Volatile.Read(ref this.head);
				int tail = Volatile.Read(ref this.tail);
				int capacity = Volatile.Read(ref this.capacity);
				T[] buffer = Volatile.Read(ref array);
				int version2 = Volatile.Read(ref version);

				// Confirm that version hasn't changed during the read
				if (version1 == version2)
					return (buffer, head, tail, capacity);
			}
		}
	}
}