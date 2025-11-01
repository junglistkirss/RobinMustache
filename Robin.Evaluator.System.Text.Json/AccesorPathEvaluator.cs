using System.Text.Json.Nodes;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;

internal static class AccesorPathEvaluator
{
    internal static object? Evaluate(this AccesorPath path, JsonNode node)
    {
        JsonEvaluationResult result = new(true, null);
        int i = 0;
        object? ctx = node;
        while (result.Found && i < path.Segments.Length)
        {
            IAccessor item = path.Segments[i];
            if (ctx is JsonNode n)
            {
                JsonEvaluationResult res = item.Accept(JsonObjectAccesorVisitor.Instance, n);
                result = res;
                if (res.Found)
                    ctx = res.Value;
                else if(n.Parent is not null) {
                    // Try to resolve from parent context
                    JsonEvaluationResult parentRes = item.Accept(JsonObjectAccesorVisitor.Instance, n.Parent);
                    result = parentRes;
                    if (parentRes.Found)
                        ctx = parentRes.Value;
                }
            }
            else
            {
                result = result with { Found = false };
            }
            i++;
        }
        return result.Value;
    }
}

