using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Accessors;

public interface IMemberDelegateAccessor
{
    bool TryGetMember(string name, [NotNull] out Delegate value);
}
public interface IMemberDelegateAccessor<T> : IMemberDelegateAccessor { }

public interface IMemberAccessor
{
    bool TryGetMember(object? obj, string name, out object? value);
}

public interface IMemberAccessor<T> : IMemberAccessor
{
    bool IMemberAccessor.TryGetMember(object? obj, string name, out object? value)
    {
        if (obj is T typed)
        {
            return TryGetMember(typed, name, out value);
        }
        value = null;
        return false;
    }

    bool TryGetMember(T obj, string name, out object? value);
}