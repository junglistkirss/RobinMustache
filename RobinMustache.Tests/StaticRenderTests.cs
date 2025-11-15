using RobinMustache.Abstractions.Nodes;
using RobinMustache.Helpers;
using System.Collections.Immutable;

namespace RobinMustache.tests;

public class StaticRenderTests
{
    public IStringRenderer StringRenderer { get; private set; } = default!;

    public StaticRenderTests()
    {
        StringHelpers.AsGlobalHelpers();
        StringRenderer = Renderer.CreateStringRenderer(accessors: x =>
        {
            x.CreateMemberObjectAccessor<TestSample>(static (TestSample sample, string member, out object? value) =>
            {
                value = member switch
                {
                    "Name" => sample.Name,
                    "Age" => sample.Age,
                    _ => throw new InvalidDataException($"Member does not exists : {member}"),
                };
                return true;
            });
        });
    }

    [Fact]
    public void Test_Render_SimpleTemplate()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Name: {{ Name }}, Age: {{ Age }}".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Name: Alice, Age: 30", result);
    }

    [Fact]
    public void Test_Render_StandaloneTagRemoval()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "{{#.}}\r\nBonjour\r\n{{/.}}".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal($"Bonjour{Environment.NewLine}", result);
    }
    [Fact]
    public void Test_Render_StandaloneTagRemovalInverted()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Hello\r\n{{^.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Hello\r\nBye", result);
    }

    [Fact]
    public void Test_Render_StandaloneTagRemovalFalsy()
    {
        ImmutableArray<INode> template = "Hello\r\n{{#.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, null);
        Assert.Equal("Hello\r\nBye", result);
    }
    [Fact]
    public void Test_Render_StandaloneTagRemovalSection()
    {
        TestSample sample = new() { Name = "Alice", Age = 30 };
        ImmutableArray<INode> template = "Hello\r\n{{#.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, sample);
        Assert.Equal("Hello\r\nDetails:\r\n  - Name: Alice\r\n  - Age: 30\r\nBye", result);
    }

    [Fact]
    public void Test_Render_StandaloneTagRemovalSectionIvertedFalsy()
    {
        ImmutableArray<INode> template = "Hello\r\n{{^.}}\r\nDetails:\r\n  - Name: {{&Name}}\r\n  - Age: {{&Age}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, null);
        Assert.Equal("Hello\r\nDetails:\r\n  - Name: \r\n  - Age: \r\nBye", result);
    }


    [Fact]
    public void Test_RenderCollection_StandaloneTagRemovalInverted()
    {
        string[] samples = ["Bob", "Alice"];
        ImmutableArray<INode> template = "Hello\r\n{{^.}}\r\n  - Name: {{&.}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, samples);
        Assert.Equal("Hello\r\nBye", result);
    }

    [Fact]
    public void Test_RenderCollection_StandaloneTagRemovalFalsy()
    {
        ImmutableArray<INode> template = "Hello\r\n{{#.}}\r\n  - Name: {{&.}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, null);
        Assert.Equal("Hello\r\nBye", result);
    }
    [Fact]
    public void Test_RenderCollection_StandaloneTagRemovalSection()
    {
        string[] samples = ["Bob", "Alice"];
        ImmutableArray<INode> template = "Hello\r\n{{#.}}\r\n  - Name: {{&.}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, samples);
        Assert.Equal("Hello\r\n  - Name: Bob\r\n  - Name: Alice\r\nBye", result);
    }

    [Fact]
    public void Test_RenderCollection_StandaloneTagRemovalSectionIvertedFalsy()
    {
        ImmutableArray<INode> template = "Hello\r\n{{^.}}\r\n  - Name: {{&.}}\r\n{{/.}}\r\nBye".AsSpan().Parse();
        string result = StringRenderer.Render(template, null);
        Assert.Equal("Hello\r\n  - Name: \r\nBye", result);
    }
}
