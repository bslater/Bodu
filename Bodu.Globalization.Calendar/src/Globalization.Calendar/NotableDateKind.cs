namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Specifies the classification or thematic category of a notable date.
	/// </summary>
	/// <remarks>
	/// The <see cref="NotableDateKind" /> enumeration provides a way to categorize notable dates based on their primary significance, such
	/// as public holidays, cultural events, religious observances, or remembrance days.
	/// </remarks>
	public enum NotableDateKind
	{
		/// <summary>
		/// No specific classification is assigned to the date.
		/// </summary>
		None,

		/// <summary>
		/// A public holiday, typically recognized with an official day off work or school.
		/// </summary>
		Holiday,

		/// <summary>
		/// A religious, cultural, or secular observance that may or may not involve public closure.
		/// </summary>
		Observance,

		/// <summary>
		/// A date dedicated to remembering significant historical events, individuals, or groups.
		/// </summary>
		Remembrance,

		/// <summary>
		/// A cultural event or celebration tied to a specific community, ethnicity, or tradition.
		/// </summary>
		Cultural,

		/// <summary>
		/// A notable date that does not fit into a predefined category.
		/// </summary>
		Other,

		/// <summary>
		/// A date significant to Christian religious traditions or calendar events.
		/// </summary>
		Christian
	}
}