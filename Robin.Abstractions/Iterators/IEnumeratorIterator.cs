using Robin.Abstractions.Accessors;
using System.Collections;

namespace Robin.Abstractions.Iterators;

internal sealed class IEnumeratorIterator(IEnumerator enumerator) : IIterator
{
    public void Iterate(Action<object?> action)
    {
        while (enumerator.MoveNext())
            action(enumerator.Current);
    }
}
