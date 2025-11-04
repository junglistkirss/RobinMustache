using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class BooleanDataFacade : IDataFacade
{
    public readonly static BooleanDataFacade Instance = new();
    private BooleanDataFacade() { }

    public bool IsTrue(object? obj) => true.Equals(obj);
    public bool IsCollection(object? _, [NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}