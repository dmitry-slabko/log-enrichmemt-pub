using BenchmarkDotNet.Attributes;
using Bogus;
using Microsoft.Extensions.Logging;
using SampleImplementations.Enrichment;
using SampleImplementations.Models;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace Benchmark;

[MemoryDiagnoser]
public class LogEnrichment {
	private readonly Faker faker = new ();
	private readonly CustomLogEnricherSource source = new();
	private ILogger logger = null!;
	private ILoggerFactory loggerFactory = null!;

	[Params(20)]
	public int Count { get; set; }

	[GlobalSetup]
	public void Setup() {
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Debug()
			.Enrich.With(new CustomLogEnricher(source))
			.Enrich.FromLogContext()
			.CreateLogger();

		loggerFactory = LoggerFactory.Create(x => {
			x.ClearProviders();
			x.AddSerilog(Log.Logger);
		});
		logger = loggerFactory.CreateLogger("");

		Randomizer.Seed = new Random(Sample.Seed);
	}

	[GlobalCleanup]
	public void Cleanup() {
		Log.CloseAndFlush();
		loggerFactory.Dispose();
	}

	private void WriteLogs() {
		for (var i = 0; i < Count; i++) {
			Log.Logger.Information("Logging a {i} message", i + 1);
		}
	}

	[Benchmark]
	public void SerilogLoggingWithContext() {
		using var p1 = LogContext.PushProperty("Name 1", faker.Name.FullName());
		using var p2 = LogContext.PushProperty("Name 2", faker.Name.FullName());
		using var p3 = LogContext.PushProperty("Name 3", faker.Name.FullName());
		WriteLogs();
	}
	
	[Benchmark]
	public void SerilogLoggingWithEnrichment() {
		source.With("Name 1", faker.Name.FullName(), LogEventLevel.Information);
		source.With("Name 2", faker.Name.FullName(), LogEventLevel.Information);
		source.With("Name 3", faker.Name.FullName(), LogEventLevel.Information);
		WriteLogs();
	}
	
	[Benchmark]
	public void MicrosoftLoggingWithEnrichment() {
		using var scope = logger.BeginScope(new List<KeyValuePair<string, object>> {
			new("Name 1", faker.Name.FullName()),
			new("Name 2", faker.Name.FullName()),
			new("Name 3", faker.Name.FullName()),
		});
		WriteLogs();
	}

	[Benchmark]
	public void SerilogLoggingWithObjectContext() {
		using var p1 = LogContext.PushProperty("Contxt 1", Sample.Faker.Generate(), true);
		using var p2 = LogContext.PushProperty("Contxt 2", Sample.Faker.Generate(), true);
		using var p3 = LogContext.PushProperty("Contxt 3", Sample.Faker.Generate(), true);
		WriteLogs();
	}
	
	[Benchmark]
	public void SerilogLoggingWithObjectEnrichment() {
		source.With("Sample 1", Sample.Faker.Generate(), LogEventLevel.Information);
		source.With("Sample 2", Sample.Faker.Generate(), LogEventLevel.Information);
		source.With("Sample 3", Sample.Faker.Generate(), LogEventLevel.Information);
		WriteLogs();
	}

	[Benchmark]
	public void MicrosoftLoggingWithObjectEnrichment() {
		using var scope = logger.BeginScope(new List<KeyValuePair<string, object>> {
			new("Sample 1", Sample.Faker.Generate()),
			new("Sample 2", Sample.Faker.Generate()),
			new("Sample 3", Sample.Faker.Generate()),
		});
		WriteLogs();
	}
	
	[Benchmark]
	public void SerilogLoggingNoEnrichment() {
		WriteLogs();
	}
	
	[Benchmark]
	public void MicrosoftLoggingNoEnrichment() {
		WriteLogs();
	}
}