using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class CharTypeConverter : TypeConverter<char, SqlConverterSettings>
{
    public override bool Convert(char obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        //todo: escape
        context.Append($"'{obj}'");
        return true;
    }
}
