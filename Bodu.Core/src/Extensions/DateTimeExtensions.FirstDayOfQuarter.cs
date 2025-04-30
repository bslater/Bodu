// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="DateTimeExtensions.FirstDayOfQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Returns a <see cref="DateTime" /> representing the first day of the calendar quarter for the specified <paramref name="dateTime" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose quarter is evaluated.</param>
		/// <returns>
		/// A <see cref="DateTime" /> value representing the first day of the quarter, with the time component set to midnight (00:00:00)
		/// and the original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <remarks>
		/// This method uses the standard calendar quarter definition:
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
		public static DateTime FirstDayOfQuarter(this DateTime dateTime)
			=> FirstDayOfQuarter(dateTime, CalendarQuarterDefinition.CalendarYear);

		/// <summary>
		/// Returns the first day of the quarter for the given <paramref name="dateTime" />, using the specified <see cref="CalendarQuarterDefinition" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> whose quarter is evaluated.</param>
		/// <param name="definition">The <see cref="CalendarQuarterDefinition" /> that defines the quarter structure to use.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first day of the identified quarter, with the time component set to 00:00:00 and the
		/// original <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a defined value of the <see cref="CalendarQuarterDefinition" /> enumeration.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />, which requires an external
		/// provider. Use the <see cref="FirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> overload instead.
		/// </exception>
		/// <remarks>
		/// This overload supports predefined calendar and financial quarter systems only. Use the <c>Custom</c> value with a provider-based
		/// overload for non-standard fiscal systems.
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			int month = GetStartMonthFromQuarter(definition, dateTime.Quarter(definition));
			int year = dateTime.Year;
			if (month > dateTime.Month)
				year--;

			return new DateTime(DateTimeExtensions.GetTicksForDate(year, month, 1), dateTime.Kind);
		}

		/// <summary>
		/// Returns the first day of the quarter based on a custom <see cref="IQuarterDefinitionProvider" /> implementation.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value whose quarter is being evaluated.</param>
		/// <param name="provider">The <see cref="IQuarterDefinitionProvider" /> that defines custom quarter logic.</param>
		/// <returns>
		/// A <see cref="DateTime" /> representing the first calendar day of the applicable custom quarter, with the time set to midnight
		/// (00:00:00) and the <see cref="DateTime.Kind" /> preserved.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <see langword="null" />.</exception>
		/// <exception cref="ArgumentOutOfRangeException">Thrown if the <paramref name="provider" /> returns an invalid quarter boundary.</exception>
		/// <remarks>
		/// This method supports advanced or domain-specific definitions of quarters by delegating logic to the specified
		/// <paramref name="provider" />, such as 4-4-5 fiscal calendars or regional fiscal systems.
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);
			return provider.GetStartDate(dateTime);
		}
	}
}