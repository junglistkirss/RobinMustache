using Robin.Contracts.Expressions;

namespace Robin.Contracts.Context;

public abstract record class RenderContext
{
    public RenderContext? Parent { get; init; }
    public  object? Data { get; init;}
    public required IEvaluator Evaluator { get; init; }
    public bool TryResolve(IExpressionNode expression, out object? value)
    {
        if( Evaluator.TryResolve(expression, this.Data, out value))
        return true;
        return Evaluator.TryResolve(expression, Parent?.Data, out value);
    }
}



