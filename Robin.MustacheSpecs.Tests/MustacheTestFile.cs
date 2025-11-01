using Robin.MustacheSpecs.Tests;
using System.Text.Json.Serialization;

namespace Robin.tests;

public sealed record MustacheTestFile(
    [property: JsonPropertyName("overview")] string Overview,
    [property: JsonPropertyName("tests")] IReadOnlyList<MustacheTestCase> Tests
);
