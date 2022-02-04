using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class BoolTypeConverter : TypeConverter<bool>
{
    public override bool Convert(StringBuilder sb, bool obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append(obj ? "true" : "false");
        return true;
    }
}
