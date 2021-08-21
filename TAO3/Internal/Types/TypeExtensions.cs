using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.Internal.Types
{
    internal static class TypeExtensions
    {
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            yield return type;
            foreach (Type interfaceTypes in type.GetInterfaces())
            {
                foreach (Type parentType in GetParentTypes(interfaceTypes))
                {
                    yield return parentType;
                }
            }

            if (type.BaseType != null)
            {
                foreach (Type parentType in GetParentTypes(type.BaseType))
                {
                    yield return parentType;
                }
            }
        }

        //https://stackoverflow.com/questions/5461295/using-isassignablefrom-with-open-generic-types
        internal static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            Type[] interfaceTypes = givenType.GetInterfaces();

            foreach (Type it in interfaceTypes)
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                {
                    return true;
                }
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
            {
                return true;
            }

            Type? baseType = givenType.BaseType;
            if (baseType == null)
            {
                return false;
            }

            return IsAssignableToGenericType(baseType, genericType);
        }
    }
}
