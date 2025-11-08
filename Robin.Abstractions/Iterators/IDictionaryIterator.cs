using Robin.Abstractions.Accessors;
using System.Collections;

namespace Robin.Abstractions.Iterators;

internal sealed class IDictionaryIterator(IDictionary dictionary) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        foreach (object? item in dictionary)
        {
            action(item);
        }
    }
}
