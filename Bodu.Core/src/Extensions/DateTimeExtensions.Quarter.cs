// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.Quarter.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

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
			=> Quarter(dateTime, CalendarQuarterDefinition.CalendarYear);

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using the given <see cref="CalendarQuarterDefinition" /> to
		/// determine the fiscal calendar structure.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="definition">The quarter system definition to apply (e.g., <see cref="CalendarQuarterDefinition.CalendarYear" />, <see cref="CalendarQuarterDefinition.FinancialJuly" />).</param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid value of the <see cref="CalendarQuarterDefinition" /> enum, or if it is
		/// <see cref="CalendarQuarterDefinition.Custom" />. Use the <c>Quarter(DateTime, IDateTimeQuarterProvider)</c> overload to support custom definitions.
		/// </exception>
		/// <remarks>
		/// This method supports predefined quarter structures aligned to calendar or financial years. The result is based on adjusting the
		/// input month by the definition offset and mapping the result to a 1-based quarter.
		/// </remarks>
		public static int Quarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			if (!Enum.IsDefined(typeof(CalendarQuarterDefinition), definition) || definition == CalendarQuarterDefinition.Custom)
				throw new ArgumentOutOfRangeException(nameof(definition),
					definition == CalendarQuarterDefinition.Custom
						? "Custom quarter definitions require an external provider."
						: string.Format(ResourceStrings.Arg_Invalid_EnumValue, typeof(CalendarQuarterDefinition).Name));

			int offset = (int)definition;
			return ((dateTime.Month + offset - 1) % 12) / 3 + 1;
		}

		/// <summary>
		/// Returns the quarter number (1–4) for the specified <see cref="DateTime" />, using a custom <see cref="IQuarterDefinitionProvider" /> implementation.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="provider">An implementation of <see cref="IQuarterDefinitionProvider" /> that determines the quarter based on custom logic.</param>
		/// <returns>An integer between 1 and 4 representing the quarter that includes the specified <paramref name="dateTime" />.</returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the quarter value returned by the provider is outside the valid range of 1 through 4.
		/// </exception>
		/// <remarks>
		/// Use this overload to support advanced or non-standard fiscal calendars, such as 4-4-5 financial periods, retail accounting
		/// calendars, or region-specific quarter models not covered by <see cref="CalendarQuarterDefinition" />.
		/// </remarks>
		public static int Quarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			int quarter = provider.GetQuarter(dateTime);

			if (quarter is < 1 or > 4)
				throw new ArgumentOutOfRangeException(nameof(provider), ResourceStrings.Arg_OutOfRange_InvalidQuarterNumber);

			return quarter;
		}
	}
}
