﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	public static partial class TestHelpers
	{
		/// <summary>
		/// Compares two byte sequences and writes detailed trace output if any differences are found.
		/// </summary>
		/// <param name="expected">The expected byte sequence.</param>
		/// <param name="actual">The actual byte sequence.</param>
		/// <param name="label">An optional label to prefix the trace output.</param>
		public static void TraceWriteIfNotEqual(ReadOnlySpan<byte> expected, ReadOnlySpan<byte> actual, string? label = null)
		{
			if (expected.SequenceEqual(actual))
				return;

			string prefix = string.IsNullOrWhiteSpace(label) ? string.Empty : $"[{label}] ";

			Trace.WriteLine($"{prefix}Mismatch detected:");
			Trace.WriteLine($"{prefix}Expected: {Convert.ToHexString(expected)}");
			Trace.WriteLine($"{prefix}Actual:   {Convert.ToHexString(actual)}");

			int minLength = Math.Min(expected.Length, actual.Length);
			for (int i = 0; i < minLength; i++)
			{
				if (expected[i] != actual[i])
				{
					Trace.WriteLine($"{prefix}First mismatch at index {i}: Expected {expected[i]:X2}, Actual {actual[i]:X2}");
					break;
				}
			}

			if (expected.Length != actual.Length)
			{
				Trace.WriteLine($"{prefix}Length mismatch: Expected {expected.Length} bytes, Actual {actual.Length} bytes");
			}
		}
	}
}