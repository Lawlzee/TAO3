using System.Reflection;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class AnonymousTypeConverter : TypeConverter<object, CSharpSerializerSettings>
{
    public override bool Convert(object obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        if (!obj.GetType().IsAnonymous())
        {
            return false;
        }

        context.AppendLine("new");
        context.Append(context.Indentation);
        context.Append("{");

        ObjectSerializerContext<CSharpSerializerSettings> propContext = context.Indent();

        PropertyInfo[] properties = obj.GetType().GetProperties();

        int i = 0;
        foreach (PropertyInfo propertyInfo in properties)
        {
            context.AppendLine();
            context.Append(propContext.Indentation);

            context.Append(propertyInfo.Name);
            context.Append(" = ");

            propContext.Serialize(propertyInfo.GetValue(obj));

            i++;
            if (i != properties.Length)
            {
                context.Append(",");
            }
        }

        context.AppendLine();
        context.AppendIndentation();
        context.Append("}");

        return true;
    }

    public override void Dispose()
    {

    }
}
