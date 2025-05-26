using Bodu.Extensions;
using System;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Represents the abstract base class for hash algorithms that require a secret key and process data in fixed-size blocks.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This class extends <see cref="BlockHashAlgorithm" /> to provide key-handling logic required by keyed hash algorithms such as HMAC or
	/// SipHash. It ensures the key is securely managed and disposed using defensive copying and memory clearing.
	/// </para>
	/// <para>
	/// Derived classes must define how the key is integrated into the hashing process. The <see cref="Key" /> property provides controlled
	/// access to the key material and prevents external mutation by using defensive copying.
	/// </para>
	/// </remarks>
	public abstract class KeyedBlockHashAlgorithm<T>
		: BlockHashAlgorithm<T>
		where T : KeyedBlockHashAlgorithm<T>, new()
	{
		private bool _disposed = false;

		/// <summary>
		/// Internal storage for the key used in the algorithm. Always set using a defensive copy.
		/// </summary>
		protected byte[] KeyValue = null!;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeyedBlockHashAlgorithm" /> class with a specified block size.
		/// </summary>
		/// <param name="blockSize">The fixed block size (in bytes) for the algorithm.</param>
		protected KeyedBlockHashAlgorithm(int blockSize)
			: base(blockSize)
		{ }

		/// <summary>
		/// Gets or sets the secret key used by the hash algorithm.
		/// </summary>
		/// <remarks>
		/// <para>
		/// Access to the key is protected by returning a copy rather than the internal array. Setting the key also creates a copy to ensure
		/// internal integrity and prevent external modifications.
		/// </para>
		/// <para>Derived implementations should validate the key size in their setter overrides if specific key lengths are required.</para>
		/// </remarks>
		/// <exception cref="ArgumentNullException">Thrown if the key value is <see langword="null" />.</exception>
		public virtual byte[] Key
		{
			get => KeyValue.ToArray(); // Return a copy to maintain immutability

			set
			{
				if (value is null)
					throw new ArgumentNullException(nameof(value));

				// Defensive copy ensures external references cannot mutate internal key
				KeyValue = value.ToArray();
			}
		}

		/// <inheritdoc />
		/// <summary>
		/// Releases the unmanaged resources used by the algorithm and clears the key from memory.
		/// </summary>
		/// <param name="disposing">
		/// <see langword="true" /> to release both managed and unmanaged resources; <see langword="false" /> to release only unmanaged resources.
		/// </param>
		/// <remarks>
		/// This override ensures the key is securely cleared using <see cref="CryptographicOperations.ZeroMemory" /> before disposal.
		/// </remarks>
		protected override void Dispose(bool disposing)
		{
			if (_disposed) return;

			if (disposing)
			{
				// Zero out the key material to avoid leaking secrets in memory
				if (KeyValue is not null)
				{
					CryptoUtilities.ClearAndNullify(ref KeyValue!);
				}
			}

			_disposed = true;
			base.Dispose(disposing);
		}
	}
}