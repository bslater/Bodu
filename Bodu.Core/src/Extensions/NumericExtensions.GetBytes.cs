// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="NumericExtensions.GetBytes.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class NumericExtensions
	{
		private static bool IsNumericType(Type type)
		{
			return type == typeof(byte) ||
				   type == typeof(sbyte) ||
				   type == typeof(short) ||
				   type == typeof(ushort) ||
				   type == typeof(int) ||
				   type == typeof(uint) ||
				   type == typeof(long) ||
				   type == typeof(ulong) ||
				   type == typeof(float) ||
				   type == typeof(double) ||
				   type == typeof(decimal);
		}

		/// <summary>
		/// Converts the specified numeric value to a byte array. Uses bitwise shifts for numeric types (e.g., <see cref="int" />,
		/// <see cref="long" />, <see cref="ushort" />, etc.) to generate the byte representation, and built-in methods like See
		/// <see cref="BitConverter" /> for methods that convert numeric types (e.g., <see cref="float" />, <see cref="double" />) to byte arrays.
		/// </summary>
		/// <typeparam name="T">The type of the value to convert. This should be a numeric type or a type supported by <see cref="BitConverter" />.</typeparam>
		/// <param name="value">
		/// The value to convert to a byte array. The value must be of a supported numeric type, such as <see cref="byte" />,
		/// <see cref="short" />, <see cref="int" />, <see cref="long" />, <see cref="ushort" />, <see cref="uint" />, <see cref="ulong" />,
		/// <see cref="float" />, or <see cref="double" />.
		/// </param>
		/// <param name="asBigEndian">
		/// If <see langword="true" />, forces the byte array to be in big-endian order; otherwise, <see langword="false" />, the array will
		/// be in the system’s native endianness.
		/// </param>
		/// <returns>A byte array representing the value in the specified or native endianness.</returns>
		/// <exception cref="InvalidOperationException">Thrown if <typeparamref name="T" /> is not a supported type (numeric or BitConverter-supported).</exception>
		public static byte[] GetBytes<T>(this T value, bool asBigEndian = false)
		{
			return (asBigEndian && BitConverter.IsLittleEndian || !asBigEndian && !BitConverter.IsLittleEndian)
				? value switch
				{
					byte byteValue => new byte[] { byteValue },

					short shortValue => new byte[] { (byte)(shortValue >> 8), (byte)shortValue },

					ushort ushortValue => new byte[] { (byte)(ushortValue >> 8), (byte)ushortValue },
					int intValue => new byte[] { (byte)(intValue >> 24), (byte)(intValue >> 16), (byte)(intValue >> 8), (byte)intValue },

					uint uintValue => new byte[] { (byte)(uintValue >> 24), (byte)(uintValue >> 16), (byte)(uintValue >> 8), (byte)uintValue },

					long longValue => new byte[] { (byte)(longValue >> 56), (byte)(longValue >> 48), (byte)(longValue >> 40), (byte)(longValue >> 32),
												   (byte)(longValue >> 24), (byte)(longValue >> 16), (byte)(longValue >> 8), (byte)longValue },

					ulong ulongValue => new byte[] { (byte)(ulongValue >> 56), (byte)(ulongValue >> 48), (byte)(ulongValue >> 40), (byte)(ulongValue >> 32),
													 (byte)(ulongValue >> 24), (byte)(ulongValue >> 16), (byte)(ulongValue >> 8), (byte)ulongValue },

					float floatValue => BitConverter.GetBytes(floatValue),
					double doubleValue => BitConverter.GetBytes(doubleValue),
					_ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}")
				}
				: value switch
				{
					byte byteValue => new byte[] { byteValue },
					short shortValue => new byte[] { (byte)shortValue, (byte)(shortValue >> 8) },

					ushort ushortValue => new byte[] { (byte)ushortValue, (byte)(ushortValue >> 8) },

					int intValue => new byte[] { (byte)intValue, (byte)(intValue >> 8), (byte)(intValue >> 16), (byte)(intValue >> 24) },
					uint uintValue => new byte[] { (byte)uintValue, (byte)(uintValue >> 8), (byte)(uintValue >> 16), (byte)(uintValue >> 24) },

					long longValue => new byte[] { (byte)longValue, (byte)(longValue >> 8), (byte)(longValue >> 16), (byte)(longValue >> 24),
												   (byte)(longValue >> 32), (byte)(longValue >> 40), (byte)(longValue >> 48), (byte)(longValue >> 56) },

					ulong ulongValue => new byte[] { (byte)ulongValue, (byte)(ulongValue >> 8), (byte)(ulongValue >> 16), (byte)(ulongValue >> 24),
													 (byte)(ulongValue >> 32), (byte)(ulongValue >> 40), (byte)(ulongValue >> 48), (byte)(ulongValue >> 56) },

					float floatValue => BitConverter.GetBytes(floatValue),
					double doubleValue => BitConverter.GetBytes(doubleValue),
					_ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}")
				};
		}
	}
}