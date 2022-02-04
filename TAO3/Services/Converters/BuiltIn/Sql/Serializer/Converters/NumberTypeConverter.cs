using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class NumberTypeConverter<T> : TypeConverter<T, SqlConverterSettings>
{
    public override bool Convert(T obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Append(obj!.ToString());
        return true;
    }
}
