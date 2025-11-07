using Robin.Contracts.Nodes;
using System.Collections.Immutable;

namespace Robin.Internals;

internal sealed class PartialExtractor : INodeVisitor<Dictionary<string, ImmutableArray<INode>>, Dictionary<string, ImmutableArray<INode>>>
{
    public readonly static PartialExtractor Instance = new();
    public Dictionary<string, ImmutableArray<INode>> VisitComment(CommentNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitLineBreak(LineBreakNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitPartialDefine(PartialDefineNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        if (args.ContainsKey(node.PartialName))
            args.Remove(node.PartialName);
        args.Add(node.PartialName, node.Children);
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitPartialCall(PartialCallNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitSection(SectionNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitText(TextNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }

    public Dictionary<string, ImmutableArray<INode>> VisitVariable(VariableNode node, Dictionary<string, ImmutableArray<INode>> args)
    {
        return args;
    }
}
