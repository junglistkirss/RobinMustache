using Robin.Abstractions.Facades;
using System.Collections.Concurrent;

namespace Robin.Internals;

internal sealed class DataFacadeResolver(IServiceProvider provider) : IDataFacadeResolver
{

    private readonly ConcurrentDictionary<Type, IDataFacade?> cache = new();

    public IDataFacade ResolveDataFacade(object? data)
    {
        if (data is null)
        {
            return DataFacade.Null;
        }
        Type type = data.GetType();
        return cache.GetOrAdd(type, (_) =>
        {
            Type genType = typeof(IDataFacade<>).MakeGenericType(type);
            IDataFacade? facade = (IDataFacade?)provider.GetService(genType);
            if (facade is not null) return facade;
            return data.GetFacade()!;
        }) ?? data.GetFacade();


    }
}
