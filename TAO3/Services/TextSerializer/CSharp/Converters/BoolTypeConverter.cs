using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.TextSerializer.CSharp
{
    internal class BoolTypeConverter : TypeConverter<bool>
    {
        public override bool Convert(StringBuilder sb, bool obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append(obj ? "true" : "false");
            return true;
        }
    }
}
