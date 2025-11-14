using RobinMustache.Abstractions.Expressions;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public sealed class SectionNode(IExpressionNode expression, ImmutableArray<INode> children, LineBreakNode? trailingBreak, bool inverted) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public ImmutableArray<INode> Children { get; } = children;
    public LineBreakNode? TrailingBreak { get; } = trailingBreak;
    public bool Inverted { get; } = inverted;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitSection(this, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitSection(this, args);
    }
}

