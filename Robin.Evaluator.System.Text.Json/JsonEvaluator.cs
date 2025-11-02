using Robin.Abstractions;
using Robin.Contracts.Expressions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

public static class JsonFacades
{
    public static IDataFacade FromJsonNode(object? obj) => obj is JsonNode jNode ? new JsonNodeFacade(jNode) : throw new InvalidDataException("Not a json node");
    public static IDataFacade AsJsonFacade(this object? obj) => obj is JsonNode node ? new JsonNodeFacade(node) : obj.AsFacade();

}

internal sealed class JsonNodeFacade(JsonNode node) : IDataFacade
{


    public object? RawValue => node;

    public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Array:
                JsonArray jArray = node.AsArray()!;
                collection = jArray;
                return jArray.Count > 0;
            default:
                break;
        }
        collection = null;
        return false;
    }

    public bool IsTrue()
    {
        switch (node.GetValueKind())
        {
            case JsonValueKind.Undefined:
                return false;
            case JsonValueKind.Object:
                JsonObject? jObject = node.AsObject();
                return jObject is not null;
            case JsonValueKind.Array:
                JsonArray? jArray = node.AsArray();
                return jArray is not null && jArray.Count > 0;
            case JsonValueKind.String:
                return !string.IsNullOrEmpty(node.GetValue<string>());
            case JsonValueKind.Number:
                return true;
            case JsonValueKind.True:
                return true;
            case JsonValueKind.False:
            case JsonValueKind.Null:
            default:
                return false;
        }
    }
}



public sealed class JsonEvaluator : IEvaluator
{
    public static readonly JsonEvaluator Instance = new();
    private static readonly ExpressionNodeVisitor NodeInstance = new(JsonAccesorVisitor.Instance);

    //public bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection)
    //{
    //    if (value is JsonNode node)
    //    {
    //        if (node.GetValueKind() == JsonValueKind.Array)
    //        {
    //            JsonArray jArray = node.AsArray()!;
    //            collection = jArray;
    //            return jArray.Count > 0;
    //        }
    //    }
    //    else if (value is IEnumerable objects)
    //    {
    //        collection = objects;
    //        return true;

    //    }
    //    collection = null;
    //    return false;
    //}

    //public bool IsTrue(object? value)
    //{
    //    if (value is JsonNode node)
    //    {
    //        switch (node.GetValueKind())
    //        {
    //            case JsonValueKind.Undefined:
    //                return false;
    //            case JsonValueKind.Object:
    //                var obj = node.AsObject();
    //                return obj is not null;
    //            case JsonValueKind.Array:
    //                JsonArray? jArray = node.AsArray();
    //                return jArray is not null && jArray.Count > 0;
    //            case JsonValueKind.String:
    //                return !string.IsNullOrEmpty(node.GetValue<string>());
    //            case JsonValueKind.Number:
    //                return true;
    //            case JsonValueKind.True:
    //                return true;
    //            case JsonValueKind.False:
    //            case JsonValueKind.Null:
    //                return false;
    //            default:
    //                break;
    //        }
    //    }
    //    return value is bool b ? b : value is string s ? !string.IsNullOrEmpty(s) : value is not null;
    //}

    public IDataFacade Resolve(IExpressionNode expression, DataContext? data)
    {
        if (data is null)
            return Facades.Null;

        EvaluationResult result = expression.Accept(NodeInstance, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(NodeInstance, data.Parent);

        if (result.Status == ResoltionState.Found)
            return result.Value;

        return Facades.Null;
    }
}

