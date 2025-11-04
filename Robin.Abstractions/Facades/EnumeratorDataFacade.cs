using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class EnumeratorDataFacade : IDataFacade
{
    public readonly static EnumeratorDataFacade Instance = new();
    private EnumeratorDataFacade() { }
    public bool IsTrue(object? _) => true;
    public bool IsCollection(object? obj, [NotNullWhen(true)] out IEnumerator? collection)
    {
        if (obj is IEnumerator value)
        {
            collection = value;
            return true;
        }
        collection = null;
        return false;
    }
}
