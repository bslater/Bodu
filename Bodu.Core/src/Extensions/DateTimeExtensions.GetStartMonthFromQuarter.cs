// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DateTime.Add.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns the 1-based starting month of the specified quarter, based on the given <see cref="QuarterDefinition" />.
		/// </summary>
		/// <param name="definition">The quarter system definition to apply (e.g., <see cref="QuarterDefinition.CalendarYear" />, <see cref="QuarterDefinition.FinancialJuly" />).</param>
		/// <param name="quarter">The quarter number to evaluate (1 to 4).</param>
		/// <returns>An integer between 1 and 12 representing the starting month of the specified quarter.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid value of the <see cref="QuarterDefinition" /> enum, or if it is
		/// <see cref="QuarterDefinition.Custom" />. Use an external provider to support custom definitions. Also thrown if
		/// <paramref name="quarter" /> is not in the range 1 through 4.
		/// </exception>
		/// <remarks>
		/// This method calculates the starting month by reversing the offset of the quarter system specified by the
		/// <paramref name="definition" />. For example, in a July-based financial year ( <see cref="QuarterDefinition.FinancialJuly" />),
		/// Q1 starts in July (month 7), Q2 in October (month 10), etc.
		/// </remarks>
		public static int GetStartMonthFromQuarter(QuarterDefinition definition, int quarter)
		{
			ThrowHelper.ThrowIfNotBetweenInclusive(quarter, 1, 4);
			if (!Enum.IsDefined(typeof(QuarterDefinition), definition) || definition == QuarterDefinition.Custom)
				throw new ArgumentOutOfRangeException(nameof(definition),
					definition == QuarterDefinition.Custom
						? "Custom quarter definitions require an external provider."
						: string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(QuarterDefinition).Name));

			return ((quarter - 1) * 3 - (int)definition + 12) % 12 + 1;
		}
	}
}
