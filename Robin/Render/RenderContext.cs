using Robin.Expressions;

namespace Robin.Render;

public abstract record class RenderContext(object? Data)
{
    public abstract bool TryResolve(IExpressionNode expression, out object? value);
}

public abstract record class RenderContext<T>(T TypedData) : RenderContext(TypedData)
{

}
