using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu
{
	/// <summary>
	/// Defines a pluggable interface for random number generation.
	/// </summary>
	public interface IRandomGenerator
	{
		/// <summary>
		/// Returns a non-negative random integer that is less than the specified maximum.
		/// </summary>
		/// <param name="maxValue">The exclusive upper bound.</param>
		/// <returns>A random integer in the range [0, maxValue].</returns>
		int Next(int maxValue);
	}
}