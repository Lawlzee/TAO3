using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class NullableTypeConverter<T> : TypeConverter<T?, SqlConverterSettings>
    where T : struct
{
    public override bool Convert(T? obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        //obj can't be null by convention
        context.Serialize(obj!.Value);
        return true;
    }
}
