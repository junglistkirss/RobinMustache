using Robin.Abstractions.Context;
using Robin.Abstractions.Facades;
using Robin.Contracts.Expressions;
using Robin.Contracts.Variables;

namespace Robin.Abstractions;

public sealed class ServiceEvaluator(IVariableSegmentVisitor<EvaluationResult, object?> accesorVisitor) : IEvaluator
{
    public object? Resolve(IExpressionNode expression, DataContext? data, out IDataFacade facade)
    {
        ExpressionNodeVisitor visitor = new(accesorVisitor);
        if (data is not null)
        {
            EvaluationResult result = expression.Accept(visitor, data);

            if (!result.IsResolved && data.Parent is not null)
                result = expression.Accept(visitor, data.Parent);

            if (result.IsResolved)
            {
                facade = result.Value.GetFacade();
                return result.Value;
            }
        }
        facade = DataFacade.Null;
        return null;
    }
}
