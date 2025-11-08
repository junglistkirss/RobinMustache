using Robin.Contracts.Variables;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Robin.Abstractions.Accessors;

public delegate bool ChainableGetter(object? input, out object? value);

public static class DelegateHelper
{
    public static ChainableGetter AsChainable(this Delegate del)
    {
        ArgumentNullException.ThrowIfNull(del);

        MethodInfo method = del.Method;
        ConstantExpression? target = del.Target == null ? null : Expression.Constant(del.Target);

        // Paramètre du Func<object?, object?>
        ParameterExpression inputParam = Expression.Parameter(typeof(object), "input");
        ParameterExpression outputParam = Expression.Parameter(typeof(object).MakeByRefType(), "value");
        LabelTarget returnLabel = Expression.Label(typeof(bool), "returnLabel");

        Type funcType = del.GetType();
        Type[] genericArgs = funcType.GetGenericArguments();
        Type argType = genericArgs[0];

        UnaryExpression convertedArg = Expression.Convert(inputParam, argType);

        // Appel : del(arg)
        Expression call = target == null
            ? Expression.Call(method, convertedArg)
            : Expression.Call(target, method, convertedArg);
        UnaryExpression convertedResult = Expression.Convert(call, typeof(object));
        BinaryExpression assignOutput = Expression.Assign(outputParam, convertedResult);
        LabelExpression returnTrue = Expression.Label(returnLabel, Expression.Constant(true));

        BlockExpression body = Expression.Block(assignOutput, returnTrue);

        Expression<ChainableGetter> lambda = Expression.Lambda<ChainableGetter>(body, inputParam, outputParam);

        return lambda.Compile();
    }

}

public interface IIndexDelegateAccessor
{
    bool TryGetIndex(int index, [NotNull] out Delegate value);
}

public interface IIndexDelegateAccessor<T> : IIndexDelegateAccessor { }


public interface IIndexAccessor
{
    bool TryGetIndex(object? obj, int index, out object? value);
}

public interface IIndexAccessor<T> : IIndexAccessor
{
    bool IIndexAccessor.TryGetIndex(object? obj, int index, out object? value)
    {
        if (obj is T typed)
        {
            return TryGetIndex(typed, index, out value);
        }
        value = null;
        return false;
    }

    bool TryGetIndex(T obj, int index, out object? value);
}

public abstract class BaseAccessorVisitor :  IVariableSegmentVisitor<Type, ChainableGetter>
{
    public abstract bool VisitIndex(IndexSegment accessor, Type args, out ChainableGetter result);
    public abstract bool VisitMember(MemberSegment accessor, Type args, out ChainableGetter result);

    public bool VisitThis(ThisSegment segment, Type args, out ChainableGetter getter)
    {
        getter = new ChainableGetter((object? input, out object? value) =>
        {
            value = input;
            return true;
        });
        return true;
    }
}