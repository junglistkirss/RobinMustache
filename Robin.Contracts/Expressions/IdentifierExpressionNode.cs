using Robin.Contracts.Variables;

namespace Robin.Contracts.Expressions;

public readonly struct IdentifierExpressionNode(AccesorPath path) : IExpressionNode
{
    public AccesorPath Path { get; } = path;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitIdenitifer(this, args);
    }
};
