using System;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// Provides general-purpose utility methods used by cryptographic components and implementations. This includes bit manipulation,
	/// secure random byte generation, and helper functions to ensure compliance with cryptographic constraints such as non-zero padding,
	/// key generation, or exclusion of reserved byte values.
	/// </summary>
	/// <remarks>
	/// <para>
	/// This helper class is intended for internal cryptographic infrastructure, test scaffolding, and algorithm development. It supports
	/// efficient and secure primitives such as span-based zero-allocation random generation and bit reflection.
	/// </para>
	/// <para>
	/// All random methods use <see cref="RandomNumberGenerator.Fill(Span{byte})" /> and are optimized for repeatable use in critical paths,
	/// including optional inlining for performance.
	/// </para>
	/// </remarks>
	public static partial class CryptoUtilities
	{
	}
}