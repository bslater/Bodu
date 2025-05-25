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
		/// Attempts to move an item from one index to another in the list.
		/// </summary>
		/// <typeparam name="T">The type of elements in the list.</typeparam>
		/// <param name="list">The list to modify.</param>
		/// <param name="oldIndex">The original index of the item.</param>
		/// <param name="newIndex">The target index to move the item to.</param>
		/// <returns><see langword="true" /> if the move succeeded; otherwise, <see langword="false" /> (e.g., invalid indexes).</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="list" /> is <see langword="null" />.</exception>
		public static bool TryMove<T>(this IList<T> list, int oldIndex, int newIndex)
		{
			ThrowHelper.ThrowIfNull(list);

			if (oldIndex < 0 || newIndex < 0 || oldIndex >= list.Count || newIndex > list.Count)
				return false;

			if (oldIndex == newIndex || oldIndex == newIndex - 1)
				return true;

			T item = list[oldIndex];
			list.RemoveAt(oldIndex);
			if (newIndex > oldIndex)
				newIndex--;

			list.Insert(newIndex, item);
			return true;
		}
	}
}