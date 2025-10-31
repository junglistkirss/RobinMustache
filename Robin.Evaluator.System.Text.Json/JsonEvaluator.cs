using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Expressions;

namespace Robin.Evaluator.System.Text.Json;

public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();

    public bool TryResolve(IExpressionNode expression, object? data, out object? value)
    {
        if (data is JsonNode node)
        {
            value = expression.Accept(JsonObjectExpressionNodeVisitor.Instance, node);
            return true;
        }
        value = null;
        return false;
    }
}

