using SampleImplementations.Contracts;
using Serilog.Core;
using Serilog.Events;

namespace SampleImplementations.Enrichment; 

public sealed class CustomLogEnricher : ILogEventEnricher {
	private readonly ICustomLogEnricherSource source;

	public CustomLogEnricher(ICustomLogEnricherSource source) {
		this.source = source;
	}

	public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory) {
		var loggedProperties = source.GetCustomProperties(logEvent.Level);
		foreach (var item in loggedProperties) {
			var property = item.ProduceProperty(propertyFactory);
			logEvent.AddOrUpdateProperty(property);
		}
	}
}