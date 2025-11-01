using Robin.Contracts.Expressions;
using System.Text;

namespace Robin.Contracts.Context;

public record class RenderContext
{
    public object? Data { get; init; }
    public required IEvaluator Evaluator { get; init; }
    public required StringBuilder Builder { get; init; }
    public bool TryResolve(IExpressionNode expression, out object? value)
    {
        return (Evaluator.TryResolve(expression, Data, out value));
    }
}



