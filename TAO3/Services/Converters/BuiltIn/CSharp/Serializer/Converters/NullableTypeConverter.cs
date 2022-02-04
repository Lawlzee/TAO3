using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class NullableTypeConverter<T> : TypeConverter<T?, CSharpSerializerSettings>
    where T : struct
{
    public override bool Convert(T? obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        //obj can't be null by convention
        context.Serialize(obj!.Value);
        return true;
    }
}
