using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Facades;

internal sealed class CharDataFacade : IDataFacade
{
    public readonly static CharDataFacade Instance = new();
    private CharDataFacade() { }
    public bool IsTrue(object? obj) => obj is not '\0';
    public bool IsCollection(object? _, [NotNullWhen(true)] out IEnumerator? collection)
    {
        collection = null;
        return false;
    }
}
