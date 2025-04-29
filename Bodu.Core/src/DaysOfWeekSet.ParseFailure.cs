// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.ParseFailure.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Represents the reason why parsing a <see cref="DaysOfWeekSet" /> might fail.
		/// </summary>
		/// <remarks>This enumeration is used internally by the parsing logic to classify error conditions.</remarks>
		private enum ParseFailure
		{
			/// <summary>
			/// No failure occurred.
			/// </summary>
			None,

			/// <summary>
			/// The provided argument was <c>null</c>.
			/// </summary>
			ArgumentNull,

			/// <summary>
			/// The input format was invalid or unrecognized.
			/// </summary>
			Format,

			/// <summary>
			/// An underlying exception occurred during parsing.
			/// </summary>
			NativeException,

			/// <summary>
			/// A format failure occurred, with an associated inner exception.
			/// </summary>
			FormatWithInnerException,
		}
	}
}