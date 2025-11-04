using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Robin.Abstractions.Facades;

namespace Robin.Abstractions;

public record EvaluationResult(ResoltionState Status, object? Value, IDataFacade Facade);

public record ValueFacade(object? Value, IDataFacade Facade)
{
    public bool IsTrue() => Facade.IsTrue(Value);
    public bool IsCollection([NotNullWhen(true)]out IEnumerator? enumerator) => Facade.IsCollection(Value, out enumerator);
};
