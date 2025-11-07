using Microsoft.Extensions.DependencyInjection;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using Robin.Contracts.Variables;
using Robin.Internals;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Robin.Abstractions.Extensions;

public static class RobinExtensions
{
    public static Dictionary<string, ImmutableArray<INode>> ExtractsPartials(this ImmutableArray<INode> nodes, ReadOnlyDictionary<string, ImmutableArray<INode>>? baseCollection = null)
    {
        Dictionary<string, ImmutableArray<INode>> collection;
        if (baseCollection is not null)
            collection = new(baseCollection);
        else collection = [];
        return nodes.Aggregate(collection, (current, node) => node.Accept(PartialExtractor.Instance, current));
    }
    private const string BaseEvaluatorKey = "base";

    public static IServiceCollection AddServiceEvaluator(this IServiceCollection services)
    {
        return services
            .AddSingleton<IExpressionNodeVisitor<DataContext>, ExpressionNodeVisitor>()
            // .AddKeyedSingleton<IEvaluator, ServiceEvaluator>(BaseEvaluatorKey)
            .AddSingleton<IDataFacadeResolver, DataFacadeResolver>()
            .AddSingleton<IEvaluator, ServiceEvaluator>()
            .AddSingleton<IVariableSegmentVisitor<Type>, ServiceAccesorVisitor>();
    }
}



