using System.Collections;
using System.Diagnostics.CodeAnalysis;
using Robin.Abstractions.Facades;

namespace Robin.Abstractions;

public record EvaluationResult(ResoltionState Status, object? Value, IDataFacade Facade);

