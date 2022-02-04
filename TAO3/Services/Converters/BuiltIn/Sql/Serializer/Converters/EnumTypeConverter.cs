using TAO3.TextSerializer;

namespace TAO3.Converters.Sql;

internal class EnumTypeConverter : TypeConverter<Enum, SqlConverterSettings>
{
    public override bool Convert(Enum obj, ObjectSerializerContext<SqlConverterSettings> context)
    {
        context.Serialize(System.Convert.ChangeType(obj, obj.GetType().GetEnumUnderlyingType()));
        return true;
    }
}
