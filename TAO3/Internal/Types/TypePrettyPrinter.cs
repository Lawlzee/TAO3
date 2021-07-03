using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TAO3.Internal.Types
{
    internal static class TypePrettyPrinter
    {
        private static readonly Dictionary<Type, string> _primitifTypeName = new Dictionary<Type, string>
        {
            { typeof(bool), "bool" },
            { typeof(byte), "byte" },
            { typeof(char), "char" },
            { typeof(decimal), "decimal" },
            { typeof(double), "double" },
            { typeof(float), "float" },
            { typeof(int), "int" },
            { typeof(long), "long" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(string), "string" },
            { typeof(uint), "uint" },
            { typeof(ulong), "ulong" },
            { typeof(ushort), "ushort" },
        };

        internal static string PrettyPrint(this Type type)
        {
            if (_primitifTypeName.ContainsKey(type))
            {
                return _primitifTypeName[type];
            }

            string prettyName;

            Type[] genericArguments = type.GetGenericArguments();

            if (type.IsArray)
            {
                prettyName = PrettyPrint(type.GetElementType()!) + $"[{new string(',', type.GetArrayRank() - 1)}]";
            }
            else if (type.IsPointer)
            {
                prettyName = PrettyPrint(type.GetElementType()!) + "*";
            }
            else if (genericArguments.Length == 0)
            {
                prettyName = type.Name;
            }
            else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                prettyName = PrettyPrint(genericArguments[0]) + "?";
            }
            else if (type.IsValueTuple())
            {
                prettyName = "(" + string.Join(", ", genericArguments.Select(PrettyPrint)) + ")";
            }
            else
            {
                string fullTypeName = type.Name;
                string typeNameWithGenerics = fullTypeName.Substring(0, fullTypeName.IndexOf("`"));
                prettyName = typeNameWithGenerics + "<" + string.Join(", ", genericArguments.Select(PrettyPrint)) + ">";
            }

            if (type.DeclaringType != null && !type.DeclaringType.Name.StartsWith("Submission#"))
            {
                return PrettyPrint(type.DeclaringType) + "." + prettyName;
            }

            return prettyName;
        }
    }
}
