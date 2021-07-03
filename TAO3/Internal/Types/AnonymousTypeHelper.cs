using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace TAO3.Internal.Types
{
    internal static class AnonymousTypeHelper
    {
        //https://stackoverflow.com/questions/2483023/how-to-test-if-a-type-is-anonymous
        //https://stackoverflow.com/questions/315146/anonymous-types-are-there-any-distingushing-characteristics/315186#315186
        internal static bool IsAnonymous(this Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.Namespace == null
                && Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }
    }
}
