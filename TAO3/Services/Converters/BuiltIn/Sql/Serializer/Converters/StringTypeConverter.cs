using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer;

namespace TAO3.Converters.SQL
{
    internal class StringTypeConverter : TypeConverter<string>
    {
        public override bool Convert(StringBuilder sb, string obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append("'");
            sb.Append(obj.ToString().Replace("'", "''"));
            sb.Append("'");
            return true;
        }
    }
}
