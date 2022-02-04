using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class StringTypeConverter : TypeConverter<string, SqlConverterSettings>
{
    public override bool Convert(string obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append("'");
        context.Append(obj.ToString().Replace("'", "''"));
        context.Append("'");
        return true;
    }
}
