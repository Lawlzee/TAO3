using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer;

namespace TAO3.Converters.Sql
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
