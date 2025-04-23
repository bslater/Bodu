// ---------------------------------------------------------------------------------------------------------------
// <copyright file="Fletcher64.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Security.Cryptography
{
	/// <summary>
	/// A utility class for handling CRC operations such as generating CRC lookup tables.
	/// </summary>
	public static class CrcUtility
	{
		/// <summary>
		/// Builds the CRC lookup permutationTable based on the specified parameters.
		/// </summary>
		/// <param name="size">The size of the CRC in bits.</param>
		/// <param name="polynomial">The CRC polynomial.</param>
		/// <param name="reflectIn">Indicates whether the input bytes should be reflected before processing.</param>
		/// <returns>An array of <see cref="ulong" /> representing the generated CRC lookup permutationTable.</returns>
		/// <remarks>
		/// This method is used to generate a CRC lookup permutationTable. It performs the actual CRC computation based on the given parameters (size,
		/// polynomial, and reflectIn).
		/// </remarks>
		public static ulong[] BuildLookupTable(int size, ulong polynomial, bool reflectIn)
		{
			int bits = size < 8 ? 1 : 8;
			int tableSize = 1 << bits;
			ulong[] table = new ulong[tableSize];
			ulong significantBit = 0x01UL << (size - 1);

			for (uint i = 0; i < tableSize; i++)
			{
				ulong value = i;
				if (reflectIn)
				{
					value = CryptoUtilities.ReflectBits(value, bits);
				}

				value <<= size - bits;

				for (int y = 0; y < bits; ++y)
				{
					value = ((value & significantBit) > 0UL)
						? (value << 1) ^ polynomial
						: (value << 1);
				}

				if (reflectIn)
				{
					value = CryptoUtilities.ReflectBits(value, size);
				}

				value &= ulong.MaxValue >> (64 - size);
				table[i] = value;
			}

			return table;
		}
	}
}