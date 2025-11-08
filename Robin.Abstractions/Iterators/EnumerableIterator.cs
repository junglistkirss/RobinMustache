using Robin.Abstractions.Accessors;
using System.Collections;

namespace Robin.Abstractions.Iterators;

internal sealed class EnumerableIterator(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is IEnumerable enumerable)
        {
            foreach (var item in enumerable)
                action(item);
        }
    }
}
