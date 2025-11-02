using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions;

public interface IDataFacade
{
    bool IsTrue();
    bool IsCollection([NotNullWhen(true)] out IEnumerable? collection);
    object? RawValue { get; }
}
