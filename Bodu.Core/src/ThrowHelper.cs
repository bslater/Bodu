// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="ThrowHelper.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System.Runtime.CompilerServices;

namespace Bodu
{
	/// <summary>
	/// Provides centralized guard clause methods for argument validation using resource-based exception messages.
	/// </summary>
	public static class ThrowHelper
	{
#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array contains any non-numeric items.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when any element in the array is not a numeric type.
		/// Message: "The array contains non-numeric values."
		/// </exception>
		/// <remarks>Validates that each element is of a numeric type (e.g., int, double, float, etc.).</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayContainsNonNumeric(
			Array array)
		{
			foreach (var item in array)
			{
				if (item is null) continue;

				var type = item.GetType();

				// Unbox nullable to underlying type if needed
				if (Nullable.GetUnderlyingType(type) is Type underlying)
					type = underlying;

				if (type != typeof(byte) && type != typeof(sbyte) &&
					type != typeof(short) && type != typeof(ushort) &&
					type != typeof(int) && type != typeof(uint) &&
					type != typeof(long) && type != typeof(ulong) &&
					type != typeof(float) && type != typeof(double) &&
					type != typeof(decimal))
				{
					throw new ArgumentException(ResourceStrings.Arg_Invalid_Array_NumericOnly, nameof(array));
				}
			}
		}
#else

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
				if (item is null) continue;

				var type = item.GetType();

				// Unbox nullable to underlying type if needed
				if (Nullable.GetUnderlyingType(type) is Type underlying)
					type = underlying;

				if (type != typeof(byte) && type != typeof(sbyte) &&
					type != typeof(short) && type != typeof(ushort) &&
					type != typeof(int) && type != typeof(uint) &&
					type != typeof(long) && type != typeof(ulong) &&
					type != typeof(float) && type != typeof(double) &&
					type != typeof(decimal))
				{
					throw new ArgumentException(ResourceStrings.Arg_Invalid_Array_NumericOnly, paramName);
				}
			}
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the specified array is not single-dimensional.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="array" /> has a rank other than 1.
		/// Message: "Only single dimension arrays are supported here."
		/// </exception>
		/// <remarks>Checks that <c>array.Rank == 1</c>.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayIsNotSingleDimension(
			Array array)
		{
			if (array.Rank != 1)
				throw new ArgumentException(ResourceStrings.Rank_MultiDimensionArrayNotSupported, nameof(array));
		}

#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array does not have a zero lower bound.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>array.GetLowerBound(0) != 0</c>.
		/// Message: "The lower bound of target array must be zero."
		/// </exception>
		/// <remarks>Checks that the array is zero-based (i.e., index starts at 0).</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayIsNotZeroBased(
			Array array)
		{
			if (array.GetLowerBound(0) != 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayNonZeroLowerBound, nameof(array));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an exception if the array does not have enough elements from the specified index to accommodate the required length.
		/// </summary>
		/// <param name="array">The array to validate. Must be non-null and single-dimensional.</param>
		/// <param name="index">The starting index to validate against.</param>
		/// <param name="requiredLength">The number of elements required starting from <paramref name="index" />.</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index" /> is negative.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown when <c><paramref name="array" />.Length - <paramref name="index" /> &lt; <paramref name="requiredLength" /></c>.
		/// Message: "Array is too short. Required minimum is {0} from a specified index."
		/// </exception>
		/// <remarks>
		/// This method ensures that a caller can safely access a block of <paramref name="requiredLength" /> elements starting at
		/// <paramref name="index" /> without exceeding array bounds.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthIsInsufficient(
			Array array, int index, int requiredLength)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.Arg_OutOfRange_IndexValidRange, nameof(array)), nameof(index));
			if (array.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_ArrayTooShort, requiredLength), nameof(array));
		}
