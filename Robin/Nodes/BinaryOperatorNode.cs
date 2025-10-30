using System.Text;

namespace Robin.Nodes;

public readonly struct BinaryOperationNode(IExpressionNode left, string @operator, IExpressionNode right) : IExpressionNode
{
    public string Operator { get; } = @operator;
    public IExpressionNode Left { get; } = left;
    public IExpressionNode Right { get; } = right;
}

