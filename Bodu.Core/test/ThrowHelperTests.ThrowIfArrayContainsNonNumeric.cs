// ---------------------------------------------------------------------------------------------------------------
// <copyright file="ThrowHelperTests.ThrowIfArrayContainsNonNumeric.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Bodu
{
	public partial class ThrowHelperTests
	{
		[DataTestMethod]
		[DynamicData(nameof(GetNonNumericArrayTestData), DynamicDataSourceType.Method)]
		public void ThrowIfArrayContainsNonNumeric_WhenArrayContainsNonNumeric_ShouldThrowExactly(Array array)
		{
			Assert.ThrowsExactly<ArgumentException>(() =>
			{
				ThrowHelper.ThrowIfArrayContainsNonNumeric(array);
			});
		}

		[DataTestMethod]
		[DynamicData(nameof(GetNumericArrayTestData), DynamicDataSourceType.Method)]
		public void ThrowIfArrayContainsNonNumeric_WhenArrayIsNumeric_ShouldNotThrow(Array array)
		{
			ThrowHelper.ThrowIfArrayContainsNonNumeric(array);
		}

		private static IEnumerable<object[]> GetNonNumericArrayTestData()
		{
			yield return new object[] { new object[] { 1, 2.0, "string" } };   // includes string
			yield return new object[] { new object[] { "abc" } };              // only string
			yield return new object[] { new object[] { new object() } };       // object instance
		}

		private static IEnumerable<object[]> GetNumericArrayTestData()
		{
			yield return new object[] { new object[] { 1, 2, 3, 4 } };                          // all int
			yield return new object[] { new object[] { 1.5, 2.5 } };                            // float/double
			yield return new object[] { new object[] { 1m, 2m, null } };                        // decimal + null
			yield return new object[] { new object[] { (byte)1, (short)2, (long)3 } };          // mixed numeric
			yield return new object[] { new object[] { (sbyte)1, (ushort)2, (uint)3, (ulong)4 } }; // all unsigned
		}
	}
}