namespace TAO3.Internal.Types;

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

    internal static string PrettyPrint(this Type type, bool anymousClassAsDynamic = false)
    {
        if (_primitifTypeName.ContainsKey(type))
        {
            return _primitifTypeName[type];
        }

        Type[] genericArguments = type.GetGenericArguments();

        if (type.IsArray)
        {
            return PrettyPrint(type.GetElementType()!, anymousClassAsDynamic) + $"[{new string(',', type.GetArrayRank() - 1)}]";
        }
        else if (type.IsPointer)
        {
            return PrettyPrint(type.GetElementType()!, anymousClassAsDynamic) + "*";
        }
        else if (genericArguments.Length == 0)
        {
            return GetTypeName();
        }
        else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return PrettyPrint(genericArguments[0], anymousClassAsDynamic) + "?";
        }
        else if (type.IsValueTuple())
        {
            return "(" + string.Join(", ", genericArguments.Select(x => PrettyPrint(x, anymousClassAsDynamic))) + ")";
        }
        else if (anymousClassAsDynamic && type.IsAnonymous())
        {
            return "dynamic";
        }
        else
        {
            string fullTypeName = GetTypeName();
            string typeNameWithhoutGenerics = fullTypeName.Substring(0, fullTypeName.IndexOf("`"));
            return typeNameWithhoutGenerics + "<" + string.Join(", ", genericArguments.Select(x => PrettyPrint(x, anymousClassAsDynamic))) + ">";
        }

        string GetTypeName()
        {
            return type.DeclaringType != null && !type.DeclaringType.Name.StartsWith("Submission#")
                ? PrettyPrint(type.DeclaringType, anymousClassAsDynamic) + "." + type.Name
                : type.Name;
        }
    }

    internal static string PrettyPrintFullName(this Type type, bool anymousClassAsDynamic = false)
    {
        if (_primitifTypeName.ContainsKey(type))
        {
            return _primitifTypeName[type];
        }

        Type[] genericArguments = type.GetGenericArguments();

        if (type.IsArray)
        {
            return PrettyPrintFullName(type.GetElementType()!, anymousClassAsDynamic) + $"[{new string(',', type.GetArrayRank() - 1)}]";
        }
        else if (type.IsPointer)
        {
            return PrettyPrintFullName(type.GetElementType()!, anymousClassAsDynamic) + "*";
        }
        else if (genericArguments.Length == 0)
        {
            return GetTypeNameWithNamespace();
        }
        else if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return PrettyPrintFullName(genericArguments[0], anymousClassAsDynamic) + "?";
        }
        else if (type.IsValueTuple())
        {
            return "(" + string.Join(", ", genericArguments.Select(x => PrettyPrintFullName(x, anymousClassAsDynamic))) + ")";
        }
        else if (anymousClassAsDynamic && type.IsAnonymous())
        {
            return "dynamic";
        }
        else
        {
            string fullTypeName = GetTypeNameWithNamespace();
            string typeNameWithoutGenerics = fullTypeName.Substring(0, fullTypeName.IndexOf("`"));
            return typeNameWithoutGenerics + "<" + string.Join(", ", genericArguments.Select(x => PrettyPrintFullName(x, anymousClassAsDynamic))) + ">";
        }

        string GetTypeNameWithNamespace()
        {
            string typeName = type.DeclaringType != null && !type.DeclaringType.Name.StartsWith("Submission#")
                ? PrettyPrintFullName(type.DeclaringType, anymousClassAsDynamic) + "." + type.Name
                : type.Name;

            return type.Namespace != null
                ? type.Namespace + "." + typeName
                : typeName;
        }
    }
}
