using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.TextSerializer.CSharp
{
    internal class StringTypeConverter : TypeConverter<string>
    {
        public override bool Convert(StringBuilder sb, string str, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append("@\"");
            sb.Append(str.Replace("\"", "\"\""));
            sb.Append("\"");

            return true;
        }
    }
}