#else

		/// <summary>
		/// Throws an exception if the array does not have enough elements from the specified index to accommodate the required length.
		/// </summary>
		/// <param name="array">The array to validate. Must be non-null and single-dimensional.</param>
		/// <param name="index">The starting index to validate against.</param>
		/// <param name="requiredLength">The number of elements required starting from <paramref name="index" />.</param>
		/// <param name="paramArrayName">The name of the array parameter (captured automatically).</param>
		/// <param name="paramIndexName">The name of the index parameter (captured automatically).</param>
		/// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index" /> is negative.</exception>
		/// <exception cref="ArgumentException">
		/// Thrown when <c><paramref name="array" />.Length - <paramref name="index" /> &lt; <paramref name="requiredLength" /></c>.
		/// Message: "Array is too short. Required minimum is {0} from a specified index."
		/// </exception>
		/// <remarks>
		/// This method ensures that a caller can safely access a block of <paramref name="requiredLength" /> elements starting at
		/// <paramref name="index" /> without exceeding array bounds.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthIsInsufficient(
			Array array, int index, int requiredLength,
			[CallerArgumentExpression(nameof(array))] string? paramArrayName = null,
			[CallerArgumentExpression(nameof(array))] string? paramIndexName = null)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException(string.Format(ResourceStrings.Arg_OutOfRange_IndexValidRange, paramArrayName), paramIndexName);
			if (array.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_ArrayTooShort, requiredLength), paramArrayName);
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array has zero length.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>array.Length == 0</c>.
		/// Message: "The provided array has zero length. Length must be greater than zero."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthIsZero(
			Array array)
		{
			if (array.Length == 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayIsZeroLength, nameof(array));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array length is not a positive multiple of a given divisor.
		/// </summary>
		/// <param name="array">The array to check.</param>
		/// <param name="divisor">The required positive divisor.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the array length is 0 or not divisible by <paramref name="divisor" />.
		/// Message: "Length of the array must be a multiple of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayLengthNotPositiveMultipleOf(
			Array array,
			int divisor)
		{
			if (array.Length == 0 || array.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_ArrayLengthMultipleOf, divisor), nameof(array));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an exception if the specified <paramref name="index" /> and <paramref name="count" /> define a segment that exceeds the
		/// bounds of the <paramref name="array" />.
		/// </summary>
		/// <param name="array">The array to validate.</param>
		/// <param name="index">The zero-based starting index within the array.</param>
		/// <param name="count">The number of elements to access from <paramref name="index" />.</param>
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
			int count)
		{
			if (index < 0 || index > array.Length)
				throw new ArgumentOutOfRangeException(nameof(index),
					string.Format(ResourceStrings.Arg_Invalid_ArrayOffset, nameof(array)));
			if (count < 0 || count > array.Length)
				throw new ArgumentOutOfRangeException(nameof(count),
					string.Format(ResourceStrings.Arg_Invalid_ArrayOffset, nameof(array)));
			if (count > (array.Length - index))
				throw new ArgumentException(string.Format(
					ResourceStrings.Arg_Invalid_ArrayOffsetOrLength,
					nameof(index), nameof(count), nameof(array)));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the array is not assignable to the specified type <typeparamref name="TExpected" />.
		/// </summary>
		/// <typeparam name="TExpected">The expected array element type.</typeparam>
		/// <param name="array">The array to check.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when the array is not of type <typeparamref name="TExpected" />[].
		/// Message: "Target array type is not compatible with the type of items in the collection."
		/// </exception>
		/// <remarks>Validates the runtime type of the array using pattern matching and avoids invalid casts.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfArrayTypeIsNotCompatible<TExpected>(
			Array array)
		{
			if (array is not TExpected[])
				throw new ArgumentException(ResourceStrings.Arg_Invalid_ArrayType, nameof(array));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the collection has fewer elements than the specified minimum.
		/// </summary>
		/// <typeparam name="T">The element type.</typeparam>
		/// <param name="collection">The collection to validate.</param>
		/// <param name="minCount">The minimum number of required elements.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="collection" /> has fewer than <paramref name="minCount" /> elements.
		/// Message: "Collection contains insufficient elements."
		/// </exception>
		/// <remarks>Performs a minimum count check on the collection.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfCollectionTooSmall<T>(
			ICollection<T> collection,
			int minCount)
		{
			if (collection.Count < minCount)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_CollectionTooSmall, nameof(collection));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the specified <paramref name="value" /> is <c>null</c> when the
		/// <paramref name="conditionalParam" /> matches the specified <paramref name="conditionalValue" />.
		/// </summary>
		/// <typeparam name="TValue">The type of the parameter being validated.</typeparam>
		/// <typeparam name="TCondition">The type of the conditional parameter.</typeparam>
		/// <param name="value">The parameter value to validate for null.</param>
		/// <param name="conditionalParam">The current value of the conditional parameter.</param>
		/// <param name="conditionalValue">The conditional value that requires <paramref name="value" /> to be non-null.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="conditionalParam" /> equals <paramref name="conditionalValue" /> and <paramref name="value" /> is
		/// <c>null</c>. The exception message follows the pattern: "Parameter '{0}' is required when '{1}' is '{2}'."
		/// </exception>
		/// <remarks>Use this method when a parameter becomes mandatory based on the value of another parameter.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfConditionallyRequiredParameterIsNull<TValue, TCondition>(
			TValue? value,
			TCondition conditionalParam,
			TCondition conditionalValue)
		{
			if (EqualityComparer<TCondition>.Default.Equals(conditionalParam, conditionalValue) && value is null)
			{
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Required_ParameterRequiredIf, nameof(value), nameof(conditionalParam), nameof(conditionalValue)),
					nameof(value));
			}
		}
