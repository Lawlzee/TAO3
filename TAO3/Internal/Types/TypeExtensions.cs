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
                yield return interfaceTypes;
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
    }
}
