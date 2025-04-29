// ---------------------------------------------------------------------------------------------------------------
// <copyright file="DaysOfWeekSet.Result.cs" company="PlaceholderCompany">
//     Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>
// ---------------------------------------------------------------------------------------------------------------

namespace Bodu
{
	public partial struct DaysOfWeekSet
	{
		/// <summary>
		/// Represents the result of a parsing operation for <see cref="DaysOfWeekSet" />.
		/// </summary>
		/// <remarks>This struct is used internally to track the parsed value, error conditions, and optional exceptions.</remarks>
		private struct Result
		{
			/// <summary>
			/// The argument name related to the parsing operation (for exception purposes).
			/// </summary>
			internal string ArgumentName;

			/// <summary>
			/// An optional inner exception that occurred during parsing.
			/// </summary>
			internal Exception InnerException;

			/// <summary>
			/// A user-friendly error message describing the parse failure.
			/// </summary>
			internal string Message;

			/// <summary>
			/// The successfully parsed <see cref="DaysOfWeekSet" />, if parsing was successful.
			/// </summary>
			internal DaysOfWeekSet ParsedWeekDays;

			/// <summary>
			/// The type of failure encountered during parsing.
			/// </summary>
			internal ParseFailure ParseFailure;

			/// <summary>
			/// Indicates whether to immediately throw an exception upon failure.
			/// </summary>
			internal bool ThrowOnError;

			/// <summary>
			/// Initializes this result tracker for a new parse operation.
			/// </summary>
			/// <param name="throwOnError">Specifies whether parsing should throw immediately on failure.</param>
			internal void Init(bool throwOnError)
			{
				this.ParsedWeekDays = default;
				this.ParseFailure = ParseFailure.None;
				this.ThrowOnError = throwOnError;
			}

			/// <summary>
			/// Sets a failure state based on a native exception.
			/// </summary>
			/// <param name="exception">The underlying exception that caused the parse to fail.</param>
			internal void SetFailure(Exception exception)
			{
				this.ParseFailure = ParseFailure.NativeException;
				this.InnerException = exception;
			}

			/// <summary>
			/// Sets a failure state based on a known failure type and message.
			/// </summary>
			/// <param name="failure">The type of failure.</param>
			/// <param name="message">An associated failure message.</param>
			internal void SetFailure(ParseFailure failure, string message)
			{
				this.SetFailure(failure, null!, message, null!);
			}

			/// <summary>
			/// Sets a failure state with optional detailed context.
			/// </summary>
			/// <param name="failure">The type of failure.</param>
			/// <param name="argumentName">The name of the argument causing the error, if applicable.</param>
			/// <param name="message">An associated error message.</param>
			/// <param name="innerException">An optional inner exception.</param>
			internal void SetFailure(ParseFailure failure, string argumentName, string message, Exception innerException = null!)
			{
				this.ParseFailure = failure;
				this.ArgumentName = argumentName;
				this.Message = message;
				this.InnerException = innerException;

				if (this.ThrowOnError)
				{
					throw this.GetParseException();
				}
			}

			/// <summary>
			/// Generates an appropriate <see cref="Exception" /> based on the current failure state.
			/// </summary>
			/// <returns>A new exception matching the type of parse failure.</returns>
			internal Exception GetParseException()
			{
				return this.ParseFailure switch
				{
					ParseFailure.ArgumentNull => new ArgumentNullException(this.ArgumentName),
					ParseFailure.FormatWithInnerException => new FormatException(this.Message, this.InnerException),
					ParseFailure.Format => new FormatException(this.Message),
					ParseFailure.NativeException => this.InnerException ?? new FormatException(this.Message),
					_ => new FormatException("The input string was not recognized as a valid DaysOfWeekSet."),
				};
			}
		}
	}
}