#else

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the specified <paramref name="value" /> is <c>null</c> when the
		/// <paramref name="conditionalParam" /> matches the specified <paramref name="conditionalValue" />.
		/// </summary>
		/// <typeparam name="TValue">The type of the parameter being validated.</typeparam>
		/// <typeparam name="TCondition">The type of the conditional parameter.</typeparam>
		/// <param name="value">The parameter value to validate for null.</param>
		/// <param name="conditionalParam">The current value of the conditional parameter.</param>
		/// <param name="conditionalValue">The conditional value that requires <paramref name="value" /> to be non-null.</param>
		/// <param name="paramName">The name of the <paramref name="value" /> parameter (captured automatically).</param>
		/// <param name="conditionalParamName">The name of the <paramref name="conditionalParam" /> parameter (captured automatically).</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <paramref name="conditionalParam" /> equals <paramref name="conditionalValue" /> and <paramref name="value" /> is
		/// <c>null</c>. The exception message follows the pattern: "Parameter '{0}' is required when '{1}' is '{2}'."
		/// </exception>
		/// <remarks>Use this method when a parameter becomes mandatory based on the value of another parameter.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfConditionallyRequiredParameterIsNull<TValue, TCondition>(
			TValue? value,
			TCondition conditionalParam,
			TCondition conditionalValue,
			[CallerArgumentExpression(nameof(value))] string? paramName = null,
			[CallerArgumentExpression(nameof(conditionalParam))] string? conditionalParamName = null)
		{
			if (EqualityComparer<TCondition>.Default.Equals(conditionalParam, conditionalValue) && value is null)
			{
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Required_ParameterRequiredIf, paramName, conditionalParamName, conditionalValue),
					paramName);
			}
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the specified <paramref name="count" /> is less than zero or greater
		/// than the number of <paramref name="available" /> items.
		/// </summary>
		/// <param name="count">The count value to validate.</param>
		/// <param name="available">The number of available items.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="count" /> is negative or greater than <paramref name="available" />.
		/// Message: "Count must be non-negative and not exceed the number of available items."
		/// </exception>
		/// <remarks>Use this method when validating that a subset operation will not exceed the size of the source.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfCountExceedsAvailable(
			int count,
			int available)
		{
			if (count < 0 || count > available)
				throw new ArgumentOutOfRangeException(nameof(count),
					string.Format(ResourceStrings.Arg_OutOfRange_CountExceedsAvailable, available));
		}
#else

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the specified <paramref name="count" /> is less than zero or greater
		/// than the number of <paramref name="available" /> items.
		/// </summary>
		/// <param name="count">The count value to validate.</param>
		/// <param name="available">The number of available items.</param>
		/// <param name="paramName">The name of the parameter being validated (usually "count").</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="count" /> is negative or greater than <paramref name="available" />.
		/// Message: "Count must be non-negative and not exceed the number of available items."
		/// </exception>
		/// <remarks>Use this method when validating that a subset operation will not exceed the size of the source.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfCountExceedsAvailable(
			int count,
			int available,
			[CallerArgumentExpression(nameof(count))] string? paramName = null)
		{
			if (count < 0 || count > available)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_CountExceedsAvailable, available));
		}

#endif

#if NETSTANDARD2_1_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the destination span is smaller than the source span.
		/// </summary>
		/// <typeparam name="TSource">The type of the source span elements.</typeparam>
		/// <typeparam name="TDestination">The type of the destination span elements.</typeparam>
		/// <param name="source">The source span.</param>
		/// <param name="destination">The destination span.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="destination" /> is shorter than <paramref name="source" />.
		/// Message: "The destination span must be at least as long as the source span."
		/// </exception>
		/// <remarks>Useful for validating buffer-to-buffer operations such as copying or endian swapping.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfDestinationSpanTooSmall<TSource, TDestination>(
			ReadOnlySpan<TSource> source,
			Span<TDestination> destination)
		{
			if (destination.Length < source.Length)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_DestinationTooSmall, "span"),
					nameof(destination));
		}
