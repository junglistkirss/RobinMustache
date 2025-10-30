using System.Text;

namespace Robin.Nodes;

public readonly struct VariableNode(IExpressionNode expression, bool unescaped) : INode
{
    public IExpressionNode Expression { get; } = expression;
    public bool IsUnescaped { get; } = unescaped;
}

