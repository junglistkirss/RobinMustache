using Robin.Contracts.Expressions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;


public interface IEvaluator
{
    IDataFacade Resolve(IExpressionNode expression, DataContext? data);
    //bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerable? collection);
    //bool IsTrue(object? value);
}



