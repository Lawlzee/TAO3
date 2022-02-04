using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class DictionaryInitializerTypeConverter<TKey, TValue> : TypeConverter<IDictionary<TKey, TValue>, CSharpSerializerSettings>
{
    public override bool Convert(IDictionary<TKey, TValue> obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        if (obj.GetType().GetConstructor(Type.EmptyTypes) == null)
        {
            return false;
        }

        context.Append("new ");
        context.Append(obj.GetType().PrettyPrint());
        context.Append("()");

        if (obj.Count == 0)
        {
            return true;
        }

        context.AppendLine();
        context.AppendIndentation();
        context.AppendLine("{");

        ObjectSerializerContext<CSharpSerializerSettings> elementContext = context.Indent();

        bool isFirst = true;
        foreach (KeyValuePair<TKey, TValue> kvp in obj)
        {
            if (!isFirst)
            {
                elementContext.AppendLine(",");
            }

            elementContext.Append(elementContext.Indentation);
            elementContext.Append("[");
            elementContext.Serialize(kvp.Key);
            elementContext.Append("] = ");
            elementContext.Serialize(kvp.Value);
            isFirst = false;
        }

        context.AppendLine();
        context.AppendIndentation();
        context.Append("}");

        return true;
    }
}
