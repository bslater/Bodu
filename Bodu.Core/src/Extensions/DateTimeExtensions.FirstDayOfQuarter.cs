// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="DateTimeExtensions.FirstDayOfQuarter.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
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
		/// <remarks>
		/// This overload supports different quarter definitions such as the Calendar Year and various Financial Year systems, as defined by
		/// the <see cref="CalendarQuarterDefinition" /> enumeration.
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
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
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if the <paramref name="provider" /> returns an invalid quarter number or month (outside the range 1–12).
		/// </exception>
		/// <remarks>
		/// This method supports advanced or domain-specific definitions of quarters by delegating logic to the specified
		/// <paramref name="provider" />, such as 4-4-5 fiscal calendars or regional fiscal systems.
		/// </remarks>
		public static DateTime FirstDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			int startMonth = provider.GetStartMonthFromQuarter(Quarter(dateTime, provider));
			if (startMonth is < 1 or > 12)
				throw new ArgumentOutOfRangeException(nameof(provider), ResourceStrings.Arg_OutOfRange_InvalidMonthNumber);

			int year = dateTime.Month >= startMonth ? dateTime.Year : dateTime.Year - 1;

			return new DateTime(GetTicksForDate(year, startMonth, 1), dateTime.Kind);
		}
	}
}
