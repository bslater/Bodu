﻿using System.Reflection;
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
			{
				Assert.Inconclusive($"Type '{typeof(TAlgorithm).Name}' has no writable properties — test passes by default.");
				return;
			}

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

		private static string[] IgnoreFieldNames => new[]
		{
			"HashSizeValue",
			"State",
			"_disposed",
			"disposed",
		};

		protected virtual IEnumerable<string> GetFieldsToExcludeFromDisposeValidation() =>
			Array.Empty<string>();

		/// <summary>
		/// Enumerates all instance fields in the algorithm and its base types to validate disposal state.
		/// </summary>
		public static IEnumerable<object[]> GetDisposableFields()
		{
			var instance = new TTest();
			var ignoreFields = IgnoreFieldNames
				.Union(instance.GetFieldsToExcludeFromDisposeValidation())
				.Distinct()
				.ToArray();

			Type type = typeof(TAlgorithm);
			BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

			var visited = new HashSet<string>(StringComparer.Ordinal);

			// Walk inheritance chain from most derived type up
			while (type != typeof(object) && type != null)
			{
				foreach (var field in type.GetFields(flags))
				{
					if (ignoreFields.Contains(field.Name) || field.IsStatic || field.FieldType.IsEnum || !visited.Add(field.Name)) continue; // skip ignored, static or duplicate names
					yield return new object[] { field };
				}

				type = type.BaseType!;
			}
		}

		/// <summary>
		/// Verifies that all fields of a disposed hash algorithm instance have been properly cleared or zeroed.
		/// </summary>
		/// <typeparam name="TAlgorithm">The hash algorithm type under test.</typeparam>
		/// <param name="field">The field to validate after disposal.</param>
		/// <summary>
		/// Verifies that a disposable algorithm properly zeroes or nullifies its internal fields after disposal.
		/// </summary>
		/// <param name="field">The field to inspect for zeroed or null state.</param>
		[DataTestMethod]
		[DynamicData(nameof(GetDisposableFields), DynamicDataSourceType.Method)]
		public void Dispose_WhenCalled_ShouldZeroPrivateField(FieldInfo field)
		{
			using TAlgorithm instance = CreateAlgorithm();
			instance.ComputeHash(Array.Empty<byte>());
			instance.Dispose();

			object? value = field.GetValue(instance);
			Type fieldType = field.FieldType;
			string label = $"Field '{field.DeclaringType},{field.Name}'";

			if (AssertPrimitiveValue(fieldType, value, label)) return;
			if (AssertPrimitiveArray(fieldType, value, label)) return;
			if (AssertMemorySpan(fieldType, value, label)) return;

			Assert.Inconclusive($"Unhandled field type '{fieldType}' for {label}.");
		}

		private static bool AssertPrimitiveValue(Type fieldType, object? value, string label)
		{
			switch (value)
			{
				case null:
					return true;

				case int i:
					Assert.AreEqual(0, i, $"{label} is not zero.");
					return true;

				case bool b:
					Assert.IsFalse(b, $"{label} is not false.");
					return true;

				case long l:
					Assert.AreEqual(0L, l, $"{label} is not zero.");
					return true;

				case uint ui:
					Assert.AreEqual(0U, ui, $"{label} is not zero.");
					return true;

				case ulong ul:
					Assert.AreEqual(0UL, ul, $"{label} is not zero.");
					return true;
			}
			return false;
		}

		private static bool AssertPrimitiveArray(Type fieldType, object? value, string label)
		{
			switch (value)
			{
				case byte[] byteArray:
					Assert.IsTrue(byteArray.All(b => b == 0), $"{label} contains non-zero bytes.");
					return true;

				case uint[] uintArray:
					Assert.IsTrue(uintArray.All(b => b == 0), $"{label} contains non-zero elements.");
					return true;

				case Array array when fieldType.IsArray && fieldType.GetElementType()?.IsPrimitive == true:
					Assert.IsNotNull(array, $"{label} is null.");
					object defaultValue = Activator.CreateInstance(fieldType.GetElementType()!)!;
					foreach (var item in array)
						Assert.AreEqual(defaultValue, item, $"{label} contains non-zero element '{item}'.");
					return true;
			}
			return false;
		}

		private static bool AssertMemorySpan(Type fieldType, object? value, string label)
		{
			if (fieldType.IsGenericType)
			{
				Type def = fieldType.GetGenericTypeDefinition();
				Type elementType = fieldType.GetGenericArguments()[0];

				if (elementType == typeof(byte))
				{
					if (def == typeof(Memory<>))
					{
						var memory = (Memory<byte>)value!;
						foreach (byte b in memory.Span)
						{
							Assert.AreEqual(0, b, $"{label} contains non-zero byte.");
						}
						return true;
					}
					if (def == typeof(ReadOnlyMemory<>))
					{
						var memory = (ReadOnlyMemory<byte>)value!;
						foreach (byte b in memory.Span)
						{
							Assert.AreEqual(0, b, $"{label} contains non-zero byte.");
						}
						return true;
					}
				}
			}
			return false;
		}
	}
}