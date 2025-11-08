using Robin.Abstractions.Accessors;
using System.Collections.Immutable;

namespace Robin.Abstractions.Iterators;

internal sealed class ImmutableArrayIterator<T>(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is ImmutableArray<T> arr)
        {
            var span = arr.AsSpan();
            for (int i = 0; i < span.Length; i++)
                action(span[i]!); // nullable suppression si nécessaire
        }
    }
}
