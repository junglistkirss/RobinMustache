using Robin.Abstractions.Accessors;

namespace Robin.Abstractions.Iterators;

internal sealed class ArrayIterator(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is Array arr)
        {
            foreach (var item in arr)
                action(item);
        }
    }
}
