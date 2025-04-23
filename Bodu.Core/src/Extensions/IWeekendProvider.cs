using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bodu.Extensions
{
	/// <summary>
	/// Defines a provider interface for determining whether a given <see cref="DayOfWeek" /> is considered a weekend day.
	/// </summary>
	/// <remarks>
	/// Implement this interface to supply custom weekend definitions that are not covered by built-in <see cref="StandardWeekend" /> options.
	/// </remarks>
	public interface IWeekendProvider
	{
		/// <summary>
		/// Determines whether the specified <see cref="DayOfWeek" /> is considered a weekend day.
		/// </summary>
		/// <param name="dayOfWeek">The day of the week to evaluate.</param>
		/// <returns><see langword="true" /> if the day is considered part of the weekend; otherwise, <see langword="false" />.</returns>
		bool IsWeekend(DayOfWeek dayOfWeek);
	}
}
