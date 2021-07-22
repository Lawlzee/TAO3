using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TAO3.TextSerializer;

namespace TAO3.Converters.SQL
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
