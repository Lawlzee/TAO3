using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class CharTypeConverter : TypeConverter<char, CSharpSerializerSettings>
{
    public override bool Convert(char obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append("'");
        //to do: escape caracteres
        context.Append(obj);
        context.Append("'");

        return true;
    }
}
