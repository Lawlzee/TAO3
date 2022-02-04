using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class GuidTypeConverter : TypeConverter<Guid>
{
    public override bool Convert(StringBuilder sb, Guid obj, ObjectSerializer serializer, ObjectSerializerOptions options)
    {
        sb.Append($"'{obj.ToString().ToUpperInvariant()}'");
        return true;
    }
}
