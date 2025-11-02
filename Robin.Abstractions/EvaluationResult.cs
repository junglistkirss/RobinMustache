using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;



public delegate IDataFacade DataFacadeFactory(object? data);
public interface IDataFacade
{
    bool IsTrue();
    bool IsCollection([NotNullWhen(true)] out IEnumerable? collection);
    object? RawValue { get; }
}

public static class Facades
{
    private sealed class NullDataFacade : IDataFacade
    {
        public object? RawValue => null;

        public bool IsCollection() => false;

        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }

        public bool IsTrue() => false;
    }
    private sealed class UndefinedDataFacade : IDataFacade
    {
        public object? RawValue => null;

        public bool IsCollection() => false;
        public bool IsTrue() => false;
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }
    private sealed class LiteralDataFacade(string value) : IDataFacade
    {
        public object? RawValue => value;

        public bool IsCollection() => false;
        public bool IsTrue() => !string.IsNullOrEmpty(value);
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }
    private sealed class DoubleDataFacade(double value) : IDataFacade
    {
        public object? RawValue => value;

        public bool IsCollection() => false;
        public bool IsTrue() => value > 0;
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }
    private sealed class LongDataFacade(long value) : IDataFacade
    {
        public object? RawValue => value;

        public bool IsCollection() => false;
        public bool IsTrue() => value > 0;
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }
    private sealed class IntDataFacade(int value) : IDataFacade
    {
        public object? RawValue => value;

        public bool IsCollection() => false;
        public bool IsTrue() => value > 0;
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }

    private sealed class ShortDataFacade(short value) : IDataFacade
    {
        public object? RawValue => value;

        public bool IsCollection() => false;
        public bool IsTrue() => value > 0;
        public bool IsCollection([NotNullWhen(true)] out IEnumerable? collection)
        {
            collection = null;
            return false;
        }
    }

    public static readonly IDataFacade Null = new NullDataFacade();

    public static readonly IDataFacade Undefined = new UndefinedDataFacade();
    private readonly static Dictionary<Type, DataFacadeFactory> _facadeFactories = new();

    public static void RegisterFacadeFactory<T>(DataFacadeFactory factory)
    {
        _facadeFactories[typeof(T)] = factory;
    }

    public static IDataFacade AsFacade(this object? obj)
    {
        if (obj is null)
            return Null;

        switch (obj)
        {
            case string v: return new LiteralDataFacade(v);
            case double v: return new DoubleDataFacade(v);
            case long v: return new DoubleDataFacade(v);
            case int v: return new IntDataFacade(v);
            case short v: return new ShortDataFacade(v);
            default:
                if (_facadeFactories.TryGetValue(obj.GetType(), out DataFacadeFactory? factory))
                    return factory(obj);
                throw new NotSupportedException($"No facade registered for type {obj.GetType().FullName}");
        }
    }
}

public record EvaluationResult(ResoltionState Status, IDataFacade Value);
