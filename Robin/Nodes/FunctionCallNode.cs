using System.Collections.Immutable;
using System.Text;

namespace Robin.Nodes;

public readonly struct FunctionCallNode(string functionName, ImmutableArray<IExpressionNode> arguments) : IExpressionNode
{
    public string FunctionName { get; } = functionName;
    public ImmutableArray<IExpressionNode> Arguments { get; } = arguments;
}
