using Robin.Contracts.Expressions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;


public interface IEvaluator
{
    bool TryResolve(IExpressionNode expression, DataContext? data, out object? value);
    bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection);
    bool IsTrue(object? value);
}



