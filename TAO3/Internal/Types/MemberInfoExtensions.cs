using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.Internal.Types
{
    internal static class MemberInfoExtensions
    {
        public static object? GetValue(this MemberInfo member, object obj)
        {
            return member.MemberType switch
            {
                MemberTypes.Field => ((FieldInfo)member).GetValue(obj),
                MemberTypes.Property => ((PropertyInfo)member).GetValue(obj),
                _ => throw new ArgumentNullException($"Invalid member type: '{member.MemberType}'. Expected Field or Property")
            };
        }
    }
}
