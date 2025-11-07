using Robin.Contracts.Nodes;
using System.Collections.Concurrent;
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

public static class RenderContextPool<T> where T : class
{
    private static readonly ConcurrentBag<RenderContext<T>> Pool = new();
    private const int MaxPoolSize = 5;

    public static RenderContext<T> Get(
        IEvaluator evaluator,
        T builder,
        ReadOnlyDictionary<string, ImmutableArray<INode>>? partials = null)
    {
        if (!Pool.TryTake(out var ctx))
            ctx = new RenderContext<T>();

        ctx.Evaluator = evaluator;
        ctx.Builder = builder;
        ctx.Partials = partials ;

        return ctx;
    }

    public static void Return(RenderContext<T> ctx)
    {
        ctx.Partials = null;
        ctx.Builder = null!;
        ctx.Evaluator = null!;
        if (Pool.Count < MaxPoolSize)
            Pool.Add(ctx);
    }
}



public sealed class PartialsScope<T> : IDisposable
    where T : class
{
    private readonly RenderContext<T> _context;
    private readonly ReadOnlyDictionary<string, ImmutableArray<INode>>? _original;

    public PartialsScope(RenderContext<T> context, ReadOnlyDictionary<string, ImmutableArray<INode>>? newPartials)
    {
        _context = context;
        // sauvegarde de l’état original
        _original = context.Partials;
        // appliquer temporairement les nouveaux partiels
        _context.Partials = newPartials;
    }

    public void Dispose()
    {
        // restauration de l’état original
        _context.Partials = _original;
        GC.SuppressFinalize(this);
    }
}