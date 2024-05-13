using Serilog.Core;
using Serilog.Events;

namespace SampleImplementations.Enrichment.Models;

public sealed class CustomLogEventProperty {
	private LogEventProperty? propertyValue;

	public CustomLogEventProperty(string property, object value, LogEventLevel level) {
		Name = property;
		Value = value;
		Level = level;
	}

	public string Name { get; }

	public object Value { get; }

	public LogEventLevel Level { get; }

	public LogEventProperty ProduceProperty(ILogEventPropertyFactory propertyFactory) {
		propertyValue ??= propertyFactory.CreateProperty(Name, Value, destructureObjects: true);
		return propertyValue;
	}
}