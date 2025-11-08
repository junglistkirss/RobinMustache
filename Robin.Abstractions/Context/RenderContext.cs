using Robin.Contracts.Nodes;
using System.Collections.Immutable;
using System.Collections.ObjectModel;

namespace Robin.Abstractions.Context;

public sealed class RenderContext<T>
    where T : class
{
    public ReadOnlyDictionary<string, ImmutableArray<INode>>? Partials { get; internal set; }
    public IEvaluator Evaluator { get; internal set; } = null!;
    public T Builder { get; internal set; } = null!;
}
