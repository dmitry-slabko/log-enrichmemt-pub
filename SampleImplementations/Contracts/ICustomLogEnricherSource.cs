using SampleImplementations.Enrichment.Models;
using Serilog.Events;

namespace SampleImplementations.Contracts; 

/// <summary>
/// Allows to enrich logs with custom information
/// </summary>
public interface ICustomLogEnricherSource {
	ICustomLogEnricherSource With<T>(string property, T? value, LogEventLevel logLevel) where T : class;

	void Remove(string property);

	IEnumerable<CustomLogEventProperty> GetCustomProperties(LogEventLevel logLevel);
}