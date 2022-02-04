using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class GuidTypeConverter : TypeConverter<Guid, CSharpSerializerSettings>
{
    public override bool Convert(Guid obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append($"new Guid(\"{obj}\")");

        return true;
    }
}
