using SampleImplementations.Contracts;
using SampleImplementations.Enrichment.Models;
using Serilog.Events;

namespace SampleImplementations.Enrichment; 

public sealed class CustomLogEnricherSource : ICustomLogEnricherSource {
	private readonly Dictionary<string, CustomLogEventProperty> properties = new();

	public ICustomLogEnricherSource With<T>(string property, T? value, LogEventLevel logLevel) where T : class {
		if (value is null) {
			return this;
		}

		properties[property] = new CustomLogEventProperty(property, value, logLevel);

		return this;
	}

	public void Remove(string property) {
		properties.Remove(property);
	}

	public IEnumerable<CustomLogEventProperty> GetCustomProperties(LogEventLevel logLevel) {
		foreach (var item in properties.Values) {
			if (item.Level <= logLevel) {
				yield return item;
			}
		}
	}
}