#elif NET5_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the destination span is smaller than the source span.
		/// </summary>
		/// <typeparam name="TSource">The type of the source span elements.</typeparam>
		/// <typeparam name="TDestination">The type of the destination span elements.</typeparam>
		/// <param name="source">The source span.</param>
		/// <param name="destination">The destination span.</param>
		/// <param name="paramDestinationName">The name of the destination parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="destination" /> is shorter than <paramref name="source" />.
		/// Message: "The destination span must be at least as long as the source span."
		/// </exception>
		/// <remarks>Useful for validating buffer-to-buffer operations such as copying or endian swapping.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfDestinationSpanTooSmall<TSource, TDestination>(
			ReadOnlySpan<TSource> source,
			Span<TDestination> destination,
			[CallerArgumentExpression(nameof(destination))] string? paramDestinationName = null)
		{
			if (destination.Length < source.Length)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_DestinationTooSmall, "span"),
					paramDestinationName);
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the destination array is smaller than the source array.
		/// </summary>
		/// <typeparam name="TSource">The type of the source array elements.</typeparam>
		/// <typeparam name="TDestination">The type of the destination array elements.</typeparam>
		/// <param name="source">The source array.</param>
		/// <param name="destination">The destination array.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="destination" /> is shorter than <paramref name="source" />.
		/// Message: "The destination array (length {0}) must be at least as long as the source array (length {1})."
		/// </exception>
		/// <remarks>
		/// Useful for validating array-to-array operations such as copying or endian swapping. Null checks must be handled separately.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfDestinationTooSmall<TSource, TDestination>(
			TSource[] source,
			TDestination[] destination)
		{
			if (destination.Length < source.Length)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_DestinationTooSmall, "array"),
					nameof(destination));
		}
#else

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the destination array is smaller than the source array.
		/// </summary>
		/// <typeparam name="TSource">The type of the source array elements.</typeparam>
		/// <typeparam name="TDestination">The type of the destination array elements.</typeparam>
		/// <param name="source">The source array.</param>
		/// <param name="destination">The destination array.</param>
		/// <param name="paramDestinationName">The name of the destination parameter.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="destination" /> is shorter than <paramref name="source" />.
		/// Message: "The destination array (length {0}) must be at least as long as the source array (length {1})."
		/// </exception>
		/// <remarks>
		/// Useful for validating array-to-array operations such as copying or endian swapping. Null checks must be handled separately.
		/// </remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfDestinationTooSmall<TSource, TDestination>(
			TSource[] source,
			TDestination[] destination,
			[CallerArgumentExpression(nameof(destination))] string? paramDestinationName = null)
		{
			if (destination.Length < source.Length)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_DestinationTooSmall, "array"),
					paramDestinationName);
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			TEnum value			)
			where TEnum : struct, Enum
		{
			if (!Enum.IsDefined(typeof(TEnum), value))
				throw new ArgumentOutOfRangeException(
					nameof(value),
					string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(TEnum).Name));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T max)
			where T : IComparable<T>
		{
			if (value.CompareTo(max) > 0)
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_OutOfRange_RequireLessThanOrEqual, max));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T max)
			where T : IComparable<T>
		{
			if (value.CompareTo(max) >= 0)
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_OutOfRange_RequireLessThan, max));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the value is greater than or equal to another parameter's value.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value being validated.</param>
		/// <param name="other">The comparison reference value.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="value" /> &gt;= <paramref name="other" />.
		/// Message: "The value must not be greater than or equal to the value of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfGreaterThanOrEqualOther<T>(
			T value, T other)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) >= 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_GreaterThanOrEqualOtherParameter, nameof(other)), nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T other)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) > 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_GreaterThanOtherParameter, nameof(other)), nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the index is outside the valid range of the specified array.
		/// </summary>
		/// <param name="index">The index to validate.</param>
		/// <param name="array">The array to validate the index against.</param>
		/// <param name="paramName">The name of the parameter representing the index.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index" /> is less than zero or greater than or equal to <c><paramref name="array" />.LongLength</c>.
		/// The exception message will state: "The index must be non-negative and less than the size of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfIndexOutOfRange(
			long index, Array array)
		{
			if (index < 0 || index >= array.LongLength)
				throw new ArgumentOutOfRangeException(nameof(index),
					string.Format(ResourceStrings.Arg_OutOfRange_IndexValidRange, array.LongLength));
		}
