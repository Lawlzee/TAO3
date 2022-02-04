using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class BoolTypeConverter : TypeConverter<bool, CSharpSerializerSettings>
{
    public override bool Convert(bool obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append(obj ? "true" : "false");
        return true;
    }
}
