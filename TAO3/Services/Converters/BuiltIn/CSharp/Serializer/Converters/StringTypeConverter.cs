using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class StringTypeConverter : TypeConverter<string, CSharpSerializerSettings>
{
    public override bool Convert(string str, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("@\"");
        context.Append(str.Replace("\"", "\"\""));
        context.Append("\"");

        return true;
    }
}
