// // ---------------------------------------------------------------------------------------------------------------
// // <copyright file="CalendarQuarterDefinition.cs" company="PlaceholderCompany">
// //     Copyright (c) PlaceholderCompany. All rights reserved.
// // </copyright>
// // ---------------------------------------------------------------------------------------------------------------

namespace Bodu.Extensions
{
	/// <summary>
	/// Specifies how quarters (Q1–Q4) are defined for a given calendar or financial year system.
	/// </summary>
	/// <remarks>
	/// This enumeration supports both standard calendar-based and regional fiscal year definitions, such as the Australian, US Federal, and
	/// ISO calendar conventions. Use <see cref="Custom" /> to define a custom quarterly system (e.g., 5–4–4 or 13-week accounting periods).
	/// </remarks>
	public enum CalendarQuarterDefinition
	{
		/// <summary>
		/// Indicates that the year is divided into four quarters beginning in January.
		/// <para>Q1 = Jan–Mar, Q2 = Apr–Jun, Q3 = Jul–Sep, Q4 = Oct–Dec.</para>
		/// </summary>
		CalendarYear = 0,

		/// <summary>
		/// Indicates that the fiscal year begins in July.
		/// <para>Q1 = Jul–Sep, Q2 = Oct–Dec, Q3 = Jan–Mar, Q4 = Apr–Jun.</para>
		/// Common in Australia and New Zealand.
		/// </summary>
		FinancialJuly = 6,

		/// <summary>
		/// Indicates that the fiscal year begins in April.
		/// <para>Q1 = Apr–Jun, Q2 = Jul–Sep, Q3 = Oct–Dec, Q4 = Jan–Mar.</para>
		/// Common in India, the UK, and parts of Japan and Canada.
		/// </summary>
		FinancialApril = 9,

		/// <summary>
		/// Indicates that the fiscal year begins in October.
		/// <para>Q1 = Oct–Dec, Q2 = Jan–Mar, Q3 = Apr–Jun, Q4 = Jul–Sep.</para>
		/// Used by the US Federal Government.
		/// </summary>
		FinancialOctober = 3,

		/// <summary>
		/// Indicates that the fiscal year begins in February.
		/// <para>Q1 = Feb–Apr, Q2 = May–Jul, Q3 = Aug–Oct, Q4 = Nov–Jan.</para>
		/// Common in retail and some fiscal 4-4-5 accounting practices.
		/// </summary>
		FinancialFebruary = 11,

		/// <summary>
		/// Indicates that the quarter system is defined by a custom rule. Requires external logic to determine quarter boundaries.
		/// </summary>
		Custom = 99
	}
}
