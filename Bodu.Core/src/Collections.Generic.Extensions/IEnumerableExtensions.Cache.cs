// ---------------------------------------------------------------------------------------------------------------
// <copyright file="IEnumerable.Cache.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Threading;

namespace Bodu.Collections.Generic.Extensions
{
	public static partial class IEnumerableExtensions
	{
		/// <summary>
		/// Produces a sequence that lazily caches the elements of the source as it is iterated for the first time. Subsequent iteration
		/// requests return the cached elements.
		/// </summary>
		/// <typeparam name="T">The type of the elements in the source sequence.</typeparam>
		/// <param name="source">The sequence whose elements should be cached.</param>
		/// <returns>
		/// An <see cref="IEnumerable{T}" /> that caches the source's elements on first enumeration and returns cached results on all
		/// subsequent enumerations.
		/// </returns>
		/// <exception cref="ArgumentNullException"><paramref name="source" /> is <see langword="null" />.</exception>
		/// <remarks>
		/// This method uses deferred execution. The caching begins only when the resulting sequence is enumerated. If the source is already
		/// a collection or an existing cached sequence, no additional wrapping is performed.
		/// </remarks>
		public static IEnumerable<T> Cache<T>(this IEnumerable<T> source)
		{
			return source switch
			{
				null => throw new ArgumentNullException(nameof(source)),

				// return the sequence for types where we don't need to explicitly cache
				ICollection<T> => source,
				IReadOnlyCollection<T> => source,
				CacheEnumerable<T> => source,
				_ => new CacheEnumerable<T>(source)
			};
		}

		/// <summary>
		/// A sequence that caches its elements as they are enumerated, allowing subsequent replays without re-enumerating the source.
		/// </summary>
		/// <typeparam name="T">The type of elements in the cached sequence.</typeparam>
		/// <remarks>This type is used internally by <see cref="Cache{T}" /> and is thread-safe for concurrent enumeration.</remarks>
		private sealed class CacheEnumerable<T>
			: System.Collections.Generic.IEnumerable<T>
			, System.IDisposable
		{
			private readonly IEnumerable<T> _source;
			private volatile List<T>? _cache;
			private IEnumerator<T>? _enumerator;
			private volatile ExceptionDispatchInfo? _exception;
			private int _exceptionIndex = -1;
			private int _initializationState; // 0 = not initialized, 1 = initializing, 2 = initialized

			/// <summary>
			/// Initializes a new instance of the <see cref="CacheEnumerable{T}" /> class.
			/// </summary>
			/// <param name="source">The original sequence to cache during iteration.</param>
			public CacheEnumerable(IEnumerable<T> source)
			{
				_source = source ?? throw new ArgumentNullException(nameof(source));
			}

			/// <summary>
			/// Disposes internal state and resets cached items and the source enumerator.
			/// </summary>
			public void Dispose()
			{
				Interlocked.Exchange(ref _enumerator, null)?.Dispose();
				_cache = null;
				_exception = null;
				_exceptionIndex = -1;
				_initializationState = 0;
			}

			/// <inheritdoc />
			public IEnumerator<T> GetEnumerator() => new Enumerator(this);

			/// <inheritdoc />
			IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

			/// <summary>
			/// Ensures the cache is initialized and the source enumerator is safely acquired.
			/// </summary>
			private void EnsureInitialized()
			{
				// Fast path: already initialized
				if (Volatile.Read(ref _cache) != null)
					return;

				// Try to claim initialization responsibility
				if (Interlocked.CompareExchange(ref _initializationState, 1, 0) == 0)
				{
					try
					{
						_enumerator = _source.GetEnumerator(); // May throw
						_cache = new List<T>();
						Interlocked.Exchange(ref _initializationState, 2); // Mark as complete
					}
					catch (Exception ex)
					{
						_exception = ExceptionDispatchInfo.Capture(ex);
						Interlocked.Exchange(ref _initializationState, 2); // Mark as complete (with failure)
						throw;
					}
				}
				else
				{
					// Spin until initialization completes
					SpinWait spin = default;
					while (Volatile.Read(ref _initializationState) != 2)
						spin.SpinOnce();

					// Re-throw captured initialization exception if any
					_exception?.Throw();
				}
			}

			/// <summary>
			/// Enumerator that reads items from the cached buffer or populates new items from the source.
			/// </summary>
			private sealed class Enumerator
				: System.Collections.Generic.IEnumerator<T>
			{
				private readonly CacheEnumerable<T> _parent;
				private int _index;

				/// <summary>
				/// Initializes a new instance of the <see cref="Enumerator" /> class.
				/// </summary>
				/// <param name="parent">The parent <see cref="CacheEnumerable{T}" /> instance.</param>
				public Enumerator(CacheEnumerable<T> parent)
				{
					_parent = parent;
					_index = -1;
					_parent.EnsureInitialized();
				}

				/// <inheritdoc />
				public T Current
				{
					get
					{
						var cache = _parent._cache ?? throw new ObjectDisposedException(nameof(CacheEnumerable<T>));
						if (_index < 0 || _index >= cache.Count)
							throw new InvalidOperationException(ResourceStrings.InvalidOperation_EnumeratorNotOnElement);

						return cache[_index];
					}
				}

				/// <inheritdoc />
				object IEnumerator.Current => Current!;

				/// <summary>
				/// Disposes the enumerator. No-op for this implementation.
				/// </summary>
				public void Dispose()
				{ }

				/// <inheritdoc />
				public bool MoveNext()
				{
					_index++;

					var cache = _parent._cache ?? throw new ObjectDisposedException(nameof(CacheEnumerable<T>));

					// Return from cache if already populated
					if (_index < cache.Count)
						return true;

					// Rethrow any prior exception at this index
					if (_parent._exceptionIndex == _index)
						_parent._exception?.Throw();

					var enumerator = _parent._enumerator;
					if (enumerator == null)
						return false;

					// Attempt to take exclusive access to advance enumerator
					if (!Monitor.TryEnter(enumerator, 0))
					{
						// Another thread is currently fetching the next item; spin and retry
						SpinWait spin = default;
						while (true)
						{
							if (_index < (_parent._cache?.Count ?? 0))
								return true;

							if (_parent._enumerator == null)
								return false;

							spin.SpinOnce();
						}
					}

					try
					{
						if (enumerator.MoveNext())
						{
							cache.Add(enumerator.Current); // Append to shared cache
							return true;
						}

						// End of sequence
						enumerator.Dispose();
						_parent._enumerator = null;
						return false;
					}
					catch (Exception ex)
					{
						_parent._exception = ExceptionDispatchInfo.Capture(ex);
						_parent._exceptionIndex = _index;
						enumerator.Dispose();
						_parent._enumerator = null;
						throw;
					}
					finally
					{
						Monitor.Exit(enumerator);
					}
				}

				/// <summary>
				/// Not supported for caching enumerators.
				/// </summary>
				/// <exception cref="NotSupportedException">Always thrown.</exception>
				public void Reset() =>
					throw new NotSupportedException();
			}
		}
	}
}