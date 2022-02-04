using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class StringTypeConverter : TypeConverter<string>
{
    public override bool Convert(StringBuilder sb, string str, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append("@\"");
        sb.Append(str.Replace("\"", "\"\""));
        sb.Append("\"");

        return true;
    }
}
