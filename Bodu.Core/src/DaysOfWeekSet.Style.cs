// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Style.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Specifies parsing or formatting styles for a <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <remarks>
		/// <para>Styles are used internally during parsing and formatting to control the interpretation of input and output strings.</para>
		/// <para>Flags may be combined using bitwise operations.</para>
		/// </remarks>
		[Flags]
		private enum Style
		{
			/// <summary>
			/// Unknown or unspecified format.
			/// </summary>
			Unknown = 0x30,

			/// <summary>
			/// Days are ordered from Sunday to Saturday.
			/// </summary>
			SundayToSaturday = 0x01,

			/// <summary>
			/// Days are ordered from Monday to Sunday.
			/// </summary>
			MondayToSunday = 0x02,

			/// <summary>
			/// Days are represented using name characters (e.g., SMTWTFS).
			/// </summary>
			Names = 0x10,

			/// <summary>
			/// Days are represented using binary digits (e.g., 1111100).
			/// </summary>
			Binary = 0x20,
		}
	}
}