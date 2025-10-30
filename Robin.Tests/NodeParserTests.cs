using Robin.Expressions;
using Robin.Nodes;
using System.Collections.Immutable;

namespace Robin.tests;

public class NodeParserTests
{

    [Fact]
    public void EmptyTemplate()
    {
        ReadOnlySpan<char> source = "".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        Assert.Empty(nodes);
    }

    [Fact]
    public void SimpleText()
    {
        ReadOnlySpan<char> source = "text".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        TextNode textNode = Assert.IsType<TextNode>(node);
        Assert.Equal("text", textNode.Text);
    }

    [Fact]
    public void SimpleVariable()
    {
        ReadOnlySpan<char> source = "{{text}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        VariableNode text = Assert.IsType<VariableNode>(node);
        Assert.NotNull(text.Expression);
        IdentifierNode id = Assert.IsType<IdentifierNode>(text.Expression);
        Assert.Equal("text", id.Path);
    }
    
    [Fact]
    public void EmptySection()
    {
        ReadOnlySpan<char> source = "{{#sec}}{{/sec}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        SectionNode section = Assert.IsType<SectionNode>(node);
        Assert.False(section.Inverted);
        Assert.Empty(section.Children);
        Assert.NotNull(section.Expression);
        IdentifierNode id = Assert.IsType<IdentifierNode>(section.Expression);
        Assert.Equal("sec", id.Path);
    }

    [Fact]
    public void EmptyInvertedSection()
    {
        ReadOnlySpan<char> source = "{{^inv}}{{/inv}}".AsSpan();
        ImmutableArray<INode> nodes = source.Parse();
        INode node = Assert.Single(nodes);
        SectionNode section = Assert.IsType<SectionNode>(node);
        Assert.True(section.Inverted);
        Assert.Empty(section.Children);
        Assert.NotNull(section.Expression);
        IdentifierNode id = Assert.IsType<IdentifierNode>(section.Expression);
        Assert.Equal("inv", id.Path);
    }
}
