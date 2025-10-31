using Robin.Contracts.Expressions;

namespace Robin.Contracts.Context;


public interface IEvaluator
{
    bool TryResolve(IExpressionNode expression, object? data, out object? value);
}

