using System.Text.Json.Nodes;
using Robin.Contracts.Context;
using Robin.Contracts.Variables;

namespace Robin.Evaluator.System.Text.Json;


internal sealed class JsonAccesorVisitor : IAccessorVisitor<EvaluationResult, DataContext>
{
    public readonly static JsonAccesorVisitor Instance = new();
    public EvaluationResult VisitIndex(IndexAccessor accessor, DataContext args)
    {
        if (args.Data is JsonArray json)
        {
            return new(true, json[accessor.Index]);
        }
        return new(false, null);
    }

    public EvaluationResult VisitKey(KeyAccessor accessor, DataContext args)
    {
        EvaluationResult resolvedKey = accessor.Key.Accept(this, args);
        if (!resolvedKey.Found && args.Previsous is not null)
            resolvedKey = accessor.Key.Accept(this, args.Previsous);
        if (resolvedKey.Found)
        {
            string key = resolvedKey.Value?.ToString() ?? string.Empty;

            if (args.Data is JsonObject json && json.TryGetPropertyValue(key, out JsonNode? keyNode))
            {
                return new(true, keyNode);
            }
            else
            {
                DataContext? prev = args.Previsous;
                EvaluationResult resolved = new(false, null);
                while (!resolved.Found && prev is not null)
                {
                    if (prev.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(key, out JsonNode? keyNodePrev))
                    {
                        resolved = new(true, keyNodePrev);
                    }
                    else
                    {
                        prev = prev.Previsous;
                    }
                }
            }
        }
        return new(false, null);
    }

    public EvaluationResult VisitMember(MemberAccessor accessor, DataContext args)
    {
        if (args.Data is JsonObject json && json.TryGetPropertyValue(accessor.MemberName, out JsonNode? node))
            return new(true, node);
        if (args.Previsous?.Data is JsonObject jsonPrev && jsonPrev.TryGetPropertyValue(accessor.MemberName, out JsonNode? nodePrev))
            return new(true, nodePrev);
        return new(false, null);
    }

    public EvaluationResult VisitParent(ParentAccessor accessor, DataContext args)
    {
        if (args.Previsous?.Data is not null)
            return new(true, args.Previsous.Data);
        return new(false, null);
    }

    public EvaluationResult VisitThis(ThisAccessor accessor, DataContext args)
    {
        return new(true, args.Data);
    }
}