#else

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the index is outside the valid range of the specified array.
		/// </summary>
		/// <param name="index">The index to validate.</param>
		/// <param name="array">The array to validate the index against.</param>
		/// <param name="paramName">The name of the parameter representing the index.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="index" /> is less than zero or greater than or equal to <c><paramref name="array" />.LongLength</c>.
		/// The exception message will state: "The index must be non-negative and less than the size of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfIndexOutOfRange(
			long index, Array array,
			[CallerArgumentExpression(nameof(index))] string? paramName = null)
		{
			if (index < 0 || index >= array.LongLength)
				throw new ArgumentOutOfRangeException(paramName,
					string.Format(ResourceStrings.Arg_OutOfRange_IndexValidRange, array.LongLength));
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the string comparison option is invalid or unsupported.
		/// </summary>
		/// <param name="comparison">The <see cref="StringComparison" /> value to validate.</param>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="comparison" /> is not a valid <see cref="StringComparison" /> enum value.
		/// Message: "The string comparison type is not supported."
		/// </exception>
		/// <remarks>Useful for guarding API input that relies on specific string comparison modes.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfInvalidStringComparison(
			StringComparison comparison)
		{
			if (!Enum.IsDefined(typeof(StringComparison), comparison))
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringComparison, nameof(comparison));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T min)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) < 0)
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThanOrEqual, min));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than the specified minimum. Optionally throws
		/// <see cref="ArgumentNullException" /> if the value is null.
		/// </summary>
		/// <typeparam name="T">A comparable value type.</typeparam>
		/// <param name="value">The value to validate (nullable).</param>
		/// <param name="min">The minimum value allowed.</param>
		/// <param name="throwIfNull">Whether to throw if <paramref name="value" /> is null. Default is false.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="value" /> is null and <paramref name="throwIfNull" /> is true.
		/// </exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> is non-null and less than <paramref name="min" />.
		/// Message: "The value must be greater than or equal to {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThan<T>(
			T? value, T min, bool throwIfNull = false)
			where T : struct, IComparable<T>
		{
			if (value is null)
			{
				if (throwIfNull)
					throw new ArgumentNullException(nameof(value));
			}
			else if (value.Value.CompareTo(min) < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThanOrEqual, min));
			}
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than or equal to the specified minimum.
		/// </summary>
		/// <typeparam name="T">A comparable type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <param name="min">The minimum exclusive bound.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt;= <paramref name="min" />.
		/// Message: "The value must be greater than {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfLessThanOrEqual<T>(
			T value, T min)
			where T : IComparable<T>
		{
			if (value.CompareTo(min) <= 0)
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_OutOfRange_RequireGreaterThan, min));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T other)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) <= 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_LessThanOrEqualOtherParameter, nameof(other)), nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value, T other)
			where T : IComparable<T>
		{
			if (value.CompareTo(other) < 0)
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_LessThanOtherParameter, nameof(other)), nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is less than zero.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt; 0.
		/// Message: "The value must be zero or positive."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNegative<T>(
			T value)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) < 0)
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequireNonNegative);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the specified value is not within the given range.
		/// </summary>
		/// <typeparam name="T">A type that implements <see cref="IComparable{T}" />.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The lower bound of the range.</param>
		/// <param name="max">The upper bound of the range.</param>
		/// <param name="inclusive">
		/// If <c>true</c>, the range check is inclusive (i.e., value must be between <paramref name="min" /> and <paramref name="max" />,
		/// inclusive); otherwise, the check is exclusive.
		/// </param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the value is outside the specified range.
		/// <list type="bullet">
		/// <item>
		/// If <paramref name="inclusive" /> is <c>true</c>, the exception is thrown when <paramref name="value" /> is less than
		/// <paramref name="min" /> or greater than <paramref name="max" />.
		/// </item>
		/// <item>
		/// If <paramref name="inclusive" /> is <c>false</c>, the exception is thrown when <paramref name="value" /> is less than or equal
		/// to <paramref name="min" /> or greater than or equal to <paramref name="max" />.
		/// </item>
		/// </list>
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfOutOfRange<T>(
			T value, T min, T max, bool inclusive = true)
			where T : IComparable<T>
		{
			if (inclusive)
			{
				if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
					throw new ArgumentOutOfRangeException(nameof(value),
						string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenInclusive, min, max));
			}
			else
			{
				if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
					throw new ArgumentOutOfRangeException(nameof(value),
						string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenExclusive, min, max));
			}
		}

