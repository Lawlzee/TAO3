using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class GuidTypeConverter : TypeConverter<Guid>
    {
        public override bool Convert(StringBuilder sb, Guid obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append($"'{obj.ToString().ToUpperInvariant()}'");
            return true;
        }
    }
}
