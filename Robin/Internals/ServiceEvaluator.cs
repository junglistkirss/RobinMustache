using Robin.Abstractions;
using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Internals;

internal sealed class ServiceEvaluator(IExpressionNodeVisitor<DataContext> visitor, IDataFacadeResolver facadeResolver) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        if (data is not null)
        {
            bool resolved = expression.Accept(visitor, data, out object? value);
            if (resolved)
            {
                facade = facadeResolver.ResolveDataFacade(value);
                return value;
            }
            else if (data.Parent is not null)
            {
                resolved = expression.Accept(visitor, data.Parent, out object? parentValue);
                if (resolved)
                {
                    facade = facadeResolver.ResolveDataFacade(parentValue);
                    return parentValue;
                }
            }
        }
        facade = DataFacade.Null;
        return null;
    }
}
