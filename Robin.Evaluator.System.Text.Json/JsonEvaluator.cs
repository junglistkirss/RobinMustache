using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Nodes;

namespace Robin.Evaluator.System.Text.Json;

internal sealed class JsonEvaluator(IEvaluator evaluator) : IJsonEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        object? value = evaluator.Resolve(expression, data, out IDataFacade baseFacade);
        facade = value.AsJsonFacade(baseFacade);
        return value;
    }
}


internal sealed class JsonDataFacadeResolver : IDataFacadeResolver
{
    public bool ResolveDataFacade(object? data, [NotNullWhen(true)] out IDataFacade? facade)
    {
        if (data is null)
        {
            facade = DataFacade.Null;
            return true;
        }
        if (data is JsonNode)
        {
            facade = JsonNodeFacade.Instance;
            return true;
        }
        facade = null;
        return false;
    }
}
