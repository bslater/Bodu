// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelper.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------
using System.Runtime.CompilerServices;

namespace Bodu
{
	/// <summary>
	/// Provides centralized guard clause methods for argument validation using resource-based exception messages.
	/// </summary>
	public static class ThrowHelper
	{
		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the specified array is not single-dimensional.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <param name="paramName">The name of the parameter, automatically inferred.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="array" /> has a rank other than 1.
		/// Message: "Only single dimension arrays are supported here."
		/// </exception>
		/// <remarks>Checks that <c>array.Rank == 1</c>.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayIsNotSingleDimension(
			Array array,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array.Rank != 1)
				throw new ArgumentException(ResourceStrings.Rank_MultiDimensionArrayNotSupported, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array does not have a zero lower bound.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <param name="paramName">The name of the array parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>array.GetLowerBound(0) != 0</c>.
		/// Message: "The lower bound of target array must be zero."
		/// </exception>
		/// <remarks>Checks that the array is zero-based (i.e., index starts at 0).</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayIsNotZeroBased(
			Array array,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayNonZeroLowerBound, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array is too short for the given index and required length.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <param name="index">The starting index.</param>
		/// <param name="requiredLength">The required length from index onward.</param>
		/// <param name="paramName">The name of the array parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>array.Length - index &lt; requiredLength</c>.
		/// Message: "Array is too short. Required minimum is {0} from a specified index."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthIsInsufficient(
			Array array, int index, int requiredLength,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_ArrayTooShort, requiredLength), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array has zero length.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <param name="paramName">The name of the array parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>array.Length == 0</c>.
		/// Message: "The provided array has zero length. Length must be greater than zero."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthIsZero(
			Array array,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array.Length == 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayIsZeroLength, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array length is not a positive multiple of a given divisor.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <param name="divisor">The required positive divisor.</param>
		/// <param name="paramName">The name of the array parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the array length is 0 or not divisible by <paramref name="divisor" />.
		/// Message: "Length of the array must be a multiple of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthNotPositiveMultipleOf(
			Array array,
			int divisor,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array.Length == 0 || array.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_ArrayLengthMultipleOf, divisor), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the remaining length of the span from the given index is less than required.
		/// </summary>
		/// <typeparam name="T">The element type of the span.</typeparam>
		/// <param name="span">The span to check.</param>
		/// <param name="index">The index from which to measure the remaining length.</param>
		/// <param name="requiredLength">The required number of elements.</param>
		/// <param name="paramName">The name of the span parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>span.Length - index &lt; requiredLength</c>.
		/// Message: "Span is too short. Required minimum is {0} from a specified index."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSpanLengthIsInsufficient<T>(
			Span<T> span, int index, int requiredLength,
			[CallerArgumentExpression(nameof(span))] string? paramName = null)
		{
			if (span.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanTooShort, requiredLength), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the span length is not a positive multiple of a given divisor.
		/// </summary>
		/// <typeparam name="T">The element type of the span.</typeparam>
		/// <param name="span">The span to check.</param>
		/// <param name="divisor">The divisor that span length must be a multiple of.</param>
		/// <param name="func">A factory for a custom exception (unused in default implementation).</param>
		/// <param name="paramName">The name of the span parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>span.Length == 0 || span.Length % divisor != 0</c>.
		/// Message: "Length of the Span must be a multiple of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSpanLengthNotPositiveMultipleOf<T>(
			ReadOnlySpan<T> span,
			int divisor,
			Func<string, Exception>? func = null,
			[CallerArgumentExpression(nameof(span))] string? paramName = null)
		{
			if (span.Length == 0 || span.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanLengthMultipleOf, divisor), paramName);
		}

		/// <summary>
		/// Throws an exception if the specified <paramref name="index" /> and <paramref name="count" /> define a segment that exceeds the
		/// bounds of the <paramref name="array" />.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <param name="index">The zero-based starting index within the array.</param>
		/// <param name="count">The number of elements to access from <paramref name="index" />.</param>
		/// <param name="paramArrayName">The name of the array parameter to include in exception messages.</param>
		/// <param name="paramIndexName">The name of the index parameter to include in exception messages.</param>
		/// <param name="paramCountName">The name of the count parameter to include in exception messages.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="index" /> is negative or greater than <c>array.Length</c>, or when <paramref name="count" /> is
		/// negative or greater than <c>array.Length</c>.
		/// Message: "The value must zero or positive, and less than the number of elements in the array."
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown when the sum of <paramref name="index" /> and <paramref name="count" /> exceeds <c>array.Length</c>.
		/// Message: "The sum of index and count exceeds the number of elements in the array."
		/// </exception>
		/// <remarks>
		/// This method validates that the segment of the array specified by <paramref name="index" /> and <paramref name="count" /> is
		/// within valid bounds. It ensures that no out-of-range access occurs when operating on a subrange.
		/// </remarks>
		public static void ThrowIfArrayOffsetOrCountInvalid(
			Array array,
			int index,
			int count,
			[CallerArgumentExpression(nameof(array))] string? paramArrayName = null,
			[CallerArgumentExpression(nameof(index))] string? paramIndexName = null,
			[CallerArgumentExpression(nameof(count))] string? paramCountName = null)
		{
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException(paramIndexName,
					string.Format(ResourceStrings.Arg_Invalid_ArrayOffset, paramArrayName));
			if (count < 0 || count > array.Length)
				throw new ArgumentOutOfRangeException(paramCountName,
					string.Format(ResourceStrings.Arg_Invalid_ArrayOffset, paramArrayName));
			if (count > (array.Length - index))
				throw new ArgumentException(string.Format(
					ResourceStrings.Arg_Invalid_ArrayOffsetOrLength,
					paramIndexName, paramCountName, paramArrayName));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array is not assignable to the specified type <typeparamref name="TExpected" />.
		/// </summary>
		/// <typeparam name="TExpected">The expected array element type.</typeparam>
		/// <param name="array">The array to check.</param>
		/// <param name="paramName">The parameter name of the array.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the array is not of type <typeparamref name="TExpected" />[].
		/// Message: "Target array type is not compatible with the type of items in the collection."
		/// </exception>
		/// <remarks>Validates the runtime type of the array using pattern matching and avoids invalid casts.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayTypeIsNotCompatible<TExpected>(
			Array array,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			if (array is not TExpected[])
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayType, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the provided enum value is not defined for <typeparamref name="TEnum" />.
		/// </summary>
		/// <typeparam name="TEnum">The enum type.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="paramName">The parameter name, inferred from the caller.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="value" /> is not a defined enum member.
		/// Message: "The value is not a valid member of the enumeration {0}."
		/// </exception>
		/// <remarks>Uses <see cref="Enum.IsDefined(Type, object)" /> to check that the enum value is valid.</remarks>
		public static void ThrowIfEnumValueIsUndefined<TEnum>(
			TEnum value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where TEnum : struct, Enum
		{
			if (!Enum.IsDefined(typeof(TEnum), value))
				throw new ArgumentOutOfRangeException(
					paramName,
					string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(TEnum).Name));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is greater than the specified maximum.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to compare.</param>
		/// <param name="max">The upper bound (inclusive).</param>
		/// <param name="paramName">The parameter name of the value.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &gt; <paramref name="max" />.
		/// Message: "The value must be less than or equal to {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfGreaterThan<T>(
			T value, T max,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireLessThanOrEqual, max));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is greater than or equal to the specified maximum.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to compare.</param>
		/// <param name="max">The exclusive upper bound.</param>
		/// <param name="paramName">The parameter name of the value.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &gt;= <paramref name="max" />.
		/// Message: "The value must be less than {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfGreaterThanOrEqual<T>(
			T value, T max,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(max) >= 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireLessThan, max));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the value is greater than another parameter's value.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The current value being validated.</param>
		/// <param name="other">The comparison reference value.</param>
		/// <param name="paramName">Name of the value parameter.</param>
		/// <param name="otherName">Name of the comparison parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> &gt; <paramref name="other" />.
		/// Message: "The value must not be greater than the value of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfGreaterThanOther<T>(
			T value, T other,
			[CallerArgumentExpression(nameof(value))] string? paramName = null,
			[CallerArgumentExpression(nameof(other))] string? otherName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) > 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_GreaterThanOtherParameter, otherName), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the value is greater than or equal to another parameter's value.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value being validated.</param>
		/// <param name="other">The comparison reference value.</param>
		/// <param name="paramName">Name of the value parameter.</param>
		/// <param name="otherName">Name of the comparison parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> &gt;= <paramref name="other" />.
		/// Message: "The value must not be greater than or equal to the value of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfGreaterThanOrEqualOther<T>(
			T value, T other,
			[CallerArgumentExpression(nameof(value))] string? paramName = null,
			[CallerArgumentExpression(nameof(other))] string? otherName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) >= 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_GreaterThanOrEqualOtherParameter, otherName), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the index is outside the valid range of a collection.
		/// </summary>
		/// <typeparam name="T">Type of items in the collection (used for annotation).</typeparam>
		/// <param name="index">The index to validate.</param>
		/// <param name="size">The valid size of the collection.</param>
		/// <param name="paramName">The parameter name for the index.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index" /> is not in [0, <paramref name="size" />).
		/// Message: "The index must be non-negative and less than the size of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfIndexOutOfRange<T>(
			int index, int size,
			[CallerArgumentExpression(nameof(index))] string? paramName = null)
			where T : IComparable<T>
		{
			if (index >= size)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_IndexValidRange, size));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than the specified minimum.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="min">The minimum value allowed.</param>
		/// <param name="paramName">The parameter name for the value.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt; <paramref name="min" />.
		/// Message: "The value must be greater than or equal to {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThan<T>(
			T value, T min,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThanOrEqual, min));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than the specified minimum. Optionally throws
		/// <see cref="ArgumentNullException" /> if the value is null.
		/// </summary>
		/// <typeparam name="T">A comparable value type.</typeparam>
		/// <param name="value">The value to validate (nullable).</param>
		/// <param name="min">The minimum value allowed.</param>
		/// <param name="throwIfNull">Whether to throw if <paramref name="value" /> is null. Default is false.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="value" /> is null and <paramref name="throwIfNull" /> is true.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> is non-null and less than <paramref name="min" />.
		/// Message: "The value must be greater than or equal to {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThan<T>(
			T? value, T min, bool throwIfNull = false,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : struct, IComparable<T>
		{
			if (value is null)
			{
				if (throwIfNull)
					throw new ArgumentNullException(paramName);
			}
			else if (value.Value.CompareTo(min) < 0)
			{
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThanOrEqual, min));
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than or equal to the specified minimum.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="min">The minimum exclusive bound.</param>
		/// <param name="paramName">The parameter name for the value.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt;= <paramref name="min" />.
		/// Message: "The value must be greater than {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThanOrEqual<T>(
			T value, T min,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) <= 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThan, min));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than zero.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt; 0.
		/// Message: "The value must be zero or positive."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNegative<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) < 0)
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequireNonNegative);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is zero.
		/// </summary>
		/// <typeparam name="T">A type that implements <see cref="IEquatable{T}" />.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> equals 0.
		/// Message: "The value must not be zero."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfZero<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IEquatable<T>
		{
			if (value.Equals(default!))
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequireNonZero);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is zero or negative.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt;= 0.
		/// Message: "The value must be a positive number."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfZeroOrNegative<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) <= 0)
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequirePositive);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is zero or positive.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &gt;= 0.
		/// Message: "The value must be a negative number."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfZeroOrPositive<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) >= 0)
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequireNegative);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is not between min and max (exclusive).
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The lower exclusive bound.</param>
		/// <param name="max">The upper exclusive bound.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt;= <paramref name="min" /> or &gt;= <paramref name="max" />.
		/// Message: "The value must be greater than {0} and less than {1}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotBetweenExclusive<T>(
			T value, T min, T max,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenExclusive, min, max));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is not between min and max (inclusive).
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The lower inclusive bound.</param>
		/// <param name="max">The upper inclusive bound.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt; <paramref name="min" /> or &gt; <paramref name="max" />.
		/// Message: "The value must be between {0} and {1}, inclusive."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotBetweenInclusive<T>(
			T value, T min, T max,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenInclusive, min, max));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the object is not assignable to type <typeparamref name="T" />.
		/// </summary>
		/// <typeparam name="T">The target type to validate against.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> is not null and not of type <typeparamref name="T" />, or if it is null and
		/// <typeparamref name="T" /> is a non-nullable value type.
		/// Message: "Object must be of type {0}."
		/// </exception>
		/// <remarks>Null is only allowed if <typeparamref name="T" /> is a reference or nullable type.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotOfType<T>(
			object? value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value is null)
			{
				if (default(T) is not null)
					throw new ArgumentException(
						string.Format(ResourceStrings.Arg_Invalid_MustBeOfType, typeof(T)),
						paramName);
			}
			else if (value is not T)
			{
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_MustBeOfType, typeof(T)),
					paramName);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException" /> if the provided value is <c>null</c>.
		/// </summary>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="value" /> is <c>null</c>.
		/// Message: "Value cannot be null."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNull<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value is null)
				throw new ArgumentNullException(paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException" /> if the value is <c>null</c>, with a custom message.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="message">A custom error message for the exception.</param>
		/// <param name="paramName">The parameter name.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c>.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNull<T>(
			T value,
			string message,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value is null)
				throw new ArgumentNullException(paramName, message);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException" /> if the string is <c>null</c>, or an <see cref="ArgumentException" /> if it is empty.
		/// </summary>
		/// <param name="value">The string value to validate.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> is an empty string.
		/// Message: "The string was either null or empty."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNullOrEmpty(
			string value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value is null)
				throw new ArgumentNullException(paramName);

			if (value.Length == 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringNullOrEmpty, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentNullException" /> if the value is <c>null</c>, or an <see cref="ArgumentException" /> if it is
		/// empty or whitespace.
		/// </summary>
		/// <param name="value">The string value to validate.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="value" /> is <c>null</c>.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> is empty or contains only whitespace.
		/// Message: "The string was either empty or contained only whitespace."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIsNullOrWhiteSpace(
			string value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value is null)
				throw new ArgumentNullException(paramName);

			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringEmptyOrWhitespace, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is positive.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="paramName">The name of the value parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &gt; 0.
		/// Message: "The value must be zero or negative."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfPositive<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) > 0)
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequireNonPositive);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is not equal to zero.
		/// </summary>
		/// <typeparam name="T">A type that supports equality comparison.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> is not zero.
		/// Message: "The value must be zero."
		/// </exception>
		/// <remarks>Ensures a value is exactly zero — commonly used for flags, counters, or reset validation.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotZero<T>(
			T value,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IEquatable<T>
		{
			if (!value.Equals(default!))
				throw new ArgumentOutOfRangeException(paramName, ResourceStrings.Arg_OutOfRange_RequireZero);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is not a positive multiple of the specified divisor.
		/// </summary>
		/// <param name="value">The value to validate.</param>
		/// <param name="divisor">The required positive divisor.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> is not greater than zero or not divisible by <paramref name="divisor" />.
		/// Message: "The value must be a positive number and a multiple of {0}."
		/// </exception>
		/// <remarks>Useful for validating aligned buffer sizes, memory boundaries, or block-aligned lengths.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNotPositiveMultipleOf(
			int value,
			int divisor,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
		{
			if (value <= 0 || value % divisor != 0)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_Invalid_PositiveMultipleOf, divisor));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array contains any non-numeric items.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when any element in the array is not a numeric type.
		/// Message: "The array contains non-numeric values."
		/// </exception>
		/// <remarks>Validates that each element is of a numeric type (e.g., int, double, float, etc.).</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayContainsNonNumeric(
			Array array,
			[CallerArgumentExpression(nameof(array))] string? paramName = null)
		{
			foreach (var item in array)
			{
				if (item == null) continue;
				TypeCode code = Type.GetTypeCode(item.GetType());
				if (code < TypeCode.SByte || code > TypeCode.Decimal)
					throw new ArgumentException(ResourceStrings.Arg_Invalid_Array_NumericOnly, paramName);
			}
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the collection has fewer elements than the specified minimum.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The collection to validate.</param>
		/// <param name="minCount">The minimum number of required elements.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="collection" /> has fewer than <paramref name="minCount" /> elements.
		/// Message: "Collection contains insufficient elements."
		/// </exception>
		/// <remarks>Performs a minimum count check on the collection.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfCollectionTooSmall<T>(
			ICollection<T> collection,
			int minCount,
			[CallerArgumentExpression(nameof(collection))] string? paramName = null)
		{
			if (collection.Count < minCount)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_CollectionTooSmall, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the string comparison option is invalid or unsupported.
		/// </summary>
		/// <param name="comparison">The <see cref="StringComparison" /> value to validate.</param>
		/// <param name="paramName">The name of the parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="comparison" /> is not a valid <see cref="StringComparison" /> enum value.
		/// Message: "The string comparison type is not supported."
		/// </exception>
		/// <remarks>Useful for guarding API input that relies on specific string comparison modes.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfInvalidStringComparison(
			StringComparison comparison,
			[CallerArgumentExpression(nameof(comparison))] string? paramName = null)
		{
			if (!Enum.IsDefined(typeof(StringComparison), comparison))
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringComparison, paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the value is less than another parameter's value.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The current value being validated.</param>
		/// <param name="other">The comparison reference value.</param>
		/// <param name="paramName">Name of the value parameter.</param>
		/// <param name="otherName">Name of the comparison parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> &gt; <paramref name="other" />.
		/// Message: "The value must not be less than the value of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThanOther<T>(
			T value, T other,
			[CallerArgumentExpression(nameof(value))] string? paramName = null,
			[CallerArgumentExpression(nameof(other))] string? otherName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) < 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_LessThanOtherParameter, otherName), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the value is less than or equal to another parameter's value.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value being validated.</param>
		/// <param name="other">The comparison reference value.</param>
		/// <param name="paramName">Name of the value parameter.</param>
		/// <param name="otherName">Name of the comparison parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> &gt;= <paramref name="other" />.
		/// Message: "The value must not be less than or equal to the value of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThanOrEqualOther<T>(
			T value, T other,
			[CallerArgumentExpression(nameof(value))] string? paramName = null,
			[CallerArgumentExpression(nameof(other))] string? otherName = null)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) <= 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_LessThanOrEqualOtherParameter, otherName), paramName);
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the calculated sequence would exceed the maximum value for <see cref="Int32" />.
		/// </summary>
		/// <param name="start">The starting value of the sequence.</param>
		/// <param name="count">The number of values to generate.</param>
		/// <param name="paramName">The name of the parameter representing <paramref name="count" />.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="start" /> + <paramref name="count" /> - 1 would exceed <see cref="Int32.MaxValue" />.
		/// </exception>
		/// <remarks>This check prevents arithmetic overflow when generating sequences like ranges.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSequenceRangeOverflows(
			int start, int count,
			[CallerArgumentExpression(nameof(count))] string? paramName = null)
		{
			if (count > 0 && start > int.MaxValue - (count - 1))
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_SequenceRangeOverflow, nameof(Int32)));
		}

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the calculated sequence would exceed the maximum value for <see cref="Int64" />.
		/// </summary>
		/// <param name="start">The starting value of the sequence.</param>
		/// <param name="count">The number of values to generate.</param>
		/// <param name="paramName">The name of the parameter representing <paramref name="count" />.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="start" /> + <paramref name="count" /> - 1 would exceed <see cref="Int64.MaxValue" />.
		/// </exception>
		/// <remarks>This check prevents arithmetic overflow when generating long-based numeric sequences.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSequenceRangeOverflows(
			long start, int count,
			[CallerArgumentExpression(nameof(count))] string? paramName = null)
		{
			if (count > 0 && start > long.MaxValue - (count - 1))
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_SequenceRangeOverflow, nameof(Int64)));
		}
	}
}
