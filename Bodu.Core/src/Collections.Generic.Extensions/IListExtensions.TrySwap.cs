// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IListExtensions.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Collections.Generic.Extensions
{
	public static partial class IListExtensions
	{
		/// <summary>
		/// Attempts to swap the elements at the specified indexes in the list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to modify.</param>
		/// <param name="indexA">The index of the first item to swap.</param>
		/// <param name="indexB">The index of the second item to swap.</param>
		/// <returns><see langword="true" /> if the swap succeeded; otherwise, <see langword="false" /> (e.g., invalid indexes).</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="list" /> is <see langword="null" />.</exception>
		public static bool TrySwap<T>(this IList<T> list, int indexA, int indexB)
		{
			ThrowHelper.ThrowIfNull(list);

			if (indexA < 0 || indexB < 0 || indexA >= list.Count || indexB >= list.Count)
				return false;

			if (indexA != indexB)
			{
				T temp = list[indexA];
				list[indexA] = list[indexB];
				list[indexB] = temp;
			}

			return true;
		}
	}
}