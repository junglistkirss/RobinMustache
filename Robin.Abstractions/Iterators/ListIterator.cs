using Robin.Abstractions.Accessors;

namespace Robin.Abstractions.Iterators;

internal sealed class ListIterator<T>(object? value) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        if (value is List<T> list)
        {
            foreach (object? item in list)
                action(item);
        }
    }
}
