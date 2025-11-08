namespace Robin.Abstractions.Accessors;

public interface IIterator
{
    void Iterate(Action<object?> action);
}