#else

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the specified value is not within the given range.
		/// </summary>
		/// <typeparam name="T">A type that implements <see cref="IComparable{T}" />.</typeparam>
		/// <param name="value">The value to validate.</param>
		/// <param name="min">The lower bound of the range.</param>
		/// <param name="max">The upper bound of the range.</param>
		/// <param name="inclusive">
		/// If <c>true</c>, the range check is inclusive (i.e., value must be between <paramref name="min" /> and <paramref name="max" />,
		/// inclusive); otherwise, the check is exclusive.
		/// </param>
		/// <param name="paramName">The name of the parameter being validated.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the value is outside the specified range.
		/// <list type="bullet">
		/// <item>
		/// If <paramref name="inclusive" /> is <c>true</c>, the exception is thrown when <paramref name="value" /> is less than
		/// <paramref name="min" /> or greater than <paramref name="max" />.
		/// </item>
		/// <item>
		/// If <paramref name="inclusive" /> is <c>false</c>, the exception is thrown when <paramref name="value" /> is less than or equal
		/// to <paramref name="min" /> or greater than or equal to <paramref name="max" />.
		/// </item>
		/// </list>
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfOutOfRange<T>(
			T value, T min, T max, bool inclusive = true,
			[CallerArgumentExpression(nameof(value))] string? paramName = null)
			where T : IComparable<T>
		{
			if (inclusive)
			{
				if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
					throw new ArgumentOutOfRangeException(paramName,
						string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenInclusive, min, max));
			}
			else
			{
				if (value.CompareTo(min) <= 0 || value.CompareTo(max) >= 0)
					throw new ArgumentOutOfRangeException(paramName,
						string.Format(ResourceStrings.Arg_OutOfRange_RequireBetweenExclusive, min, max));
			}
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			object? value)
		{
			if (value is null)
			{
				if (default(T) is not null)
					throw new ArgumentException(
						string.Format(ResourceStrings.Arg_Invalid_MustBeOfType, typeof(T)),
						nameof(value));
			}
			else if (value is not T)
			{
				throw new ArgumentException(
					string.Format(ResourceStrings.Arg_Invalid_MustBeOfType, typeof(T)),
					nameof(value));
			}
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			int divisor)
		{
			if (value <= 0 || value % divisor != 0)
				throw new ArgumentOutOfRangeException(nameof(value),
					string.Format(ResourceStrings.Arg_Invalid_PositiveMultipleOf, divisor));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value)
			where T : IEquatable<T>
		{
			if (!value.Equals(default!))
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequireZero);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentNullException" /> if the provided value is <c>null</c>.
		/// </summary>
		/// <typeparam name="T">The type of the object.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="value" /> is <c>null</c>.
		/// Message: "Value cannot be null."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfNull<T>(			T value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			string message)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value), message);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			string value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));

			if (value.Length == 0)
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringNullOrEmpty, nameof(value));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) > 0)
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequireNonPositive);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			int start, int count)
		{
			if (count > 0 && start > int.MaxValue - (count - 1))
				throw new ArgumentOutOfRangeException(nameof(start),
					string.Format(ResourceStrings.Arg_OutOfRange_SequenceRangeOverflow, nameof(Int32)));
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the calculated sequence would exceed the maximum value for <see cref="Int64" />.
		/// </summary>
		/// <param name="start">The starting value of the sequence.</param>
		/// <param name="count">The number of values to generate.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown when <paramref name="start" /> + <paramref name="count" /> - 1 would exceed <see cref="Int64.MaxValue" />.
		/// </exception>
		/// <remarks>This check prevents arithmetic overflow when generating long-based numeric sequences.</remarks>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSequenceRangeOverflows(
			long start, int count)
		{
			if (count > 0 && start > long.MaxValue - (count - 1))
				throw new ArgumentOutOfRangeException(nameof(start),
					string.Format(ResourceStrings.Arg_OutOfRange_SequenceRangeOverflow, nameof(Int64)));
		}
#else

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

#endif

