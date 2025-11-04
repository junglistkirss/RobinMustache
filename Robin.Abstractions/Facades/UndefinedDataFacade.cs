using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class ObjectDataFacade : IDataFacade
{
    public readonly static ObjectDataFacade Instance = new();
    private ObjectDataFacade() { }
    public bool IsTrue(object? value) => value is not null;
    public bool IsCollection(object? value, [NotNullWhen(true)] out IEnumerator? collection)
    {
        if (value is IEnumerator enumerator)
        {
            collection = enumerator;
            return collection is not null;
        }
        else if (value is IEnumerable enumerable)
        {
            collection = enumerable.GetEnumerator();
            return collection is not null;
        }
        collection = null;
        return false;
    }
}
