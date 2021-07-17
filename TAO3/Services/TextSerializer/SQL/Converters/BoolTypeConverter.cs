using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class BoolTypeConverter : TypeConverter<bool>
    {
        public override bool Convert(StringBuilder sb, bool obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append(obj ? "1" : "0");
            return true;
        }
    }
}
