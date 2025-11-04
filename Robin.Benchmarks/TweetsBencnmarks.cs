using BenchmarkDotNet.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Perfolizer;
using Robin.Abstractions;
using Robin.Abstractions.Extensions;
using Robin.Contracts.Nodes;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Robin.Benchmarks;


[MemoryDiagnoser]
[MarkdownExporter]
public class TweetsBencnmarks
{
    private readonly IServiceProvider serviceProvider;
    private readonly Tweet[] tweets;
    private readonly ImmutableArray<INode> template;
    private readonly IStringRenderer renderer;
    public TweetsBencnmarks()
    {


        ServiceCollection services = [];
        services
            .AddServiceEvaluator()
            .AddStringRenderer()
            .AddMemberAccessor<Tweet>(TweetAccessor.TryGetPropertyValue);
        serviceProvider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });
        string path = Path.Combine(AppContext.BaseDirectory, "datasets", "tweets.json");
        string json = File.ReadAllText(path);
        tweets = JsonSerializer.Deserialize<Tweet[]>(json)!;
        template = TweetsTemplates.List.AsSpan().Parse();
        renderer = serviceProvider.GetRequiredService<IStringRenderer>();

    }

    [Benchmark]
    public string RenderTweets() => renderer.Render(template, tweets);

}
