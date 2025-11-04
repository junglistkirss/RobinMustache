using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ServiceEvaluator(IVariableSegmentVisitor<EvaluationResult, object?> accesorVisitor) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        var visitor = new ExpressionNodeVisitor(accesorVisitor);
        if (data is null)
        {
            facade = DataFacade.Null;
            return null;
        }

        EvaluationResult result = expression.Accept(visitor, data);

        if (result.Status == ResoltionState.NotFound && data.Parent is not null)
            result = expression.Accept(visitor, data.Parent);

        if (result.Status == ResoltionState.Found)
        {
            facade = result.Facade;
            return result.Value;
        }
        facade = DataFacade.Null;
        return null;
    }
}
