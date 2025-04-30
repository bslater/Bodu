namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Represents a source of structured metadata for notable calendar dates.
	/// </summary>
	public interface INotableDateDefinitionProvider
	{
		/// <summary>
		/// Loads all <see cref="NotableDateDefinition" /> records from the underlying source.
		/// </summary>
		/// <returns>A collection of notable date definitions.</returns>
		/// <exception cref="System.Exception">Thrown when the metadata cannot be loaded or is invalid.</exception>
		IEnumerable<NotableDateDefinition> LoadDefinitions();
	}
}
