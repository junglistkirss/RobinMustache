using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;

namespace Robin.Abstractions;

public delegate object? Helper(params object?[] args);

public interface IEvaluator
{
    IDataFacade Resolve(IExpressionNode expression, DataContext? data);
}



