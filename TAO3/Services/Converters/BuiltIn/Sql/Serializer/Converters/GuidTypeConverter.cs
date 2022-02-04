using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class GuidTypeConverter : TypeConverter<Guid, SqlConverterSettings>
{
    public override bool Convert(Guid obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append($"'{obj.ToString().ToUpperInvariant()}'");
        return true;
    }
}
