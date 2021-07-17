using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class CharTypeConverter : TypeConverter<char>
    {
        public override bool Convert(StringBuilder sb, char obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            //todo: escape
            sb.Append($"'{obj}'");
            return true;
        }
    }
}
