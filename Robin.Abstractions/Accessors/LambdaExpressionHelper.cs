namespace Robin.Internals;

public static class LambdaExpressionHelper
{
    public static Type GetInputType(this Delegate lambda)
    {
        var parameters = lambda.Method.GetParameters();
        if (parameters.Length == 0)
            throw new ArgumentException("La lambda doit avoir au moins un param�tre");

        return parameters[0].ParameterType;
    }

    public static Type GetReturnType(this Delegate lambda)
    {
        return lambda.Method.ReturnType;
    }
}
