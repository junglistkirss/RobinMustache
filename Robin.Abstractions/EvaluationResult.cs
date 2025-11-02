namespace Robin.Abstractions;

public enum ResoltionState
{
    Found,
    NotFound,
    Partial,
}

public record EvaluationResult(ResoltionState Status, object? Value);
