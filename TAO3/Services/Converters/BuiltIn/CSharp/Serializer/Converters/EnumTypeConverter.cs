using System.Text.RegularExpressions;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class EnumTypeConverter : TypeConverter<Enum, CSharpSerializerSettings>
{
    public override bool Convert(Enum obj, ObjectSerializerContext<CSharpSerializerSettings> context)
    {
        Type type = obj.GetType();

        string valueText = obj.ToString();

        bool isDefined = !Regex.IsMatch(valueText, @"^-?\d+$");
        if (isDefined)
        {
            string prettyType = type.PrettyPrint();
            string[] values = valueText.Split(", ");
            for (int i = 0; i < values.Length; i++)
            {
                if (i > 0)
                {
                    context.Append(" | ");
                }

                context.Append(prettyType);
                context.Append(".");
                context.Append(values[i]);
            }
            return true;
        }

        context.Append("(");
        context.Append(type.PrettyPrint());
        context.Append(")");

        bool isNegative = ((IConvertible)obj).ToInt64(null) < 0;
        if (isNegative)
        {
            context.Append("(");
        }

        context.Serialize(System.Convert.ChangeType(obj, type.GetEnumUnderlyingType()));

        if (isNegative)
        {
            context.Append(")");
        }

        return true;
    }
}
