using Bogus;

namespace SampleImplementations.Models;

public record Sample {
	public const int Seed = 8675309;

	public Guid Id { get; set; }

	public string Name { get; set; } = null!;

	public int Size { get; set; }

	public double Value { get; set; }

	public static readonly Faker<Sample> Faker = new Faker<Sample>()
		.UseSeed(Seed)
		.StrictMode(true)
		.RuleFor(x => x.Value, f => f.Random.Double())
		.RuleFor(x => x.Id, f => f.Random.Guid())
		.RuleFor(x => x.Size, f => f.Random.Int())
		.RuleFor(x => x.Name, f => f.Name.FullName());
}

public sealed class SampleClass {
	public Guid Id { get; set; }

	public string Name { get; set; } = null!;

	public int Size { get; set; }

	public double Value { get; set; }

	public static readonly Faker<SampleClass> Faker = new Faker<SampleClass>()
		.UseSeed(Sample.Seed)
		.StrictMode(true)
		.RuleFor(x => x.Value, f => f.Random.Double())
		.RuleFor(x => x.Id, f => f.Random.Guid())
		.RuleFor(x => x.Size, f => f.Random.Int())
		.RuleFor(x => x.Name, f => f.Name.FullName());
}