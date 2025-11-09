using System.Diagnostics.CodeAnalysis;

namespace Robin.Abstractions.Helpers;

public sealed class Helper
{
    public delegate object? Function(object?[] args);

    private readonly Dictionary<string, Function> _functions = [];

    public bool TryAddFunction(string name, Function function)
    {
        if(_functions.ContainsKey(name))
            return false;
        _functions.Add(name, function);
        return true;
    }

    public bool TryGetFunction(string name, out Function? function)
    {
        return _functions.TryGetValue(name, out function);
    }
}
