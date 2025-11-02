using Robin.Abstractions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin;

internal class PartialExtractor : INodeVisitor<ImmutableDictionary<string, ImmutableArray<INode>>, NoValue>
{
    public ImmutableDictionary<string, ImmutableArray<INode>> VisitComment(CommentNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitLineBreak(LineBreakNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
        throw new NotImplementedException();
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartial(PartialDefineNode node, NoValue args)
    {
        ImmutableDictionary<string, ImmutableArray<INode>>.Builder builder = ImmutableDictionary.CreateBuilder<string, ImmutableArray<INode>>();
        //foreach (var item in collection)
        {

        }
        return builder.ToImmutable();

    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitPartialCall(PartialCallNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitSection(SectionNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitText(TextNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    }

    public ImmutableDictionary<string, ImmutableArray<INode>> VisitVariable(VariableNode node, NoValue args)
    {
        return ImmutableDictionary<string, ImmutableArray<INode>>.Empty;
    }
}

public static class Renderer
{
    public static string Render(this INodeVisitor<NoValue, RenderContext> visitor, IEvaluator evaluator, ImmutableArray<INode> template, object? data)
    {
        RenderContext ctx = new()
        {
            Data = new DataContext(data, null),
            Evaluator = evaluator,
            Builder = new StringBuilder()
        };
        ImmutableArray<INode>.Enumerator enumerator = template.GetEnumerator();
        while (enumerator.MoveNext())
        {
            _ = enumerator.Current.Accept(visitor, ctx);
        }
        return ctx.Builder.ToString();
    }
}
