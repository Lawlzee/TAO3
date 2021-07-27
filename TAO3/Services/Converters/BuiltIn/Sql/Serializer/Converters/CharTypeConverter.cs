using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer;

namespace TAO3.Converters.Sql
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
