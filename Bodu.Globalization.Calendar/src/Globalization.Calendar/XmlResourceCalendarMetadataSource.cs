using System.Reflection;

namespace Bodu.Globalization.Calendar
{
	/// <summary>
	/// Provides a metadata source that loads notable calendar dates from an embedded XML resource.
	/// </summary>
	/// <remarks>The loaded XML is validated and parsed internally upon first access. Subsequent calls return cached results.</remarks>
	public sealed class XmlResourceCalendarMetadataSource
		: INotableDateDefinitionProvider
	{
		private readonly string _xmlResourceName;
		private readonly Assembly _assembly;
		private readonly Lazy<List<NotableDateDefinition>> _definitions;

		/// <summary>
		/// Initializes a new instance of the <see cref="XmlResourceCalendarMetadataSource" /> class.
		/// </summary>
		/// <param name="xmlResourceName">The full resource name of the embedded XML document containing notable dates.</param>
		/// <param name="assembly">The assembly containing the embedded resource. Defaults to the executing assembly.</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="xmlResourceName" /> is <c>null</c>.</exception>
		public XmlResourceCalendarMetadataSource(string xmlResourceName, Assembly? assembly = null)
		{
			_xmlResourceName = xmlResourceName ?? throw new ArgumentNullException(nameof(xmlResourceName));
			_assembly = assembly ?? Assembly.GetExecutingAssembly();
			_definitions = new Lazy<List<NotableDateDefinition>>(LoadAndParseDefinitions, isThreadSafe: true);
		}

		/// <inheritdoc />
		public IEnumerable<NotableDateDefinition> LoadDefinitions() => _definitions.Value;

		/// <summary>
		/// Loads and parses the embedded XML resource into notable date definitions.
		/// </summary>
		/// <returns>A list of parsed <see cref="NotableDateDefinition" /> instances.</returns>
		/// <exception cref="FileNotFoundException">Thrown if the embedded XML resource is not found.</exception>
		private List<NotableDateDefinition> LoadAndParseDefinitions()
		{
			using var xmlStream = _assembly.GetManifestResourceStream(_xmlResourceName)
				?? throw new FileNotFoundException($"Embedded XML resource '{_xmlResourceName}' was not found in assembly '{_assembly.FullName}'.");

			using var reader = new StreamReader(xmlStream);
			var xmlContent = reader.ReadToEnd();

			return NotableDateDefinitionParser.ParseXml(xmlContent);
		}
	}
}