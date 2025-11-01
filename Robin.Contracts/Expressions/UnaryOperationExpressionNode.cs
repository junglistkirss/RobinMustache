namespace Robin.Contracts.Expressions;

public readonly struct UnaryOperationExpressionNode(UnaryOperator @operator, IExpressionNode operand) : IExpressionNode
{
    public UnaryOperator Operator { get; } = @operator;
    public IExpressionNode Operand { get; } = operand;

    public TOut Accept<TOut, TArgs>(IExpressionNodeVisitor<TOut, TArgs> visitor, TArgs args)
    {
        return visitor.VisitUnaryOperation(this, args);
    }
}