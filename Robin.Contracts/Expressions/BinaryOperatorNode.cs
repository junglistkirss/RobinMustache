namespace Robin.Contracts.Expressions;

public readonly struct BinaryOperationExpressionNode(IExpressionNode left, BinaryOperator @operator, IExpressionNode right) : IExpressionNode
{
    public BinaryOperator Operator { get; } = @operator;
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitBinaryOperation(this, args);
    }
}

