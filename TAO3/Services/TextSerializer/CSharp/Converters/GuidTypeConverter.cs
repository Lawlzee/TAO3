using System;
using System.Collections.Generic;
using System.Text;

namespace TAO3.TextSerializer.CSharp
{
    internal class GuidTypeConverter : TypeConverter<Guid>
    {
        public override bool Convert(StringBuilder sb, Guid obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append($"new Guid(\"{obj}\")");

            return true;
        }
    }
}
