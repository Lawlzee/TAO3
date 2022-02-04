using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class ICSharpNodeTypeConverter : TypeConverter<ICSharpNode, CSharpSerializerSettings>
{
    public override bool Convert(ICSharpNode obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append(obj.Syntax.ToString());
        return true;
    }
}
