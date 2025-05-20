using System.Reflection;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	public abstract partial class HashAlgorithmTests<TTest, TAlgorithm, TVariant>
	{
		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.ComputeHash(byte[])" /> after disposal throws an <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Dispose_WhenComputeHashWithBufferCalledAfterDispose_ShouldThrowExactly()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				_ = algorithm.ComputeHash(Array.Empty<byte>());
			});
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.ComputeHash(byte[], int, int)" /> after disposal throws an <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Dispose_WhenComputeHashWithBufferRangeCalledAfterDispose_ShouldThrowExactly()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				_ = algorithm.ComputeHash(Array.Empty<byte>(), 0, 0);
			});
		}

		/// <summary>
		/// Verifies that calling <see cref="HashAlgorithm.ComputeHash(Stream)" /> after disposal throws an <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Dispose_WhenComputeHashWithStreamCalledAfterDispose_ShouldThrowExactly()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
				_ = algorithm.ComputeHash(Stream.Null));
		}

		/// <summary>
		/// Verifies that accessing the <see cref="HashAlgorithm.TransformBlock(byte[], int, int, byte[]?, int)" /> property after disposal
		/// throws an <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Dispose_WhenTransformBlockCalledAfterDispose_ShouldThrowExactly()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				_ = algorithm.TransformBlock(Array.Empty<byte>(), 0, 0, null, 0);
			});
		}

		/// <summary>
		/// Verifies that accessing the <see cref="HashAlgorithm.Hash" /> property after disposal throws an <see cref="ObjectDisposedException" />.
		/// </summary>
		[TestMethod]
		public void Dispose_WhenTransformFinalBlockCalledAfterDispose_ShouldThrowExactly()
		{
			var algorithm = this.CreateAlgorithm();
			algorithm.Dispose();

			Assert.ThrowsExactly<ObjectDisposedException>(() =>
			{
				_ = algorithm.TransformFinalBlock(Array.Empty<byte>(), 0, 0);
			});
		}

		/// <summary>
		/// Verifies that writable public properties on a hash algorithm throw an <see cref="ObjectDisposedException" /> when set after the
		/// algorithm instance has been disposed.
		/// </summary>
		/// <param name="property">The property to test for post-disposal access.</param>
		/// <remarks>
		/// This test uses reflection to reassign the property's current hashValue after calling <see cref="HashAlgorithm.Dispose" />. This
		/// ensures concrete <see cref="HashAlgorithm" /> implementations enforce correct disposal behavior.
		/// </remarks>
		[DataTestMethod]
		[DynamicData(nameof(GetWritableProperties), DynamicDataSourceType.Method)]
		public void Dispose_WhenAssigningProperty_ShouldThrowExactly(PropertyInfo property)
		{
			if (property is null)
				Assert.Inconclusive($"No writable properties were found for type {typeof(TAlgorithm).Name}.");

			using var algorithm = this.CreateAlgorithm();

			object? currentValue;
			try
			{
				currentValue = property.GetValue(algorithm);
			}
			catch
			{
				Assert.Inconclusive($"Property '{property.Name}' could not be read before disposal.");
				return;
			}

			algorithm.Dispose();

			try
			{
				property.SetValue(algorithm, currentValue);
				Assert.Fail($"Expected ObjectDisposedException when setting property '{property.Name}' after disposal.");
			}
			catch (TargetInvocationException tie) when (tie.InnerException is ObjectDisposedException)
			{
				// ✅ Expected: disposed object should not allow configuration
			}
			catch (Exception ex)
			{
				Assert.Fail($"Unexpected exception when setting property '{property.Name}' after disposal: {ex.GetType().Name} - {ex.Message}");
			}
		}
	}
}