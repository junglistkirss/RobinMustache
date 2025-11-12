using System.Runtime.CompilerServices;

namespace RobinMustache.Abstractions.Nodes;

public sealed class CommentNode(string message, bool isStandalone) : INode
{
    public string Message { get; } = message;
    public bool IsStandalone { get; } = isStandalone;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitComment(this, args);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args)
    {
        visitor.VisitComment(this, args);
    }
}

