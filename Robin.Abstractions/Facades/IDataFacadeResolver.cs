namespace Robin.Abstractions.Facades;

public interface IDataFacadeResolver
{
    IDataFacade ResolveDataFacade(object? data);
}
