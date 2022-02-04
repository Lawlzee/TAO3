using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class BoolTypeConverter : TypeConverter<bool, SqlConverterSettings>
{
    public override bool Convert(bool obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append(obj ? "1" : "0");
        return true;
    }
}
