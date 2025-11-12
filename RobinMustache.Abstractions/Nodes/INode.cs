namespace RobinMustache.Abstractions.Nodes;

public interface INode
{
    bool IsStandalone { get; }
    TOut Accept<TOut, TArgs>(INodeVisitor<TOut, TArgs> visitor, TArgs args);
    void Accept<TArgs>(INodeVisitor<TArgs> visitor, TArgs args);
}
