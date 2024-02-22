using Bogus;
using Microsoft.Extensions.Logging;
using SampleImplementations.Enrichment;
using SampleImplementations.Models;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Serilog.Formatting.Json;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Tests;

public class LogEnrichmentTests {
	private readonly Faker faker = new ();
	private readonly CustomLogEnricherSource source = new();
	private ILogger logger = null!;

	public LogEnrichmentTests() {
		var formatter = new JsonFormatter();
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.File(formatter, "logs.json", LogEventLevel.Debug, buffered: false)
			.Enrich.With(new CustomLogEnricher(source))
			.Enrich.FromLogContext()
			.CreateLogger();

		using var loggerFactory = LoggerFactory.Create(x => {
			x.ClearProviders();
			x.AddSerilog(Log.Logger);
		});
		logger = loggerFactory.CreateLogger("");

		Randomizer.Seed = new Random(Sample.Seed);
	}
	
	private void WriteLogs() {
		Log.Logger.Information("Test message at {Timestamp}", DateTimeOffset.Now);
	}

	[Fact]
	public void TestLogContext() {
		using var p1 = LogContext.PushProperty("Context 1", Sample.Faker.Generate(), true);
		using var p2 = LogContext.PushProperty("Context 2", Sample.Faker.Generate(), true);
		using var p3 = LogContext.PushProperty("Context 3", Sample.Faker.Generate(), true);
		WriteLogs();
	}

	[Fact]
	public void TestCustomEnricher() {
		source.With("Sample 1", Sample.Faker.Generate(), LogEventLevel.Information);
		source.With("Sample 2", Sample.Faker.Generate(), LogEventLevel.Information);
		source.With("Sample 3", Sample.Faker.Generate(), LogEventLevel.Information);
		WriteLogs();
	}

	[Fact]
	public void MicrosoftLoggingWithRecordEnrichment() {
		using var scope = logger.BeginScope(new List<KeyValuePair<string, object>> {
			new("Scope 1", Sample.Faker.Generate()),
			new("Scope 2", Sample.Faker.Generate()),
			new("Scope 3", Sample.Faker.Generate()),
		});
		WriteLogs();
	}

	[Fact]
	public void MicrosoftLoggingWithClassEnrichment() {
		using var scope = logger.BeginScope(new List<KeyValuePair<string, object>> {
			new("Scope 1", SampleClass.Faker.Generate()),
			new("Scope 2", SampleClass.Faker.Generate()),
			new("Scope 3", SampleClass.Faker.Generate()),
		});
		WriteLogs();
	}
}