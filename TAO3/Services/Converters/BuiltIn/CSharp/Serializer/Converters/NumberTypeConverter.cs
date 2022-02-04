using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class NumberTypeConverter<T> : TypeConverter<T, CSharpSerializerSettings>
{
    public override bool Convert(T obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        context.Append(obj!.ToString());
        string? suffix = NumberHelper.GetNumberSuffix(typeof(T));
        if (suffix != null)
        {
            context.Append(suffix);
        }

        return true;
    }
}
