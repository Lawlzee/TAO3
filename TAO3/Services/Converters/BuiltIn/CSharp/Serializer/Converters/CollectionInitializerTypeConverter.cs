using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class CollectionInitializerTypeConverter<T> : TypeConverter<IEnumerable<T>, CSharpSerializerSettings>
{
    public override bool Convert(IEnumerable<T> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        Type type = obj.GetType();

        int matches = type.GetMethods()
            .Where(x => x.Name == "Add")
            .Where(x => !x.IsStatic)
            .Where(x => x.GetParameters().Length == 1)
            .Where(x => x.GetParameters()[0].ParameterType == typeof(T))
            .Where(x => x.IsPublic || x.IsAssembly)
            .Count();

        if (matches != 1 && !type.IsArray)
        {
            return false;
        }

        bool isDictionary = type.IsAssignableToGenericType(typeof(IDictionary<,>));

        if (isDictionary)
        {
            return false;
        }

        List<T> values = obj.ToList();

        context.Append("new ");

        if (values.Count == 0)
        {
            if (type.IsArray)
            {
                //Ugly, but it kinds of works
                context.Append(obj.GetType().PrettyPrint()
                    .Replace("[", "[0")
                    .Replace(",", ", 0"));
            }
            else
            {
                context.Append(obj.GetType().PrettyPrint());
                context.Append("()");
            }

            return true;
        }

        context.AppendLine(obj.GetType().PrettyPrint());
        context.AppendIndentation();
        context.AppendLine("{");

        var elementContext = context.Indent();

        bool isFirst = true;
        foreach (T element in obj)
        {
            if (!isFirst)
            {
                elementContext.AppendLine(",");
            }

            elementContext.AppendIndentation();
            elementContext.Serialize(element);

            isFirst = false;
        }

        context.AppendLine();
        context.AppendIndentation();
        context.Append("}");

        return true;
    }
}
