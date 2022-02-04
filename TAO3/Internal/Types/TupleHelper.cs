namespace TAO3.Internal.Types;

internal static class TupleHelper
{
    private static readonly HashSet<Type> _valueTupleTypes = new HashSet<Type>(new Type[]
    {
        typeof(ValueTuple<>),
        typeof(ValueTuple<,>),
        typeof(ValueTuple<,,>),
        typeof(ValueTuple<,,,>),
        typeof(ValueTuple<,,,,>),
        typeof(ValueTuple<,,,,,>),
        typeof(ValueTuple<,,,,,,>),
        typeof(ValueTuple<,,,,,,,>)
    });

    public static bool IsValueTuple(this Type type)
    {
        if (type.IsGenericType)
        {
            type = type.GetGenericTypeDefinition();
        }

        return _valueTupleTypes.Contains(type);
    }
}
