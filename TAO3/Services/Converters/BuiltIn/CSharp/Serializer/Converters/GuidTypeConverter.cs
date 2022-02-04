using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class GuidTypeConverter : TypeConverter<Guid>
{
    public override bool Convert(StringBuilder sb, Guid obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append($"new Guid(\"{obj}\")");

        return true;
    }
}
