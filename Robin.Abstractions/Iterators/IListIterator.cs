using Robin.Abstractions.Accessors;
using System.Collections;

namespace Robin.Abstractions.Iterators;

internal sealed class IListIterator(IList list) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (object? item in list)
            action(item);
    }
}
