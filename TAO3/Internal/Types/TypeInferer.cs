using FastExpressionCompiler;
using System.Linq.Expressions;
using System.Reflection;

namespace TAO3.Internal.Types;

internal static class TypeInferer
{
    public static void Invoke(
        object obj,
        Type genericTypeDefinition,
        Expression<Action> methodExpression)
    {
        Invoke(
            obj.GetType(),
            genericTypeDefinition,
            methodExpression);
    }

    public static R Invoke<R>(
        object obj,
        Type genericTypeDefinition,
        Expression<Func<R>> methodExpression)
    {
        return Invoke(
            obj.GetType(),
            genericTypeDefinition,
            methodExpression);
    }

    public static void Invoke(
        Type type,
        Type genericTypeDefinition,
        Expression<Action> methodExpression)
    {
        Expression bindedMethod = BindMethod(
            type,
            genericTypeDefinition,
            methodExpression);

        Action lambdaExpression = Expression
            .Lambda<Action>(bindedMethod)
            .CompileFast();

        lambdaExpression.Invoke();
    }

    public static R Invoke<R>(
        Type type,
        Type genericTypeDefinition,
        Expression<Func<R>> methodExpression)
    {
        Expression bindedMethod = BindMethod(
            type,
            genericTypeDefinition,
            methodExpression);

        Func<R> lambdaExpression = Expression
            .Lambda<Func<R>>(bindedMethod)
            .CompileFast();

        return lambdaExpression.Invoke();
    }

    private static Expression BindMethod(
        Type type,
        Type genericTypeDefinition,
        LambdaExpression methodExpression)
    {
        _ = type ?? throw new ArgumentException(nameof(type));
        _ = genericTypeDefinition ?? throw new ArgumentNullException(nameof(genericTypeDefinition));
        _ = methodExpression ?? throw new ArgumentNullException(nameof(methodExpression));

        if (!genericTypeDefinition.IsGenericTypeDefinition)
        {
            throw new ArgumentException($"{nameof(genericTypeDefinition)} must be a generic type definition");
        }

        List<Type> genericTypeMatches = type
            .GetSelfAndParentTypes()
            .Where(x => x.IsGenericType)
            .Where(x => x.GetGenericTypeDefinition() == genericTypeDefinition)
            .ToList();

        if (genericTypeMatches.Count == 0)
        {
            throw new ArgumentException($"{type.FullName} does not implement the generic type definition {genericTypeDefinition.FullName}");
        }

        if (genericTypeMatches.Count > 1)
        {
            throw new ArgumentException($"{type.FullName} implements the generic type definition {genericTypeDefinition.FullName} multiple time.");
        }

        Type genericType = genericTypeMatches[0];

        Expression body = methodExpression.Body;

        UnaryExpression? castExpression = body as UnaryExpression;
        if (castExpression != null && castExpression.NodeType == ExpressionType.Convert)
        {
            body = castExpression.Operand;
        }

        MethodCallExpression? methodCallExpression = body as MethodCallExpression;
        if (methodCallExpression == null)
        {
            throw new ArgumentException($"{methodExpression} must contain a MethodCallExpression");
        }

        MethodInfo templateMethodInfo = methodCallExpression.Method;
        Type[] methodGenericArguments = templateMethodInfo.GetGenericArguments();

        Type[] typeGenericArguments = genericType.GetGenericArguments();

        if (typeGenericArguments.Length > methodGenericArguments.Length)
        {
            throw new ArgumentException($"The method in {nameof(methodExpression)} must have at least {typeGenericArguments.Length} type arguments");
        }

        Type[] typeArguments = typeGenericArguments
            .Concat(methodGenericArguments.Skip(typeGenericArguments.Length))
            .ToArray();

        MethodInfo methodInfo = templateMethodInfo
            .GetGenericMethodDefinition()
            .MakeGenericMethod(typeArguments);

        MethodCallExpression methodCall = methodInfo.IsStatic
            ? Expression.Call(
                methodInfo,
                methodCallExpression.Arguments)
            : Expression.Call(
                methodCallExpression.Object,
                methodInfo,
                methodCallExpression.Arguments);

        if (castExpression != null)
        {
            return Expression.Convert(
                methodCall,
                castExpression.Type);
        }

        return methodCall;
    }
}
