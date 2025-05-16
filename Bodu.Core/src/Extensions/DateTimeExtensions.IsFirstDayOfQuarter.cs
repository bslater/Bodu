// // --------------------------------------------------------------------------------------------------------------- //
// <copyright file="IsFirstDayOfQuarter.cs" company="PlaceholderCompany">
//     // Copyright (c) PlaceholderCompany. All rights reserved. //
// </copyright>
// // ---------------------------------------------------------------------------------------------------------------

using System;

namespace Bodu.Extensions
{
	public static partial class DateTimeExtensions
	{
		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> represents the first day of its calendar quarter, based on the standard
		/// calendar quarter definition (Q1: January–March, Q2: April–June, etc.).
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <returns><see langword="true" /> if <paramref name="dateTime" /> is the first day of its quarter; otherwise, <see langword="false" />.</returns>
		/// <remarks>
		/// <para>
		/// This method compares the date (normalized to midnight) against the start of the quarter that contains
		/// <paramref name="dateTime" />, using <see cref="CalendarQuarterDefinition.JanuaryDecember" />.
		/// </para>
		/// </remarks>
		public static bool IsFirstDayOfQuarter(this DateTime dateTime)
		{
			var (year, quarter) = GetQuarterAndYearFromDate(CalendarQuarterDefinition.JanuaryDecember, referenceDate: dateTime);
			return dateTime.Date.Ticks == ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(CalendarQuarterDefinition.JanuaryDecember));
		}

		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> represents the first day of its calendar quarter, based on a specified <see cref="CalendarQuarterDefinition" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="definition">The quarter definition used to determine the start of the quarter (e.g., <see cref="CalendarQuarterDefinition.AprilToMarch" />).</param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="dateTime" /> is the first day of its quarter based on the given definition;
		/// otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Thrown if <paramref name="definition" /> is not a valid member of the <see cref="CalendarQuarterDefinition" /> enumeration.
		/// </exception>
		/// <exception cref="InvalidOperationException">
		/// Thrown if <paramref name="definition" /> is <see cref="CalendarQuarterDefinition.Custom" />. Use
		/// <see cref="IsFirstDayOfQuarter(DateTime, IQuarterDefinitionProvider)" /> for custom quarter systems.
		/// </exception>
		/// <remarks>
		/// <para>
		/// The evaluation is based on the first day of the quarter determined by the specified <paramref name="definition" />. The
		/// comparison is made using <see cref="DateTime.Date" /> to ensure the time component is ignored.
		/// </para>
		/// </remarks>
		public static bool IsFirstDayOfQuarter(this DateTime dateTime, CalendarQuarterDefinition definition)
		{
			ThrowHelper.ThrowIfEnumValueIsUndefined(definition);

			if (definition == CalendarQuarterDefinition.Custom)
				throw new InvalidOperationException(
					string.Format(ResourceStrings.Arg_Required_ProviderInterface, nameof(IQuarterDefinitionProvider)));

			var (year, quarter) = GetQuarterAndYearFromDate(definition, referenceDate: dateTime);
			return dateTime.Date.Ticks == ComputeQuarterStartTicks(year, quarter, GetQuarterDefinition(definition));
		}

		/// <summary>
		/// Determines whether the specified <see cref="DateTime" /> represents the first day of its calendar quarter, based on a custom <see cref="IQuarterDefinitionProvider" />.
		/// </summary>
		/// <param name="dateTime">The <see cref="DateTime" /> value to evaluate.</param>
		/// <param name="provider">A custom quarter provider that defines the start of each quarter.</param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="dateTime" /> is the first day of its quarter as defined by
		/// <paramref name="provider" />; otherwise, <see langword="false" />.
		/// </returns>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="provider" /> is <c>null</c>.</exception>
		/// <remarks>
		/// <para>
		/// This method compares the normalized date against the custom-defined quarter start date returned by <paramref name="provider" />.
		/// Time-of-day is ignored in the comparison.
		/// </para>
		/// </remarks>
		public static bool IsFirstDayOfQuarter(this DateTime dateTime, IQuarterDefinitionProvider provider)
		{
			ThrowHelper.ThrowIfNull(provider);

			return dateTime.Date.Ticks == provider.GetQuarterStart(dateTime).Date.Ticks;
		}
	}
}