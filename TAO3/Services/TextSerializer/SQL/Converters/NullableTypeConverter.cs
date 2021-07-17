using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class NullableTypeConverter<T> : TypeConverter<T?>
        where T : struct
    {
        public override bool Convert(StringBuilder sb, T? obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            //obj can't be null by convention
            serializer.Serialize(sb, obj!.Value, options);
            return true;
        }
    }
}
