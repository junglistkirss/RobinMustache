using RobinMustache.Abstractions;
using RobinMustache.Abstractions.Context;
using RobinMustache.Abstractions.Expressions;
using RobinMustache.Abstractions.Facades;

namespace RobinMustache.Internals;

internal sealed class ServiceEvaluator(IExpressionNodeVisitor<DataContext> visitor, IEnumerable<IDataFacadeResolver> facadeResolver) : IEvaluator
{

    private bool TryResolve(IExpressionNode expression, DataContext data, out object? value, out IDataFacade facade)
    {
        try
        {
            if (expression.Accept(visitor, data, out value))
            {
                foreach (IDataFacadeResolver resolver in facadeResolver)
                {
                    if (resolver.ResolveDataFacade(value, out IDataFacade? resolvedFacade) && resolvedFacade is not null)
                    {
                        facade = resolvedFacade;
                        return true;
                    }
                }
                facade = value.GetPrimitiveFacade();
                return true;
            }
        }
        catch (Exception) { }
        facade = DataFacade.Null;
        value = null;
        return false;
    }

    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        if (data is not null)
        {
            if (TryResolve(expression, data, out object? value, out facade))
            {
                return value;
            }
            else if (data.Parent is not null && TryResolve(expression, data.Parent, out object? parentValue, out facade))
            {
                return parentValue;
            }
        }
        facade = DataFacade.Null;
        return null;
    }
}
