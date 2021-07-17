using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class NumberTypeConverter<T> : TypeConverter<T>
    {
        public override bool Convert(StringBuilder sb, T obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            sb.Append(obj!.ToString());
            return true;
        }
    }
}
