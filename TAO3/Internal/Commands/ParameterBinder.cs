using FastExpressionCompiler;
using System.Linq.Expressions;
using System.Reflection;

namespace TAO3.Internal.Commands;

internal static class ParameterBinder
{
    public static Action<TObject> Create<TObject, TProperty>(TProperty value)
    {
        List<PropertyInfo> properties = typeof(TObject)
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)
            .Where(x => x.PropertyType == typeof(TProperty))
            .ToList();

        if (properties.Count == 0)
        {
            return x => { };
        }

        ParameterExpression objectParameter = Expression.Parameter(typeof(TObject));

        return Expression
            .Lambda<Action<TObject>>(
                Expression.Block(
                    properties
                        .Select(property => Expression.Assign(
                            Expression.Property(objectParameter, property),
                            Expression.Constant(value)))),
                objectParameter)
            .CompileFast();
    }
}
