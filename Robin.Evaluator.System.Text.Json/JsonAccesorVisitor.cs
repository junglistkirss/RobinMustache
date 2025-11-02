using System.Text.Json.Nodes;
using Robin.Abstractions;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;


internal sealed class JsonAccesorVisitor : IAccessorVisitor<EvaluationResult, DataContext>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexAccessor accessor, DataContext context)
    {
        if (context.Data is JsonArray json)
        {
            return new(ResoltionState.Found, json[accessor.Index]);
        }
        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitKey(KeyAccessor accessor, DataContext context)
    {
        EvaluationResult resolvedKey = accessor.Key.Evaluate(this, context);

        if (resolvedKey.Status == ResoltionState.NotFound && context.Parent is not null)
            resolvedKey = accessor.Key.Evaluate(this, context.Parent);

        if (resolvedKey.Status == ResoltionState.Found)
        {
            string key = resolvedKey.Value?.ToString() ?? string.Empty;

            if (context.Data is JsonObject json && json.TryGetPropertyValue(key, out JsonNode? keyNode))
            {
                return new(ResoltionState.Found, keyNode);
            }
            else if (context.Parent?.Data is JsonObject prevJson && prevJson.TryGetPropertyValue(key, out JsonNode? prevKeyNode))
            {
                return new(ResoltionState.Found, prevKeyNode);
            }
        }
        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, DataContext context)
    {
        if (context.Data is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(ResoltionState.Found, node);

        if (context.Parent?.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(accessor.MemberName, out JsonNode? nodePrev))
            return new(ResoltionState.Found, nodePrev);

        return new(ResoltionState.NotFound, null);
    }

    public EvaluationResult VisitParent(ParentAccessor accessor, DataContext context)
    {
        DataContext parent = context;
        while (parent.Parent is not null)
        {
            parent = parent.Parent;
        }
        return new(ResoltionState.Found, parent.Data);
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, DataContext context)
    {
        return new(ResoltionState.Found, context.Data);
    }
}

