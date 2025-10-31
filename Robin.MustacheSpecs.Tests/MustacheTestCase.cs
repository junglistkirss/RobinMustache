using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace Robin.tests;

public sealed record MustacheTestCase(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("desc")] string Description,
    [property: JsonPropertyName("data")] JsonObject Data,
    [property: JsonPropertyName("template")] string Template,
    [property: JsonPropertyName("expected")] string Expected
);
