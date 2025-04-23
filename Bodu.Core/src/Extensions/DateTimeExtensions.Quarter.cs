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
		/// Returns the quarter number (1–4) of the year for the specified <see cref="DateTime" />, using the standard calendar quarter definition.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that contains the <paramref name="dateTime" />.</returns>
		/// <remarks>
		/// This method uses the standard calendar quarter structure:
		/// <list type="bullet">
		/// <item>
		/// <term>Q1</term>
		/// <description>January–March</description>
		/// </item>
		/// <item>
		/// <term>Q2</term>
		/// <description>April–June</description>
		/// </item>
		/// <item>
		/// <term>Q3</term>
		/// <description>July–September</description>
		/// </item>
		/// <item>
		/// <term>Q4</term>
		/// <description>October–December</description>
		/// </item>
		/// </list>
		/// </remarks>
		public static int Quarter(this DateTime dateTime)
			=> Quarter(dateTime, QuarterDefinition.CalendarYear);

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using the given <see cref="QuarterDefinition" /> to
		/// determine the fiscal calendar structure.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="definition">The quarter system definition to apply (e.g., <see cref="QuarterDefinition.CalendarYear" />, <see cref="QuarterDefinition.FinancialJuly" />).</param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid value of the <see cref="QuarterDefinition" /> enum, or if it is
		/// <see cref="QuarterDefinition.Custom" />. Use the <c>Quarter(DateTime, IQuarterProvider)</c> overload to support custom definitions.
		/// </exception>
		/// <remarks>
		/// This method supports predefined quarter structures aligned to calendar or financial years. The result is based on adjusting the
		/// input month by the definition offset and mapping the result to a 1-based quarter.
		/// </remarks>
		public static int Quarter(this DateTime dateTime, QuarterDefinition definition)
		{
			if (!Enum.IsDefined(typeof(QuarterDefinition), definition) || definition == QuarterDefinition.Custom)
				throw new ArgumentOutOfRangeException(nameof(definition),
					definition == QuarterDefinition.Custom
						? "Custom quarter definitions require an external provider."
						: string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(QuarterDefinition).Name));

			int offset = (int)definition;
			return ((dateTime.Month + offset - 1) % 12) / 3 + 1;
		}

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using a custom <see cref="IQuarterProvider" /> implementation.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="provider">An implementation of <see cref="IQuarterProvider" /> that determines the quarter based on custom logic.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the quarter value returned by the provider is outside the valid range of 1 through 4.
		/// </exception>
		/// <remarks>
		/// Use this overload to support advanced or non-standard fiscal calendars, such as 4-4-5 financial periods, retail accounting
		/// calendars, or region-specific quarter models not covered by <see cref="QuarterDefinition" />.
		/// </remarks>
		public static int Quarter(this DateTime dateTime, IQuarterProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			int quarter = provider.GetQuarter(dateTime);

			if (quarter is < 1 or > 4)
				throw new ArgumentOutOfRangeException(nameof(provider), ResourceStrings.Arg_OutOfRange_InvalidQuarterNumber);

			return quarter;
		}
	}
}
