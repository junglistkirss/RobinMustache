using Robin.Contracts.Expressions;
using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Text;

namespace Robin.Contracts.Context;


public interface IEvaluator
{
    bool TryResolve(IExpressionNode expression, object? data, out object? value);
    bool IsTrue(object? value);
}



