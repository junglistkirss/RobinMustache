using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Abstractions;

public interface IEvaluator
{
    object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade);
}
