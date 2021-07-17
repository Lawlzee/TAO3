using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TAO3.TextSerializer.SQL
{
    internal class EnumTypeConverter : TypeConverter<Enum>
    {
        public override bool Convert(StringBuilder sb, Enum obj, ObjectSerializer serializer, ObjectSerializerOptions options)
        {
            serializer.Serialize(sb, System.Convert.ChangeType(obj, obj.GetType().GetEnumUnderlyingType()), options);
            return true;
        }
    }
}