#if NETSTANDARD2_1_OR_GREATER

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
			ReadOnlySpan<T> span, int index, int requiredLength)
		{
			if (span.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanTooShort, requiredLength), nameof(span));
		}
#elif NET5_0_OR_GREATER

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
			ReadOnlySpan<T> span, int index, int requiredLength,
			[CallerArgumentExpression(nameof(span))] string? paramName = null)
		{
			if (span.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanTooShort, requiredLength), paramName);
		}

#endif

#if NETSTANDARD2_1_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the remaining length of the span from the given index is less than required.
		/// </summary>
		/// <typeparam name="T">The element type of the span.</typeparam>
		/// <param name="span">The span to check.</param>
		/// <param name="index">The index from which to measure the remaining length.</param>
		/// <param name="requiredLength">The required number of elements.</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>span.Length - index &lt; requiredLength</c>.
		/// Message: "Span is too short. Required minimum is {0} from a specified index."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSpanLengthIsInsufficient<T>(
			Span<T> span, int index, int requiredLength)
		{
			if (span.Length - index < requiredLength)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanTooShort, requiredLength), nameof(span));
		}
#elif NET5_0_OR_GREATER

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

#endif

#if NETSTANDARD2_1_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the span length is not a positive multiple of a given divisor.
		/// </summary>
		/// <typeparam name="T">The element type of the span.</typeparam>
		/// <param name="span">The span to check.</param>
		/// <param name="divisor">The divisor that span length must be a multiple of.</param>
		/// <param name="func">A factory for a custom exception (unused in default implementation).</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>span.Length == 0 || span.Length % divisor != 0</c>.
		/// Message: "Length of the Span must be a multiple of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSpanLengthNotPositiveMultipleOf<T>(
			ReadOnlySpan<T> span,
			int divisor,
			Func<string, Exception>? func = null)
		{
			if (span.Length == 0 || span.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanLengthMultipleOf, divisor), nameof(span));
		}
#elif NET5_0_OR_GREATER

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

#endif

#if NETSTANDARD2_1_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentException" /> if the span length is not a positive multiple of a given divisor.
		/// </summary>
		/// <typeparam name="T">The element type of the span.</typeparam>
		/// <param name="span">The span to check.</param>
		/// <param name="divisor">The divisor that span length must be a multiple of.</param>
		/// <param name="func">A factory for a custom exception (unused in default implementation).</param>
		/// <exception cref="ArgumentException">
		/// Thrown when <c>span.Length == 0 || span.Length % divisor != 0</c>.
		/// Message: "Length of the Span must be a multiple of {0}."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfSpanLengthNotPositiveMultipleOf<T>(
			Span<T> span,
			int divisor,
			Func<string, Exception>? func = null)
		{
			if (span.Length == 0 || span.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanLengthMultipleOf, divisor), nameof(span));
		}
#elif NET5_0_OR_GREATER

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
			Span<T> span,
			int divisor,
			Func<string, Exception>? func = null,
			[CallerArgumentExpression(nameof(span))] string? paramName = null)
		{
			if (span.Length == 0 || span.Length % divisor != 0)
				throw new ArgumentException(string.Format(ResourceStrings.Arg_Invalid_SpanLengthMultipleOf, divisor), paramName);
		}

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value)
			where T : IEquatable<T>
		{
			if (value.Equals(default!))
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequireNonZero);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

		/// <summary>
		/// Throws an <see cref="ArgumentOutOfRangeException" /> if the value is zero or negative.
		/// </summary>
		/// <typeparam name="T">A comparable numeric type.</typeparam>
		/// <param name="value">The value to check.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="value" /> &lt;= 0.
		/// Message: "The value must be a positive number."
		/// </exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ThrowIfZeroOrNegative<T>(
			T value)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) <= 0)
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequirePositive);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			T value)
			where T : IComparable<T>
		{
			if (value.CompareTo(default!) >= 0)
				throw new ArgumentOutOfRangeException(nameof(value), ResourceStrings.Arg_OutOfRange_RequireNegative);
		}
#else

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

#endif

#if NETSTANDARD2_0_OR_GREATER

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
			string value)
		{
			if (value is null)
				throw new ArgumentNullException(nameof(value));

			if (string.IsNullOrWhiteSpace(value))
				throw new ArgumentException(ResourceStrings.Arg_Invalid_StringEmptyOrWhitespace, nameof(value));
		}
#else

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

#endif
	}
}