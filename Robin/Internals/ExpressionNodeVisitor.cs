using Robin.Abstractions.Accessors;
using Robin.Abstractions.Context;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;
using System.Collections.Concurrent;
using System.Diagnostics.Metrics;

namespace Robin.Internals;

internal sealed class ExpressionNodeVisitor(IEnumerable<IVariableSegmentVisitor<Type, ChainableGetter>> accessorVisitors) : IExpressionNodeVisitor<DataContext>
{


    private readonly ConcurrentDictionary<CacheKey, ChainableGetter> cache = new();
    private record struct CacheKey(Type Type, IVariableSegment Segment);

    public bool VisitFunctionCall(FunctionCallNode node, DataContext args, out object? value)
    {
        //if (args.Helper.TryGetFunction(node.FunctionName, out Helper.Function? function) && function is not null)
        //{
        //    object?[] evaluatedArgs = new object?[node.Arguments.Length];
        //    for (int i = 0; i < node.Arguments.Length; i++)
        //    {
        //        EvaluationResult evalResult = node.Arguments[i].Accept(this, args);
        //        if (evalResult.IsResolved)
        //        {
        //            evaluatedArgs[i] = evalResult.Value;
        //        }
        //        else
        //        {
        //            evaluatedArgs[i] = null;
        //        }
        //    }
        //    object? functionResult = function(evaluatedArgs);
        //    return new EvaluationResult(true, functionResult);
        //}
        value = null;
        return false;
    }

    private ChainableGetter BuildGetter(CacheKey cacheKey)
    {
        Type currentType = cacheKey.Type;
        IVariableSegment current = cacheKey.Segment;
        foreach (IVariableSegmentVisitor<Type, ChainableGetter> accessorVisitor in accessorVisitors)
        {
            bool getterResolved = current.Accept(accessorVisitor, currentType, out ChainableGetter getterInfo);
            if (getterResolved)
                return getterInfo;
        }
        return new ChainableGetter((object? input, out object? output) =>
        {
            output = null;
            return false;
        });
    }

    public bool VisitIdenitifer(IdentifierExpressionNode node, DataContext args, out object? value)
    {
        if (args.Data is null)
        {
            value = null;
            return false;
        }
        if (node.Path.Length == 0)
        {
            value = null;
            return false;
        }

        object? current = args.Data;
        bool resolved = true;
        int i = 0;
        while (i < node.Path.Length && current is not null && resolved)
        {
            IVariableSegment segment = node.Path[i];
            ChainableGetter getter = cache.GetOrAdd(new CacheKey(current.GetType(), segment), BuildGetter);
            try
            {
                resolved = getter(current, out object? next);
                if (!resolved && i > 0)
                {
                    value = null;
                    return true; // avoid precedence here
                }
                current = next;
                i++;
            }
            catch (Exception)
            {
                resolved = false;
                if (i > 0)
                {
                    value = null;
                    return false;
                }
            }
        }
        if (resolved)
        {
            value = current;
            return true;
        }

        if (args.Parent is not null)
            return VisitIdenitifer(node, args.Parent, out value);

        value = null;
        return false;
    }

    public bool VisitLiteral(LiteralExpressionNode node, DataContext _, out object? value)
    {
        value = node.Constant;
        return true;
    }

    public bool VisitIndex(IndexExpressionNode node, DataContext _, out object? value)
    {
        value = node.Constant;
        return true;
    }
}
