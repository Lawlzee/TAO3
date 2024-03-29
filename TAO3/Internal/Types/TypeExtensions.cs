﻿namespace TAO3.Internal.Types;

internal static class TypeExtensions
{
    public static IEnumerable<Type> GetSelfAndParentTypes(this Type type)
    {
        yield return type;
        foreach (Type interfaceType in type.GetInterfaces())
        {
            yield return interfaceType;
        }

        Type? parentType = type.BaseType;
        while (parentType != null)
        {
            yield return parentType;
            parentType = parentType.BaseType;
        }
    }

    //https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types
    internal static bool IsAssignableToGenericType(this Type givenType, Type genericType)
    {
        return givenType.GetSelfAndParentTypes()
            .Where(x => x.IsGenericType)
            .Select(x => x.GetGenericTypeDefinition())
            .Where(x => x == genericType)
            .Any();
    }
}
