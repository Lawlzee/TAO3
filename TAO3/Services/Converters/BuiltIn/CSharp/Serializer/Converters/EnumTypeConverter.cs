using System.Text.RegularExpressions;
using TAO3.Internal.Types;
using TAO3.TextSerializer;

namespace TAO3.Converters.CSharp;

internal class EnumTypeConverter : TypeConverter<Enum>
{
    public override bool Convert(StringBuilder sb, Enum obj, ObjectSerializer serializer, ObjectSerializerOptions options)
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
                    sb.Append(" | ");
                }

                sb.Append(prettyType);
                sb.Append(".");
                sb.Append(values[i]);
            }
            return true;
        }

        sb.Append("(");
        sb.Append(type.PrettyPrint());
        sb.Append(")");

        bool isNegative = ((IConvertible)obj).ToInt64(null) < 0;
        if (isNegative)
        {
            sb.Append("(");
        }

        serializer.Serialize(sb, System.Convert.ChangeType(obj, type.GetEnumUnderlyingType()), options);

        if (isNegative)
        {
            sb.Append(")");
        }

        return true;
    }
}
